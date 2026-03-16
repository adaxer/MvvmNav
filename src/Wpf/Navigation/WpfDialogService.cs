using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Wpf.Views;

namespace ADaxer.MvvmNav.Wpf.Navigation;

public class WpfDialogService : IDialogService
{
    public Task<bool> ConfirmAsync(string message, string? title = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(IDialogAware dialogContent, NavigationParameters parameters)
    {
        var result = await CreateAndShowDialogAsync(dialogContent, parameters);
        return new DialogResult<TResult>(result, dialogContent is IDialogResult<TResult> dialogResult ? dialogResult.Value : default(TResult));
    }

    async Task<DialogResult> IDialogService.ShowDialogAsync(IDialogAware dialogContent, NavigationParameters parameters)
    {
        var result = await CreateAndShowDialogAsync(dialogContent, parameters);
        return result;
    }

    private async Task<DialogResult> CreateAndShowDialogAsync(IDialogAware dialogContent, NavigationParameters parameters)
    {
        if (dialogContent is not IDialogCompletionSource completionSource)
            throw new InvalidOperationException(
                $"Dialog content '{dialogContent.GetType().FullName}' must inherit from DialogViewModelBase.");

        completionSource.ResetDialogCompletion();

        if (dialogContent is INavigationAware navigationAware)
            await navigationAware.OnNavigatedToAsync(parameters);

        var dlg = new WpfDialog
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            DataContext = dialogContent
        };

        dlg.SetBinding(WpfDialog.ContentProperty, "Content");

        dlg.ShowDialog();

        return await completionSource.CompletionTask;
    }
}
