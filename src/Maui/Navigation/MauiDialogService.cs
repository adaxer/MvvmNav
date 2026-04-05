using ADaxer.MvvmNav.Abstractions;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Maui.Navigation;

/// <summary>
/// MAUI implementation of <see cref="IDialogService"/>.
/// </summary>
/// <remarks>
/// Dialogs are hosted inside the shell overlay via <see cref="IDialogHost"/>.
/// The service sets the current dialog, waits for completion, and then
/// clears the dialog again.
/// </remarks>
public class MauiDialogService : IDialogService
{
    private readonly IFactory<IDialogHost> _dialogHostFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MauiDialogService"/> class.
    /// </summary>
    /// <param name="dialogHostFactory">
    /// The factory used to lazily create the dialog host. The host is only created when the first dialog is shown, and then reused for all subsequent dialogs. This also prevents a circular dependency as IDialogHost is normally implemented by the ShellViewModel.
    /// </param>
    public MauiDialogService(IFactory<IDialogHost> dialogHostFactory)
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
                CommandInfos = [
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
        var result = await ShowCoreAsync(dialogContent, parameters);

        return new DialogResult<TResult>(
            result,
            dialogContent is IDialogResult<TResult> dialogResult ? dialogResult.Value : default);
    }

    /// <inheritdoc />
    public Task<DialogResult> ShowDialogAsync(
        IDialogController dialogContent,
        NavigationParameters parameters)
        => ShowCoreAsync(dialogContent, parameters);

    /// <summary>
    /// Shows the specified dialog in the current dialog host and waits for completion.
    /// </summary>
    /// <param name="dialogContent">
    /// The dialog view model.
    /// </param>
    /// <param name="parameters">
    /// The navigation parameters passed to the dialog.
    /// </param>
    /// <returns>
    /// The dialog result.
    /// </returns>
    private async Task<DialogResult> ShowCoreAsync(
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
