namespace ADaxer.MvvmNav.Avalonia.Hosting;

/// <summary>
/// Stores startup-related configuration for the Avalonia integration of MvvmNav.
/// </summary>
public sealed class MvvmNavOptions
{
    /// <summary>
    /// Gets or sets the configured shell view type.
    /// </summary>
    public Type? ShellViewType { get; set; }

    /// <summary>
    /// Gets or sets the configured shell view model type.
    /// </summary>
    public Type? ShellViewModelType { get; set; }

    /// <summary>
    /// Gets or sets the configured startup navigation target view model type.
    /// </summary>
    public Type? StartupNavigationType { get; set; }
}
