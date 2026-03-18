using System.ComponentModel;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class FeaturesViewModel : ViewModelBase, INavigationAware
{
    private readonly IFileService _fileService;
    private readonly INavigationService _navigationService;

    public FeaturesViewModel(IFileService fileService, INavigationService navigationService)
    {
        _fileService = fileService;
        _navigationService = navigationService;
        Title= "✨ Features";
    }

    [ObservableProperty]
    private string _markdown = string.Empty;


    [ObservableProperty]
    private FeatureItem[] _features =
        [
            new FeatureItem { Name = "All Platforms", Key = "detail_platforms.md" },
            new FeatureItem { Name = "ViewModel-first navigation", Key = "detail_navigation.md" },
            new FeatureItem { Name = "Native view resolution", Key = "detail_navigation.md" },
            new FeatureItem { Name = "Navigation parameters", Key = "detail_navigation.md" },
            new FeatureItem { Name = "Back navigation", Key = "detail_navigation.md" },
            new FeatureItem { Name = "Navigation guards", Key = "detail_guards.md" },
            new FeatureItem { Name = "Dialog integration", Key = "detail_dialogs.md" },
            new FeatureItem { Name = "Generic factory support", Key = "detail_factory.md" },
            new FeatureItem { Name = "Dependency injection integration", Key = "detail_DI.md" },
            new FeatureItem { Name = "Logging support", Key = "detail_logging.md" }
        ];

    [RelayCommand]
    private async Task ShowFeatureAsync(string name)
    {
        var feature = Features.SingleOrDefault(f => f.Name == name);
        await _navigationService.NavigateAsync<DetailsViewModel>([("Key", feature?.Key ?? "notfound"), ("Referrer", feature?.Name ?? "notfound")]);
    }

    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        Markdown = await _fileService.GetFileAsync(".\\Markdown\\features.md");
    }
}

public class FeatureItem
{
    public string Name { get; internal set; } = string.Empty;
    public string Key { get; internal set; } = string.Empty;
}
