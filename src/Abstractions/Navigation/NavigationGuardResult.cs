namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the outcome of a navigation guard check.
/// </summary>
public sealed class NavigationGuardResult
{
    public NavigationGuardResult(NavigationGuardDecision decision, NavigationParameters? context, Func<DialogResult, CancellationToken, Task>? continueAsync, CancellationToken cancellationToken)
    {
        Decision = decision;
        Context = context;
        ContinueAsync = continueAsync;
    }

    /// <summary>
    /// Gets the guard decision.
    /// </summary>
    public NavigationGuardDecision Decision { get; init; }

    public NavigationParameters? Context { get; private set; }
    public Func<DialogResult, CancellationToken, Task>? ContinueAsync { get; init; }

    public static NavigationGuardResult Allow() =>
        new(NavigationGuardDecision.Allow, null, null, CancellationToken.None);

    public static NavigationGuardResult Disallow() =>
        new(NavigationGuardDecision.Disallow, null, null, CancellationToken.None);
    public static NavigationGuardResult AskUser(object context,
        Func<DialogResult, CancellationToken, Task> continueAsync)
        => new(NavigationGuardDecision.AskUser, context as NavigationParameters, continueAsync, CancellationToken.None);
}

