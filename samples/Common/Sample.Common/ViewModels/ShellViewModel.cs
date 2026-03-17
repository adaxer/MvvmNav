
using System.Collections.ObjectModel;
using System.Windows.Input;
using ADaxer.MvvmNav.Abstractions.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class ShellViewModel : ObservableObject, IShellViewModel
{
    private readonly INavigationService _navigation;

    public ShellViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        navigation.NavigationStateChanged += (_,_) => (GoBackCommand as IAsyncRelayCommand)?.NotifyCanExecuteChanged();

        NavigationItems =
        [
            new NavigationItem("Home", NavigateHomeCommand),
            new NavigationItem("About", ShowAboutCommand),
            new NavigationItem("Settings", NavigateSettingsCommand)
        ];
    }

    [ObservableProperty]
    private object? currentModule;

    [ObservableProperty]
    private string title = "MvvmNav Sample";

    [ObservableProperty]
    private bool isBusy;

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

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private Task GoBack()
        => _navigation.GoBackAsync();

    private bool CanGoBack()
        => _navigation.CanGoBack();
}

/// <summary>
/// A simple way to combine a Name of a "Page" to show and a Command to navigate to it.
/// MvvmNav could offer something like this, but we leave it up to you (To add keyboard shortcuts, or icons, or navigation parameters).
/// </summary>
/// <param name="Title">The title of the navigation item.</param>
/// <param name="Command">The command to execute when the navigation item is selected.</param>
public sealed record NavigationItem(string Title, ICommand Command);
