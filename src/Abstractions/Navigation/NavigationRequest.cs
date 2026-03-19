namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Describes a requested navigation operation.
/// </summary>
public sealed class NavigationRequest
{
    /// <summary>
    /// Gets the target type that should be activated.
    /// This may be <c>null</c> for back navigation.
    /// </summary>
    public Type? TargetType { get; init; }

    /// <summary>
    /// Gets the navigation parameters passed to the request.
    /// </summary>
    public NavigationParameters Parameters { get; init; } = NavigationParameters.Empty;

    /// <summary>
    /// Gets the normalized navigation key that identifies the semantic navigation target.
    /// Equal keys represent the same target for navigation comparison purposes.
    /// </summary>
    public string? NavigationKey { get; init; }

    /// <summary>
    /// Gets a value indicating whether the request represents a back navigation.
    /// </summary>
    public bool IsBackNavigation { get; init; }
}
