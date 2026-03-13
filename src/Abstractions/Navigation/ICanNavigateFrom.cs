namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Defines a contract for navigation targets that can decide whether they may be left.
/// </summary>
public interface ICanNavigateFrom
{
    /// <summary>
    /// Determines whether navigation away from the current target is allowed.
    /// </summary>
    /// <param name="request">The requested navigation operation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task<NavigationGuardResult> CanNavigateFromAsync(
        NavigationRequest request,
        CancellationToken cancellationToken = default);
}
