namespace ADaxer.MvvmNav.Maui.Hosting;

/// <summary>
/// Describes the shell types and optional startup navigation used by MAUI startup integration.
/// </summary>
public sealed class StartupOptions
{
    /// <summary>
    /// Gets or sets the shell view type resolved to create the application's root page.
    /// </summary>
    public Type? ShellViewType { get; set; }

    /// <summary>
    /// Gets or sets the shell view model type resolved for the root page.
    /// </summary>
    public Type? ShellViewModelType { get; set; }

    /// <summary>
    /// Gets or sets the view model type navigated to during startup when one is configured.
    /// </summary>
    public Type? StartupNavigationType { get; set; }
}
