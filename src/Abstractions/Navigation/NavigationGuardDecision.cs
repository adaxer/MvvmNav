namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the decision returned by a navigation guard.
/// </summary>
public enum NavigationGuardDecision
{
    /// <summary>
    /// Navigation may proceed.
    /// </summary>
    Allow,

    /// <summary>
    /// Navigation must be cancelled.
    /// </summary>
    Disallow,

    /// <summary>
    /// The user should be asked before navigation continues.
    /// </summary>
    AskUser
}
