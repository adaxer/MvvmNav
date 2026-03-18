using System.Diagnostics;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigationAware, ICanNavigateFrom
{
    private readonly IFileService _fileService;

    public SettingsViewModel(IFileService fileService)
    {
        _fileService = fileService;
        Title = "⚙️ Settings";
    }

    [ObservableProperty]
    private string _markdown = string.Empty;


    [ObservableProperty]
    private string _state = string.Empty;

    [ObservableProperty]
    private bool? _isChecked = null;

    [ObservableProperty]
    private bool _isDirty = false;

    partial void OnIsCheckedChanged(bool? value) => IsDirty = true;
    partial void OnStateChanged(string value) => IsDirty = true;

    public Task<NavigationGuardResult> CanNavigateFromAsync(NavigationRequest request, CancellationToken cancellationToken = default)
    {
        var result = IsDirty
           ? NavigationGuardResult.AskUser("There are changes. Do you want to keep them?", OnConfirmedOrNotAsync)
           : NavigationGuardResult.Allow();
        return Task.FromResult(result);
    }

    private async Task OnConfirmedOrNotAsync(DialogResult result, CancellationToken token)
    {
        Trace.TraceInformation("Confirmation Dialog said: {0}", result.IsConfirmed.HasValue ? result.IsConfirmed : "None");
        if (result.IsConfirmed == false)
        {
            State = string.Empty;
            IsChecked = null;
        }
        if (result.IsConfirmed.HasValue)
        {
            IsDirty = false;
        }
    }

    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
        Markdown = await _fileService.GetFileAsync(".\\Markdown\\settings.md");
    }
}
