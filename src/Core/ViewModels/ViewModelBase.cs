
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private bool _isBusy;
}
