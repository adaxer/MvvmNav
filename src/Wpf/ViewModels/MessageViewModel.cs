using ADaxer.MvvmNav.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Wpf.ViewModels;

/// <summary>
/// Simple dialog view model used for built-in confirmation messages.
/// </summary>
internal partial class MessageViewModel : DialogViewModelBase
{
    /// <summary>
    /// Gets or sets the message shown in the confirmation dialog.
    /// </summary>
    [ObservableProperty]
    private string _message = string.Empty;
}
