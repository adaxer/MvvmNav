namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Defines a contract for navigation targets that want to receive
/// notifications when they become active.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Called after the navigation target has been activated.
    /// </summary>
    /// <param name="context">
    /// The navigation context containing parameters passed during navigation.
    /// </param>
    Task OnNavigatedToAsync(NavigationParameters context);
}
