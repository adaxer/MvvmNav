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
    public object? Context { get; private set; }
    public Func<DialogResult, CancellationToken, Task>? ContinueAsync { get; init; }

    public static NavigationGuardResult Allow() =>
        new() { Decision = NavigationGuardDecision.Allow };

    public static NavigationGuardResult Disallow() =>
        new() { Decision = NavigationGuardDecision.Disallow };
    public static NavigationGuardResult AskUser(object context,
        Func<DialogResult, CancellationToken, Task> continueAsync)
        => new()
        {
            Decision = NavigationGuardDecision.AskUser,
            Context = context,
            ContinueAsync = continueAsync
        };
}

