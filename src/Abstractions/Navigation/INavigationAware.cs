namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Defines a contract for navigation targets that want to receive
/// notifications when they become active.
/// </summary>
/// <remarks>
/// Implement this interface on view models that need to react to
/// navigation events, such as loading data or applying parameters
/// passed through <see cref="NavigationContext"/>.
/// </remarks>
public interface INavigationAware
{
    /// <summary>
    /// Called after the navigation target has been activated.
    /// </summary>
    /// <param name="context">
    /// The navigation context containing parameters passed during navigation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous initialization of the target.
    /// </returns>
    Task OnNavigatedToAsync(NavigationContext context);
}
