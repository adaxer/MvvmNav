using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Sample.Wpf.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class ShellWindow : Window, IShellView
{
    public ShellWindow()
    {
        InitializeComponent();
    }
}
