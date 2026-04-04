using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class AboutViewModel : DialogViewModelBase, INavigationAware, IDialogExchange
{
    private readonly IFileService _fileService;

    [ObservableProperty]
    private string _markdown = string.Empty;

    public AboutViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }

    public DialogExchangeInfo DialogExchange => new();

    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        Markdown = await _fileService.GetFileAsync(".\\Markdown\\about.md");
    }
}
