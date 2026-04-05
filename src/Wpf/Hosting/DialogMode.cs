namespace ADaxer.MvvmNav.Wpf.Hosting;

/// <summary>
/// Specifies how dialogs are hosted in a WPF application.
/// </summary>
public enum DialogMode
{
    /// <summary>
    /// Displays dialogs inside the shell as an overlay.
    /// </summary>
    Overlay = 0,

    /// <summary>
    /// Displays dialogs inside a separate window.
    /// </summary>
    Window = 1
}
