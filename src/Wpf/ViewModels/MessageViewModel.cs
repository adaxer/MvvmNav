using System;
using System.Collections.Generic;
using System.Text;
using ADaxer.MvvmNav.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ADaxer.MvvmNav.Wpf.ViewModels;

internal partial class MessageViewModel : DialogViewModelBase
{
    [ObservableProperty]
    private string _message = string.Empty;
}
