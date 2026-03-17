using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Wpf;

/// <summary>
/// Represents a started WPF navigation host.
/// </summary>
public sealed class WpfNavigationHost<TShellView, TShellViewModel>
    where TShellView : class, IShellView
    where TShellViewModel : class, IShellViewModel
{
    internal WpfNavigationHost(
        IServiceProvider services,
        TShellView shell,
        TShellViewModel shellViewModel)
    {
        Services = services;
        Shell = shell;
        ShellViewModel = shellViewModel;
    }

    public IServiceProvider Services { get; }

    public TShellView Shell { get; }

    public TShellViewModel ShellViewModel { get; }
}
