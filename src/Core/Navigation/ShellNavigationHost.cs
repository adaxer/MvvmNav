using ADaxer.MvvmNav.Abstractions.Navigation;
using CommunityToolkit.Mvvm.Input;

namespace ADaxer.MvvmNav.Core.Navigation;

/// <summary>
/// Default implementation of <see cref="INavigationHost"/> that displays
/// navigation targets through an <see cref="IShellViewModel"/>.
/// </summary>
/// <remarks>
/// This host updates the <see cref="IShellViewModel.CurrentModule"/> property
/// whenever navigation occurs.
/// 
/// After the navigation target has changed, the host optionally refreshes
/// the executable state of the command exposed through
/// <see cref="IShellViewModel.ShowItemCommand"/>. If the command implements
/// <see cref="IRelayCommand"/>, its
/// <see cref="IRelayCommand.NotifyCanExecuteChanged"/> method is invoked
/// so that the UI can update navigation-dependent commands (such as
/// a "Back" button).
/// </remarks>
public sealed class ShellNavigationHost : INavigationHost
{
    private readonly IShellViewModel _shell;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellNavigationHost"/> class.
    /// </summary>
    /// <param name="shell">
    /// The application's shell view model responsible for presenting
    /// the currently active navigation target.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="shell"/> is <c>null</c>.
    /// </exception>
    public ShellNavigationHost(IShellViewModel shell)
    {
        ArgumentNullException.ThrowIfNull(shell);
        _shell = shell;
    }

    /// <inheritdoc/>
    public object? Current => _shell.CurrentModule;

    /// <inheritdoc/>
    public Task ShowAsync(object target, NavigationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(target);

        _shell.CurrentModule = target;

        if (_shell.ShowItemCommand is IRelayCommand relayCommand)
            relayCommand.NotifyCanExecuteChanged();

        return Task.CompletedTask;
    }
}
