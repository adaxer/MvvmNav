using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class DetailsViewModel : ViewModelBase, INavigationAware
{
    private readonly FeatureService _featureService;
    private readonly ILogger<DetailsViewModel> _logger;
    private readonly INavigationService _navigationService;

    public DetailsViewModel(INavigationService navigationService, FeatureService featureService, ILogger<DetailsViewModel> logger)
    {
        _navigationService = navigationService;
        _featureService = featureService;
        _logger = logger;
    }

    [ObservableProperty]
    private string markdown = string.Empty;

    [ObservableProperty]
    private FeatureItem? _feature;

    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        try
        {
            var id = context.TryGetValue<int>("Id", out var value)
                ? value
                : 0;

            Feature = await _featureService.GetFeatureAsync(id);
            _logger.LogInformation("Navigated to feature: {Id}: {FeatureName} in File: {FeatureFile}", Feature.Id, Feature.Name, Feature.Key);
            Markdown = Feature.Markdown;
            Title = $"🔎 Feature: {context["Referrer"] ?? Feature.Name} ({Feature.Id})";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to feature");
            Markdown = "✋ **Feature not found**";
        }
    }

    [RelayCommand]
    private async Task NextAsync()
    {
        var nextFeature = await _featureService.GetNextFeatureAsync(Feature);
        await NavigateToFeatureAsync(nextFeature);
    }

    private async Task NavigateToFeatureAsync(FeatureItem feature)
    {
        _logger.LogDebug("Navigating to feature: {Id}: {FeatureName} in File: {FeatureFile}", feature.Id, feature.Name, feature.Key);
        await _navigationService.NavigateAsync<DetailsViewModel>([("Key", feature?.Key ?? "notfound"), ("Id", feature?.Id)]);
    }

    [RelayCommand]
    private async Task PreviousAsync()
    {
        var previousFeature = await _featureService.GetPreviousFeatureAsync(Feature);
        await NavigateToFeatureAsync(previousFeature);
    }
}
