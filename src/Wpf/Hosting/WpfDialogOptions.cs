namespace ADaxer.MvvmNav.Wpf.Hosting;

/// <summary>
/// Provides configuration options for dialog hosting in WPF.
/// </summary>
public sealed class WpfDialogOptions
{
    /// <summary>
    /// Gets or sets the dialog hosting mode.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="DialogMode.Overlay"/>.
    /// </remarks>
    public DialogMode DialogMode { get; set; } = DialogMode.Overlay;
}
