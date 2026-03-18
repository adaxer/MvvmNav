using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class AboutViewModel : DialogViewModelBase, INavigationAware
{
    private readonly IFileService _fileService;

    [ObservableProperty]
    private string _markdown = string.Empty;

    public AboutViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }
    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        Markdown = await _fileService.GetFileAsync(".\\Markdown\\about.md");
    }

    public override void CloseDialog(DialogResult result)
    {
        base.CloseDialog(result);
        Title = result.ToString();
    }
}
