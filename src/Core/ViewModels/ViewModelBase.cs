
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    protected ViewModelBase()
    {
        _title = GetType().Name;    
    }

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private bool _isBusy = false;
}
