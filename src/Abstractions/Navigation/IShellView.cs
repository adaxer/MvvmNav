namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the application's main shell view.
/// </summary>
public interface IShellView
{
    /// <summary>
    /// Gets or sets the data context of the shell view.
    /// </summary>
    object? DataContext { get; set; }

    /// <summary>
    /// Shows the shell view.
    /// </summary>
    void Show();
}
