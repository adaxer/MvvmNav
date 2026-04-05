using System.Windows;
using ADaxer.MvvmNav.Abstractions;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Wpf.Hosting;
using ADaxer.MvvmNav.Wpf.Views;

namespace ADaxer.MvvmNav.Wpf.Navigation;

/// <summary>
/// WPF implementation of <see cref="IDialogService"/>.
/// </summary>
/// <remarks>
/// Dialogs are hosted in a <see cref="WpfDialogWindow"/> window and resolved
/// through the current WPF data templating setup.
/// </remarks>
public class WpfDialogService : IDialogService
{
    private readonly IFactory<IDialogHost> _dialogHostFactory;
    private readonly DialogMode _dialogMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfDialogService"/> class.
    /// </summary>
    /// <param name="dialogHostFactory">
    /// A factory that resolves the current <see cref="IDialogHost"/> instance used
    /// for hosting overlay dialogs in the shell.
    /// </param>
    /// <param name="dialogOptions">
    /// The dialog options that determine among other whether dialogs are shown as shell
    /// overlays or inside a separate <see cref="WpfDialogWindow"/>.
    /// </param>
    public WpfDialogService(IFactory<IDialogHost> dialogHostFactory, WpfDialogOptions dialogOptions)
    {
        ArgumentNullException.ThrowIfNull(dialogHostFactory);
        ArgumentNullException.ThrowIfNull(dialogOptions);
        _dialogHostFactory = dialogHostFactory;
        _dialogMode = dialogOptions.DialogMode;
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

        return ShowDialogAsync(dialogViewModel ?? throw new InvalidOperationException("Confirmation context must resolve to an IDialogController."), NavigationParameters.Empty);
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
    public async Task<DialogResult> ShowDialogAsync(
        IDialogController dialogContent,
        NavigationParameters parameters)
    {
        return await CreateAndShowDialogAsync(dialogContent, parameters);
    }

    /// <summary>
    /// Creates the host window, shows the dialog and waits for completion.
    /// </summary>
    /// <param name="dialogContent">
    /// The dialog view model.
    /// </param>
    /// <param name="parameters">
    /// The navigation parameters passed to the dialog.
    /// </param>
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

        IDialogHost dialogHost = null!; 
        if (_dialogMode == DialogMode.Overlay)
        {
            dialogHost = _dialogHostFactory.Create();
            dialogHost.CurrentDialog = dialogContent;
        }
        else
        {
            var dlg = new WpfDialogWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                DataContext = dialogContent
            };

            dlg.SetBinding(WpfDialogWindow.ContentProperty, ".");
            dlg.ShowDialog();
        }

        try
        {
            return await completionSource.CompletionTask;
        }
        finally
        {
            if (_dialogMode == DialogMode.Overlay && ReferenceEquals(dialogHost.CurrentDialog, dialogContent))
            {
                dialogHost.CurrentDialog = null;
            }
        }
    }
}
