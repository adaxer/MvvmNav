using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Maui.Hosting;

/// <summary>
/// Represents a MAUI shell view that can accept a bound shell view model.
/// </summary>
public interface IMauiShellView : IShellView
{
    /// <summary>
    /// Gets or sets the object used as the binding context for the shell view.
    /// </summary>
    object? BindingContext { get; set; }
}
