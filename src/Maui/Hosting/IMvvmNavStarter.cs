namespace ADaxer.MvvmNav.Maui.Hosting;

/// <summary>
/// Defines the MAUI startup workflow used to create the shell window and trigger initial navigation.
/// </summary>
public interface IMvvmNavStarter
{
    /// <summary>
    /// Creates or returns the cached application window for the configured shell.
    /// </summary>
    /// <param name="activationState">
    /// The MAUI activation state for the window creation request.
    /// </param>
    /// <returns>
    /// The application window that hosts the resolved shell page.
    /// </returns>
    Window CreateWindow(IActivationState? activationState);

    /// <summary>
    /// Runs the startup flow and performs configured startup navigation.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token observed before startup navigation begins.
    /// </param>
    /// <returns>
    /// A task representing the startup operation.
    /// </returns>
    Task StartAsync(CancellationToken cancellationToken = default);
}
