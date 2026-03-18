
using System.Collections.ObjectModel;
using System.Windows.Input;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class ShellViewModel : ViewModelBase, IShellViewModel
{
    private readonly INavigationService _navigation;

    public ShellViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        navigation.NavigationStateChanged += NavigationStateChanged; ;

        NavigationItems =
        [
            new NavigationItem("🏠 Home","Overview", NavigateHomeCommand),
            new NavigationItem("ℹ️ About", "Simple dialog example", ShowAboutCommand),
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

/// <summary>
/// A simple way to combine a Name of a "Page" to show and a Command to navigate to it.
/// MvvmNav could offer something like this, but we leave it up to you (To add keyboard shortcuts, or icons, or navigation parameters).
/// </summary>
/// <param name="Title">The title of the navigation item.</param>
/// <param name="Command">The command to execute when the navigation item is selected.</param>
public sealed record NavigationItem(string Title, string Subtitle, ICommand Command);
