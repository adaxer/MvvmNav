using ADaxer.MvvmNav.Abstractions.Navigation;
using Avalonia.Controls;

namespace ADaxer.MvvmNav.Sample.Avalonia.Desktop.Views;

public partial class ShellWindow : Window, IAvaloniaShellView
{
    public ShellWindow()
    {
        InitializeComponent();
    }
}
