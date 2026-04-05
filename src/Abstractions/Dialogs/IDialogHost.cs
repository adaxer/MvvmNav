namespace ADaxer.MvvmNav.Abstractions.Dialogs;

/// <summary>
/// Represents a shell-level host for the currently active dialog.
/// </summary>
public interface IDialogHost
{
    /// <summary>
    /// Gets or sets the currently active dialog view model.
    /// </summary>
    object? CurrentDialog { get; set; }
}
