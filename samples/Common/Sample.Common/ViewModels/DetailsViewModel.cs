using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class DetailsViewModel : ViewModelBase, INavigationAware
{
    private readonly IFileService _fileService;

    public DetailsViewModel(IFileService fileService)
    {
        _fileService = fileService;
            }

    [ObservableProperty]
    private string markdown = string.Empty;

    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        var key = context.TryGetValue<string>("Key", out var value)
            ? value
            : "notfound";

        var path = $".\\Markdown\\{key}";
        Title = $"🔎 Feature: {context["Referrer"]}";

        try
        {
            Markdown = await _fileService.GetFileAsync(path);
        }
        catch (Exception ex)
        {
            Markdown = "✋ **Feature not found**";
        }
    }
}
