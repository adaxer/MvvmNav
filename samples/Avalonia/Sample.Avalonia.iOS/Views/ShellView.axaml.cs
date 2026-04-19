using ADaxer.MvvmNav.Abstractions.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ADaxer.MvvmNav.Sample.Avalonia.iOS.Views;

public partial class ShellView : UserControl, IShellView
{
    public ShellView()
    {
        InitializeComponent();
    }
}
