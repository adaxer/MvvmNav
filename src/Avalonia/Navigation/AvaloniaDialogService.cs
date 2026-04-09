using ADaxer.MvvmNav.Abstractions;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Avalonia.Navigation;

/// <summary>
/// Avalonia implementation of <see cref="IDialogService"/>.
/// </summary>
/// <remarks>
/// Dialogs are hosted as overlays inside the current shell via <see cref="IDialogHost"/>.
/// This implementation assumes overlay-based dialog hosting.
/// </remarks>
public sealed class AvaloniaDialogService : IDialogService
{
    private readonly IFactory<IDialogHost> _dialogHostFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaDialogService"/> class.
    /// </summary>
    /// <param name="dialogHostFactory">
    /// A factory that resolves the current <see cref="IDialogHost"/> instance used
    /// for hosting overlay dialogs in the shell.
    /// </param>
    public AvaloniaDialogService(IFactory<IDialogHost> dialogHostFactory)
    {
        ArgumentNullException.ThrowIfNull(dialogHostFactory);
        _dialogHostFactory = dialogHostFactory;
    }

    /// <inheritdoc />
    public Task<DialogResult> ConfirmAsync(object context, CancellationToken cancellationToken = default)
    {
        var dialogViewModel = context is string message
            ? new MessageViewModel
            {
                Message = message,
                CommandInfos =
                [
                    new DialogCommandInfo("Yes", DialogResult.True) { IsPrimary = true },
                    new DialogCommandInfo("No", DialogResult.False),
                    new DialogCommandInfo("Cancel", DialogResult.None)
                ]
            }
            : context as IDialogController;

        return ShowDialogAsync(
            dialogViewModel ?? throw new InvalidOperationException(
                "Confirmation context must resolve to an IDialogController."),
            NavigationParameters.Empty);
    }

    /// <inheritdoc />
    public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        IDialogController dialogContent,
        NavigationParameters parameters)
    {
        var result = await CreateAndShowDialogAsync(dialogContent, parameters);

        return new DialogResult<TResult>(
            result,
            dialogContent is IDialogResult<TResult> dialogResult ? dialogResult.Value : default);
    }

    /// <inheritdoc />
    public Task<DialogResult> ShowDialogAsync(
        IDialogController dialogContent,
        NavigationParameters parameters)
    {
        return CreateAndShowDialogAsync(dialogContent, parameters);
    }

    /// <summary>
    /// Shows the specified dialog content in the current shell overlay and waits for completion.
    /// </summary>
    /// <param name="dialogContent">The dialog view model.</param>
    /// <param name="parameters">The navigation parameters passed to the dialog.</param>
    /// <returns>
    /// A task that completes when the dialog finishes and yields the resulting <see cref="DialogResult"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dialogContent"/> or <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the dialog content does not implement <see cref="IDialogCompletionSource"/>.
    /// </exception>
    private async Task<DialogResult> CreateAndShowDialogAsync(
        IDialogController dialogContent,
        NavigationParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(dialogContent);
        ArgumentNullException.ThrowIfNull(parameters);

        if (dialogContent is not IDialogCompletionSource completionSource)
        {
            throw new InvalidOperationException(
                $"Dialog content '{dialogContent.GetType().FullName}' must implement {nameof(IDialogCompletionSource)}.");
        }

        completionSource.ResetDialogCompletion();

        if (dialogContent is INavigationAware navigationAware)
        {
            await navigationAware.OnNavigatedToAsync(parameters);
        }

        var dialogHost = _dialogHostFactory.Create();
        dialogHost.CurrentDialog = dialogContent;

        try
        {
            return await completionSource.CompletionTask;
        }
        finally
        {
            if (ReferenceEquals(dialogHost.CurrentDialog, dialogContent))
            {
                dialogHost.CurrentDialog = null;
            }
        }
    }
}
