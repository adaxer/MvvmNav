using System.ComponentModel;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public class HomeViewModel : INavigationAware, INotifyPropertyChanged
{
    private readonly IFileService _fileService;

    public HomeViewModel(IFileService fileService, IPlatformNameProvider platformName)
    {
        _fileService = fileService;
        Title = $"🏠 Welcome to MvvmNav on {platformName.Name}!";
    }
    public string Title { get; }

    public string Markdown {  get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public  async Task OnNavigatedToAsync(NavigationParameters parameters)
    {
        Markdown = await _fileService.GetFileAsync("markdown", "home.md");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Markdown)));
    }
}
