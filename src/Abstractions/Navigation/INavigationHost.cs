namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the platform-specific component responsible for displaying
/// navigation targets within the user interface.
/// </summary>
/// <remarks>
/// Implementations typically integrate with a UI framework such as
/// Avalonia, WPF, MAUI, or Uno and are responsible for presenting the
/// supplied navigation target to the user.
/// 
/// The <see cref="INavigationService"/> delegates all UI-related work
/// to this host.
/// </remarks>
public interface INavigationHost
{
    /// <summary>
    /// Gets the currently displayed navigation target.
    /// </summary>
    /// <remarks>
    /// The returned object is typically the active view model that
    /// represents the current screen.
    /// </remarks>
    object? Current { get; }

    /// <summary>
    /// Displays the specified navigation target.
    /// </summary>
    /// <param name="target">
    /// The navigation target to display. This is typically a view model
    /// resolved from the dependency injection container.
    /// </param>
    /// <param name="options">
    /// Optional navigation options controlling how the target should
    /// be presented.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous display operation.
    /// </returns>
    Task ShowAsync(object target, NavigationOptions? options = null);
}
