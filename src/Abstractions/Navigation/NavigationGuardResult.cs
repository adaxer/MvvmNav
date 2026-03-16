namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the outcome of a navigation guard check.
/// </summary>
public sealed class NavigationGuardResult<T> : NavigationGuardResult
{
    /// <summary>
    /// Gets the guard decision.
    /// </summary>
    public NavigationGuardDecision Decision { get; init; }

    /// <summary>
    /// Gets an optional message that may be shown to the user.
    /// </summary>
    public T? Context { get; init; }


    public static NavigationGuardResult<T> AskUser(T context) =>
        new() { Decision = NavigationGuardDecision.AskUser, Context = context };
}


/// <summary>
/// Represents the outcome of a navigation guard check.
/// </summary>
public class NavigationGuardResult
{
    /// <summary>
    /// Gets the guard decision.
    /// </summary>
    public NavigationGuardDecision Decision { get; init; }

    public static NavigationGuardResult Allow() =>
        new() { Decision = NavigationGuardDecision.Allow };

    public static NavigationGuardResult Disallow() =>
        new() { Decision = NavigationGuardDecision.Disallow };
}

