namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Provides additional options for a navigation operation.
/// </summary>
public sealed class NavigationOptions
{
    /// <summary>
    /// Gets the default navigation options.
    /// </summary>
    public static NavigationOptions Default { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the current target should be added
    /// to the back stack before activating the new target.
    /// </summary>
    public bool AddToBackStack { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the back stack should be cleared
    /// before the navigation is performed.
    /// </summary>
    public bool ClearBackStack { get; init; }

    /// <summary>
    /// Gets the optional custom navigation key used to identify the semantic target.
    /// If not specified, the navigation service may derive a default key
    /// from the target type and navigation parameters.
    /// </summary>
    public string? NavigationKey { get; init; }

    /// <summary>
    /// Creates a new <see cref="NavigationOptions"/> instance with the specified navigation key.
    /// </summary>
    /// <param name="navigationKey">The navigation key to use.</param>
    /// <returns>A new <see cref="NavigationOptions"/> instance.</returns>
    public static NavigationOptions WithKey(string navigationKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(navigationKey);

        return new NavigationOptions
        {
            NavigationKey = navigationKey
        };
    }
}
