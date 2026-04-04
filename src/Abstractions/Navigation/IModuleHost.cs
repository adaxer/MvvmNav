namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents a shell-level host for the currently active module accessible via navigation.
/// </summary>
public interface IModuleHost
{
    /// <summary>
    /// Gets or sets the currently active module view model.
    /// </summary>
    object? CurrentModule { get; set; }
}
