using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Wpf.Views;

namespace ADaxer.MvvmNav.Wpf.Navigation;

/// <summary>
/// WPF implementation of <see cref="IDialogService"/>.
/// </summary>
/// <remarks>
/// Dialogs are hosted in a <see cref="WpfDialog"/> window and resolved
/// through the current WPF data templating setup.
/// </remarks>
public class WpfDialogService : IDialogService
{
    /// <inheritdoc />
    public Task<DialogResult> ConfirmAsync(object context, CancellationToken cancellationToken = default)
    {
        var dialogViewModel = context is string message
            ? new MessageViewModel { Message = message }
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

        var dlg = new WpfDialog
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            DataContext = dialogContent
        };

        dlg.SetBinding(WpfDialog.ContentProperty, ".");
        dlg.ShowDialog();

        return await completionSource.CompletionTask;
    }
}
