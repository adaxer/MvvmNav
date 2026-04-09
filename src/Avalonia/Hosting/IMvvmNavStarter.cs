using Avalonia;

namespace ADaxer.MvvmNav.Avalonia.Hosting;

/// <summary>
/// Starts and initializes the Avalonia-specific MvvmNav application shell.
/// </summary>
public interface IMvvmNavStarter
{
    /// <summary>
    /// Initializes MvvmNav for the specified Avalonia application and wires the shell
    /// into the current application lifetime.
    /// </summary>
    /// <param name="application">The current Avalonia application.</param>
    void Initialize(Application application);

    /// <summary>
    /// Executes startup navigation after the shell has been prepared.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous startup operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);
}
