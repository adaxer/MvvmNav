using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Core.Navigation;

/// <summary>
/// Default implementation of <see cref="IDialogHost"/> that delegates
/// dialog presentation to an <see cref="IDialogService"/>.
/// </summary>
/// <remarks>
/// The dialog host acts as an adapter between the navigation infrastructure
/// and the application's dialog service.
/// 
/// Dialog instances are typically resolved through the dependency injection
/// container and then displayed using the underlying dialog service.
/// </remarks>
public sealed class DialogHost : IDialogHost
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogHost"/> class.
    /// </summary>
    /// <param name="dialogService">
    /// The dialog service responsible for displaying dialogs.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="dialogService"/> is <c>null</c>.
    /// </exception>
    public DialogHost(IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        _dialogService = dialogService;
    }

    /// <inheritdoc/>
    public async Task<DialogResult> ShowDialogAsync(
        object dialog,
        NavigationContext context)
    {
        ArgumentNullException.ThrowIfNull(dialog);
        ArgumentNullException.ThrowIfNull(context);

        var parameters = context.Parameters.ToDictionary(x => x.Key, x => x.Value);

        var (ok, _) = await _dialogService.ShowDialogAsync(dialog, parameters);

        return ok ? DialogResult.Ok : DialogResult.Cancel;
    }

    /// <inheritdoc/>
    public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        object dialog,
        NavigationContext context)
    {
        ArgumentNullException.ThrowIfNull(dialog);
        ArgumentNullException.ThrowIfNull(context);

        var parameters = context.Parameters.ToDictionary(x => x.Key, x => x.Value);

        var (ok, returnedDialog) = await _dialogService.ShowDialogAsync(dialog, parameters);

        if (!ok)
            return new DialogResult<TResult>(false);

        if (returnedDialog is TResult typedResult)
            return new DialogResult<TResult>(true, typedResult);

        return new DialogResult<TResult>(true);
    }
}
