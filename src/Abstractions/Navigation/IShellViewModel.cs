using System.Windows.Input;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the application's main shell view model.
/// </summary>
/// <remarks>
/// The shell view model acts as the root of the application's visual
/// composition and hosts the currently active navigation target.
/// 
/// UI frameworks typically bind the shell view to this view model and
/// display the current module using a content control or similar
/// mechanism.
/// 
/// The navigation infrastructure updates <see cref="CurrentModule"/>
/// whenever navigation occurs.
/// </remarks>
public interface IShellViewModel
{
    /// <summary>
    /// Gets or sets the currently active navigation target.
    /// </summary>
    /// <remarks>
    /// The value is typically a view model representing the active screen.
    /// UI frameworks usually bind a content presenter or region to this
    /// property.
    /// </remarks>
    object? CurrentModule { get; set; }
}
