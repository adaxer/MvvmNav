using Avalonia.Controls;

namespace ADaxer.MvvmNav.Avalonia.Hosting;

/// <summary>
/// Minimal host window used when the configured shell view is not itself a window.
/// </summary>
public sealed class AvaloniaShellWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaShellWindow"/> class.
    /// </summary>
    /// <param name="content">The shell content to host.</param>
    public AvaloniaShellWindow(Control content)
    {
        Content = content;
    }
}
