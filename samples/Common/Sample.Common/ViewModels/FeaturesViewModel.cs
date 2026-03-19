using System.ComponentModel;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class FeaturesViewModel : ViewModelBase, INavigationAware
{
    private readonly IFileService _fileService;
    private readonly INavigationService _navigationService;
    private readonly FeatureService _featureService;

    public FeaturesViewModel(IFileService fileService, INavigationService navigationService, FeatureService featureService)
    {
        _fileService = fileService;
        _navigationService = navigationService;
        _featureService = featureService;
        Title= "✨ Features";
    }

    [ObservableProperty]
    private string _markdown = string.Empty;


    [ObservableProperty]
    private FeatureItem[] _features =Array.Empty<FeatureItem>();

    [RelayCommand]
    private async Task ShowFeatureAsync(string name)
    {
        var feature = Features.SingleOrDefault(f => f.Name == name);
        await _navigationService.NavigateAsync<DetailsViewModel>([("Id", feature?.Id), ("Referrer", feature?.Name ?? "notfound")]);
    }

    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        Features = await _featureService.GetFeaturesAsync();
        Markdown = await _fileService.GetFileAsync(".\\Markdown\\features.md");
    }
}
