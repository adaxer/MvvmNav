using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Wpf.ViewModels;
using ADaxer.MvvmNav.Wpf.Views;

namespace ADaxer.MvvmNav.Wpf.Navigation;

public class WpfDialogService : IDialogService
{
    public Task<DialogResult> ConfirmAsync(object context, CancellationToken cancellationToken = default)
    {
        var dialogViewModel = context is string message
            ? new MessageViewModel { Message = message }
            : context as IDialogController; ;
        return ShowDialogAsync(dialogViewModel, NavigationParameters.Empty);
    }

    public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(IDialogController dialogContent, NavigationParameters parameters)
    {
        var result = await CreateAndShowDialogAsync(dialogContent, parameters);
        return new DialogResult<TResult>(result, dialogContent is IDialogResult<TResult> dialogResult ? dialogResult.Value : default(TResult));
    }

    public async Task<DialogResult> ShowDialogAsync(IDialogController dialogContent, NavigationParameters parameters)
    {
        var result = await CreateAndShowDialogAsync(dialogContent, parameters);
        return result;
    }

    private async Task<DialogResult> CreateAndShowDialogAsync(IDialogController dialogContent, NavigationParameters parameters)
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

        dlg.SetBinding(WpfDialog.ContentProperty, ".");

        dlg.ShowDialog();

        return await completionSource.CompletionTask;
    }
}
