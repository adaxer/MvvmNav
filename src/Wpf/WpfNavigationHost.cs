using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Wpf;

/// <summary>
/// Represents a started WPF navigation host.
/// </summary>
/// <typeparam name="TShellView">
/// The shell view type.
/// </typeparam>
/// <typeparam name="TShellViewModel">
/// The shell view model type.
/// </typeparam>
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

    /// <summary>
    /// Gets the service provider used by the host.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets the resolved shell view.
    /// </summary>
    public TShellView Shell { get; }

    /// <summary>
    /// Gets the resolved shell view model.
    /// </summary>
    public TShellViewModel ShellViewModel { get; }
}
