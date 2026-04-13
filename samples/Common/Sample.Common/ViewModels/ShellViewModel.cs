
using System.Collections.ObjectModel;
using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class ShellViewModel : ViewModelBase, IShellViewModel, IDialogHost
{
    private readonly INavigationService _navigation;

    public ShellViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        navigation.NavigationStateChanged += NavigationStateChanged;

        NavigationItems =
        [
            new NavigationItem("🏠 Home","Overview", NavigateHomeCommand),
            new NavigationItem("ⓘ About", "Simple dialog example", ShowAboutCommand),
            new NavigationItem("⚙️ Settings", "Back Navigation example", NavigateSettingsCommand),
            new NavigationItem("✨ Features", "Framework overview", NavigateFeaturesCommand)
        ];
    }

    private void NavigationStateChanged(object? sender, EventArgs e)
    {
        (GoBackCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();
        Title = GetTitle();
    }

    private string GetTitle()
    {
        if(CurrentModule is null)
        {
               return "MvvmNav Sample";
        }

        var currentType = CurrentModule.GetType();
        return (CurrentModule as ViewModelBase)?.Title ?? 
            currentType.GetProperty("Title")?.GetValue(CurrentModule)?.ToString() ??
            currentType.Name;
    }

    [ObservableProperty]
    private object? _currentModule;

    [ObservableProperty]
    private object? _currentDialog;

    [ObservableProperty]
    private object? _title;

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    [RelayCommand]
    private Task NavigateHome()
        => _navigation.NavigateAsync<HomeViewModel>();

    [RelayCommand]
    private Task ShowAbout()
        => _navigation.ShowDialogAsync<AboutViewModel>();

    [RelayCommand]
    private Task NavigateSettings()
        => _navigation.NavigateAsync<SettingsViewModel>();

    [RelayCommand]
    private Task NavigateFeatures()
        => _navigation.NavigateAsync<FeaturesViewModel>();

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private Task GoBack()
        => _navigation.GoBackAsync();

    private bool CanGoBack()
        => _navigation.CanGoBack();
}
