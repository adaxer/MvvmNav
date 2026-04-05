namespace ADaxer.MvvmNav.Abstractions.Dialogs;

/// <summary>
/// Represents a command exposed by a dialog to the dialog host.
/// </summary>
/// <remarks>
/// A dialog command describes a user action that can be triggered from the
/// dialog host (e.g. a button).
/// 
/// Each command maps to a <see cref="DialogResult"/>, which may be used to
/// close the dialog.
/// </remarks>
public sealed class DialogCommandInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogCommandInfo"/> class.
    /// </summary>
    /// <param name="text">
    /// The display text of the command.
    /// </param>
    /// <param name="result">
    /// The dialog result associated with the command.
    /// </param>
    public DialogCommandInfo(string text, DialogResult result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentNullException.ThrowIfNull(result);

        Text = text;
        Result = result;
    }

    /// <summary>
    /// Gets the display text of the command.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the dialog result associated with the command.
    /// </summary>
    /// <remarks>
    /// This result is passed to the dialog completion logic when the command
    /// is executed.
    /// </remarks>
    public DialogResult Result { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this command represents the
    /// primary action of the dialog.
    /// </summary>
    /// <remarks>
    /// The dialog host may use this information to visually emphasize the
    /// command (e.g. highlight the button or apply a primary style).
    /// 
    /// This property does not affect dialog logic.
    /// </remarks>
    public bool IsPrimary { get; init; }
}
