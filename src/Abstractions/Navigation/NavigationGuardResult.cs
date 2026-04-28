using ADaxer.MvvmNav.Abstractions.Dialogs;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents the outcome of a navigation guard check.
/// </summary>
/// <remarks>
/// Use the static factory methods to create instances for the common cases.
/// When <see cref="Decision"/> is <see cref="NavigationGuardDecision.AskUser"/>,
/// <see cref="Context"/> and <see cref="ContinueAsync"/> are expected to be provided.
/// </remarks>
public sealed class NavigationGuardResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationGuardResult"/> class.
    /// </summary>
    /// <param name="decision">
    /// The guard decision.
    /// </param>
    /// <param name="context">
    /// Optional dialog context used when the user needs to be asked.
    /// </param>
    /// <param name="continueAsync">
    /// Optional callback invoked after the confirmation dialog completed.
    /// </param>
    /// <param name="cancellationToken">
    /// Reserved for future use.
    /// </param>
    public NavigationGuardResult(
        NavigationGuardDecision decision,
        object? context,
        Func<DialogResult, CancellationToken, Task>? continueAsync,
        CancellationToken cancellationToken)
    {
        Decision = decision;
        Context = context;
        ContinueAsync = continueAsync;
    }

    /// <summary>
    /// Gets the guard decision.
    /// </summary>
    public NavigationGuardDecision Decision { get; set; }

    /// <summary>
    /// Gets the optional dialog context used when the guard asks the user.
    /// </summary>
    public object? Context { get; private set; }

    /// <summary>
    /// Gets the continuation callback invoked after the confirmation dialog completed.
    /// </summary>
    public Func<DialogResult, CancellationToken, Task>? ContinueAsync { get; set; }

    /// <summary>
    /// Creates a result allowing navigation to continue.
    /// </summary>
    public static NavigationGuardResult Allow() =>
        new(NavigationGuardDecision.Allow, null, null, CancellationToken.None);

    /// <summary>
    /// Creates a result blocking navigation.
    /// </summary>
    public static NavigationGuardResult Disallow() =>
        new(NavigationGuardDecision.Disallow, null, null, CancellationToken.None);

    /// <summary>
    /// Creates a result that asks the user before navigation continues.
    /// </summary>
    /// <param name="context">
    /// The dialog context shown to the user.
    /// </param>
    /// <param name="continueAsync">
    /// The callback executed after the dialog produced a result.
    /// </param>
    public static NavigationGuardResult AskUser(
        object? context,
        Func<DialogResult, CancellationToken, Task> continueAsync)
        => new(NavigationGuardDecision.AskUser, context, continueAsync, CancellationToken.None);
}
