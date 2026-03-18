using System.ComponentModel;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public class HomeViewModel : INavigationAware, INotifyPropertyChanged
{
    private readonly IFileService _fileService;

    public HomeViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }
    public string Title => "🏠 Welcome to MvvmNav!";

    public string Markdown {  get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public  async Task OnNavigatedToAsync(NavigationParameters context)
    {
        Markdown = await _fileService.GetFileAsync(".\\Markdown\\home.md");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Markdown)));
    }
}
