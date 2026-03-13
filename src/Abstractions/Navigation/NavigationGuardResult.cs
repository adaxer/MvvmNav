namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the outcome of a navigation guard check.
/// </summary>
public sealed class NavigationGuardResult
{
    /// <summary>
    /// Gets the guard decision.
    /// </summary>
    public NavigationGuardDecision Decision { get; init; }

    /// <summary>
    /// Gets an optional message that may be shown to the user.
    /// </summary>
    public string? Message { get; init; }

    public static NavigationGuardResult Allow() =>
        new() { Decision = NavigationGuardDecision.Allow };

    public static NavigationGuardResult Disallow() =>
        new() { Decision = NavigationGuardDecision.Disallow };

    public static NavigationGuardResult AskUser(string? message) =>
        new() { Decision = NavigationGuardDecision.AskUser, Message = message };
}
