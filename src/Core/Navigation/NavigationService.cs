using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Core.Navigation;

/// <summary>
/// Default implementation of <see cref="INavigationService"/>.
/// </summary>
/// <remarks>
/// This service is the central orchestration point for:
/// <list type="bullet">
/// <item><description>view model navigation</description></item>
/// <item><description>back stack management</description></item>
/// <item><description>navigation guard evaluation</description></item>
/// <item><description>dialog integration</description></item>
/// <item><description>activation of navigation-aware targets</description></item>
/// </list>
/// </remarks>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private readonly IDialogService _dialogService;
    private readonly ILogger<NavigationService> _logger;
    private readonly Stack<NavigationEntry> _backStack = new();

    private IShellViewModel? _shell;
    private NavigationEntry? _currentEntry;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="services">The application service provider.</param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="logger">The logger instance.</param>
    public NavigationService(
        IServiceProvider services,
        IDialogService dialogService,
        ILogger<NavigationService> logger)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(logger);

        _services = services;
        _dialogService = dialogService;
        _logger = logger;
    }

    /// <inheritdoc />
    public event EventHandler? NavigationStateChanged;

    private IShellViewModel Shell =>
        _shell ??= _services.GetRequiredService<IShellViewModel>();

    /// <inheritdoc />
    public bool CanGoBack() => _backStack.Count > 0;

    /// <inheritdoc />
    public Task NavigateAsync<TTarget>(
        NavigationParameters? parameters = null,
        NavigationOptions? options = null)
        where TTarget : class
        => NavigateAsync(typeof(TTarget), parameters, options);

    /// <inheritdoc />
    public async Task NavigateAsync(
        Type targetType,
        NavigationParameters? parameters = null,
        NavigationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        parameters ??= NavigationParameters.Empty;
        options ??= NavigationOptions.Default;

        var navigationKey = BuildNavigationKey(targetType, parameters, options);

        _logger.LogDebug(
            "Navigation requested. Target={TargetType}, NavigationKey={NavigationKey}, ClearBackStack={ClearBackStack}, AddToBackStack={AddToBackStack}",
            targetType.FullName,
            navigationKey,
            options.ClearBackStack,
            options.AddToBackStack);

        var target = _services.GetRequiredService(targetType);
        var targetEntry = new NavigationEntry(target, targetType, parameters, navigationKey);

        await NavigateCoreAsync(
            targetEntry,
            isBackNavigation: false,
            clearBackStack: options.ClearBackStack,
            addCurrentToBackStack: options.AddToBackStack);
    }

    /// <inheritdoc />
    public async Task GoBackAsync()
    {
        if (!CanGoBack())
        {
            _logger.LogDebug("GoBack requested, but back stack is empty.");
            return;
        }

        _logger.LogDebug("Back navigation requested.");

        var targetEntry = _backStack.Peek();

        await NavigateCoreAsync(
            targetEntry,
            isBackNavigation: true,
            clearBackStack: false,
            addCurrentToBackStack: false);
    }

    /// <inheritdoc />
    public Task<DialogResult> ShowDialogAsync<TDialog>(NavigationParameters? parameters = null)
        where TDialog : class
        => ShowDialogAsync(typeof(TDialog), parameters);

    /// <inheritdoc />
    public Task<DialogResult> ShowDialogAsync(
        Type dialogType,
        NavigationParameters? parameters = null)
        => ShowDialogCoreAsync(dialogType, parameters ?? NavigationParameters.Empty);

    /// <inheritdoc />
    public Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationParameters? parameters = null)
        where TDialog : class
        => ShowDialogAsync<TResult>(typeof(TDialog), parameters);

    /// <inheritdoc />
    public Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        Type dialogType,
        NavigationParameters? parameters = null)
        => ShowDialogCoreAsync<TResult>(dialogType, parameters ?? NavigationParameters.Empty);

    /// <summary>
    /// Executes the shared navigation flow for both forward and back navigation.
    /// </summary>
    /// <param name="targetEntry">
    /// The target entry to activate.
    /// </param>
    /// <param name="isBackNavigation">
    /// Indicates whether the navigation is a back navigation.
    /// </param>
    /// <param name="clearBackStack">
    /// Indicates whether the existing back stack should be cleared
    /// before navigation.
    /// </param>
    /// <param name="addCurrentToBackStack">
    /// Indicates whether the current entry should be pushed onto
    /// the back stack.
    /// </param>
    private async Task NavigateCoreAsync(
        NavigationEntry targetEntry,
        bool isBackNavigation,
        bool clearBackStack,
        bool addCurrentToBackStack)
    {
        var request = new NavigationRequest
        {
            TargetType = targetEntry.TargetType,
            Parameters = targetEntry.Parameters,
            NavigationKey = targetEntry.NavigationKey,
            IsBackNavigation = isBackNavigation
        };

        var decision = await CanLeaveCurrentAsync(request);

        if (!decision.ShouldProceed)
        {
            _logger.LogInformation(
                "{NavigationKind} cancelled/disallowed by guard. Target={TargetType}, NavigationKey={NavigationKey}",
                isBackNavigation ? "Back navigation" : "Navigation",
                targetEntry.TargetType.FullName,
                targetEntry.NavigationKey);

            return;
        }

        if (isBackNavigation)
        {
            targetEntry = _backStack.Pop();
        }
        else
        {
            if (clearBackStack)
            {
                _logger.LogDebug("Clearing back stack before navigation.");
                _backStack.Clear();
            }

            if (addCurrentToBackStack && _currentEntry is not null)
            {
                _logger.LogDebug(
                    "Pushing current module onto back stack. CurrentType={CurrentType}, NavigationKey={NavigationKey}",
                    _currentEntry.TargetType.FullName,
                    _currentEntry.NavigationKey);

                _backStack.Push(_currentEntry);
            }
        }

        _logger.LogDebug(
            "{NavigationKind} target resolved. TargetType={TargetType}, NavigationKey={NavigationKey}",
            isBackNavigation ? "Back navigation" : "Navigation",
            targetEntry.TargetType.FullName,
            targetEntry.NavigationKey);

        await ActivateAsync(targetEntry.Target, targetEntry.Parameters, targetEntry.NavigationKey);

        _logger.LogInformation(
            "{NavigationKind} completed. TargetType={TargetType}, NavigationKey={NavigationKey}",
            isBackNavigation ? "Back navigation" : "Navigation",
            targetEntry.TargetType.FullName,
            targetEntry.NavigationKey);
    }

    /// <summary>
    /// Evaluates whether the current target may be left.
    /// </summary>
    /// <param name="request">
    /// The requested navigation operation.
    /// </param>
    /// <returns>
    /// A decision describing whether navigation should proceed.
    /// </returns>
    private async Task<LeaveDecision> CanLeaveCurrentAsync(NavigationRequest request)
    {
        if (_currentEntry is not null &&
            _currentEntry.TargetType == request.TargetType &&
            string.Equals(_currentEntry.NavigationKey, request.NavigationKey, StringComparison.Ordinal))
        {
            _logger.LogDebug(
                "Cannot navigate to the same target. TargetType={TargetType}, NavigationKey={NavigationKey}",
                request.TargetType?.FullName,
                request.NavigationKey);

            return new LeaveDecision(false, DialogResult.None);
        }

        if (Shell.CurrentModule is not ICanNavigateFrom guarded)
        {
            _logger.LogDebug("Current module has no navigation guard.");
            return new LeaveDecision(true, DialogResult.True);
        }

        _logger.LogDebug(
            "Evaluating navigation guard for current module. CurrentType={CurrentType}, IsBackNavigation={IsBackNavigation}, TargetType={TargetType}, NavigationKey={NavigationKey}",
            Shell.CurrentModule?.GetType().FullName,
            request.IsBackNavigation,
            request.TargetType?.FullName,
            request.NavigationKey);

        var result = await guarded.CanNavigateFromAsync(request);

        _logger.LogDebug(
            "Navigation guard returned decision {Decision}.",
            result.Decision);

        return result.Decision switch
        {
            NavigationGuardDecision.Allow => new LeaveDecision(true, DialogResult.True),
            NavigationGuardDecision.Disallow => new LeaveDecision(false, DialogResult.False),
            NavigationGuardDecision.AskUser => await GetAskUserDecisionAsync(result),
            _ => new LeaveDecision(false, DialogResult.None)
        };
    }

    /// <summary>
    /// Converts an <see cref="NavigationGuardDecision.AskUser"/> result
    /// into a concrete leave decision.
    /// </summary>
    /// <param name="result">
    /// The guard result requesting user interaction.
    /// </param>
    /// <returns>
    /// A decision indicating whether navigation should proceed.
    /// </returns>
    /// <remarks>
    /// According to the framework semantics:
    /// <list type="bullet">
    /// <item><description><see cref="DialogResult.True"/> → proceed</description></item>
    /// <item><description><see cref="DialogResult.False"/> → proceed</description></item>
    /// <item><description><see cref="DialogResult.None"/> → cancel</description></item>
    /// </list>
    /// </remarks>
    private async Task<LeaveDecision> GetAskUserDecisionAsync(NavigationGuardResult result)
    {
        var confirmation = await ConfirmNavigationAsync(result);

        return confirmation.IsConfirmed.HasValue
            ? new LeaveDecision(true, confirmation)
            : new LeaveDecision(false, confirmation);
    }

    /// <summary>
    /// Builds the semantic navigation key for a target.
    /// </summary>
    /// <param name="targetType">
    /// The target type.
    /// </param>
    /// <param name="parameters">
    /// The navigation parameters.
    /// </param>
    /// <param name="options">
    /// The navigation options.
    /// </param>
    /// <returns>
    /// The navigation key.
    /// </returns>
    private static string BuildNavigationKey(
        Type targetType,
        NavigationParameters parameters,
        NavigationOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.NavigationKey))
        {
            return options.NavigationKey;
        }

        var normalizedParameters = parameters.ToNormalizedString();
        var typeName = targetType.FullName ?? targetType.Name;

        return string.IsNullOrEmpty(normalizedParameters)
            ? typeName
            : $"{normalizedParameters}|{typeName}";
    }

    /// <summary>
    /// Shows a non-typed dialog after resolving and initializing it.
    /// </summary>
    /// <param name="dialogType">
    /// The dialog type.
    /// </param>
    /// <param name="context">
    /// The dialog parameters.
    /// </param>
    /// <returns>
    /// The dialog result.
    /// </returns>
    private async Task<DialogResult> ShowDialogCoreAsync(
        Type dialogType,
        NavigationParameters context)
    {
        var dialogController = await ResolveDialogAsync(dialogType, context);

        var result = await _dialogService.ShowDialogAsync(dialogController, context);

        _logger.LogInformation(
            "Dialog completed. DialogType={DialogType}, Confirmed={Confirmed}",
            dialogType.FullName,
            result.IsConfirmed);

        return result;
    }

    /// <summary>
    /// Shows a typed dialog after resolving and initializing it.
    /// </summary>
    /// <typeparam name="TResult">
    /// The dialog payload type.
    /// </typeparam>
    /// <param name="dialogType">
    /// The dialog type.
    /// </param>
    /// <param name="context">
    /// The dialog parameters.
    /// </param>
    /// <returns>
    /// The typed dialog result.
    /// </returns>
    private async Task<DialogResult<TResult>> ShowDialogCoreAsync<TResult>(
        Type dialogType,
        NavigationParameters context)
    {
        var dialogController = await ResolveDialogAsync(dialogType, context);

        var result = await _dialogService.ShowDialogAsync<TResult>(dialogController, context);

        _logger.LogInformation(
            "Dialog completed. DialogType={DialogType}, Confirmed={Confirmed}",
            dialogType.FullName,
            result.IsConfirmed);

        return result;
    }

    /// <summary>
    /// Resolves and initializes a dialog instance.
    /// </summary>
    /// <param name="dialogType">
    /// The dialog type to resolve.
    /// </param>
    /// <param name="context">
    /// The dialog parameters.
    /// </param>
    /// <returns>
    /// The resolved dialog controller.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved dialog does not implement
    /// <see cref="IDialogController"/>.
    /// </exception>
    private async Task<IDialogController> ResolveDialogAsync(
        Type dialogType,
        NavigationParameters context)
    {
        ArgumentNullException.ThrowIfNull(dialogType);

        _logger.LogDebug(
            "Dialog requested. DialogType={DialogType}",
            dialogType.FullName);

        var dialog = _services.GetRequiredService(dialogType);

        if (dialog is not IDialogController dialogController)
        {
            _logger.LogError(
                "Resolved dialog does not implement IDialogController. DialogType={DialogType}",
                dialogType.FullName);

            throw new InvalidOperationException(
                $"Dialog type '{dialogType.FullName}' must implement IDialogController.");
        }

        if (dialog is INavigationAware aware)
        {
            await aware.OnNavigatedToAsync(context);
        }

        return dialogController;
    }

    /// <summary>
    /// Activates the specified target.
    /// </summary>
    /// <param name="target">
    /// The target instance to activate.
    /// </param>
    /// <param name="context">
    /// The navigation parameters.
    /// </param>
    /// <param name="navigationKey">
    /// The semantic navigation key of the target.
    /// </param>
    protected virtual async Task ActivateAsync(object target, NavigationParameters context, string navigationKey)
    {
        _logger.LogDebug(
            "Activating target. TargetType={TargetType}, NavigationKey={NavigationKey}",
            target.GetType().FullName,
            navigationKey);

        Shell.CurrentModule = target;

        _currentEntry = new NavigationEntry(
            target,
            target.GetType(),
            context,
            navigationKey);

        if (target is INavigationAware aware)
        {
            await aware.OnNavigatedToAsync(context);
        }

        NavigationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Shows the navigation confirmation dialog for a guard
    /// requesting user interaction.
    /// </summary>
    /// <param name="result">
    /// The guard result.
    /// </param>
    /// <returns>
    /// The dialog result returned by the confirmation dialog.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the guard requests user interaction but no context
    /// is provided.
    /// </exception>
    private async Task<DialogResult> ConfirmNavigationAsync(NavigationGuardResult result)
    {
        if (result.Context is null)
        {
            _logger.LogError(
                "Navigation guard requested AskUser, but context was null.");

            throw new ArgumentNullException(
                nameof(result.Context),
                "Context can not be null, it is needed to show the ask user dialog");
        }

        _logger.LogDebug("Showing navigation confirmation dialog.");

        var confirmation = await _dialogService.ConfirmAsync(result.Context);

        await result.ContinueAsync(confirmation, CancellationToken.None);

        _logger.LogInformation(
            "Navigation confirmation completed. Confirmed={Confirmed}",
            confirmation.IsConfirmed);

        return confirmation;
    }

    /// <summary>
    /// Represents a semantic navigation target and its state.
    /// </summary>
    /// <param name="Target">
    /// The resolved target instance.
    /// </param>
    /// <param name="TargetType">
    /// The target type.
    /// </param>
    /// <param name="Parameters">
    /// The navigation parameters associated with the target.
    /// </param>
    /// <param name="NavigationKey">
    /// The semantic navigation key.
    /// </param>
    private sealed record NavigationEntry(
        object Target,
        Type TargetType,
        NavigationParameters Parameters,
        string NavigationKey);

    /// <summary>
    /// Represents the result of evaluating whether the current target
    /// may be left.
    /// </summary>
    /// <param name="ShouldProceed">
    /// Indicates whether navigation should continue.
    /// </param>
    /// <param name="Result">
    /// The underlying dialog result associated with the decision.
    /// </param>
    private readonly record struct LeaveDecision(
        bool ShouldProceed,
        DialogResult Result);
}
