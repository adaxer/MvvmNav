namespace ADaxer.MvvmNav.Abstractions.Dialogs;

/// <summary>
/// Describes how a dialog should expose interaction capabilities to the host.
/// </summary>
/// <remarks>
/// This class provides metadata used by the dialog host to render command
/// buttons and to coordinate dialog completion.
/// 
/// It allows the dialog view model to:
/// <list type="bullet">
/// <item><description>define available commands</description></item>
/// <item><description>decide whether a dialog should close after a command</description></item>
/// <item><description>execute validation or side effects before closing</description></item>
/// </list>
/// </remarks>
public sealed class DialogExchangeInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogExchangeInfo"/> class.
    /// </summary>
    /// <param name="commands">
    /// The collection of commands exposed by the dialog.
    /// </param>
    public DialogExchangeInfo(IReadOnlyList<DialogCommandInfo>? commands = null)
    {
        Commands = commands ?? Array.Empty<DialogCommandInfo>();
    }

    /// <summary>
    /// Gets the commands exposed by the dialog.
    /// </summary>
    /// <remarks>
    /// The dialog host uses this collection to render command buttons.
    /// 
    /// If the collection is empty, the host may choose to hide the command
    /// bar or fall back to a default command (e.g. "OK").
    /// </remarks>
    public IReadOnlyList<DialogCommandInfo> Commands { get; }

    /// <summary>
    /// Gets or sets the callback that is invoked when a dialog command is executed.
    /// </summary>
    /// <remarks>
    /// The callback receives the selected <see cref="DialogResult"/> and may
    /// perform validation or side effects.
    /// 
    /// The returned boolean indicates whether the dialog should close:
    /// <list type="bullet">
    /// <item><description><see langword="true"/> → close dialog</description></item>
    /// <item><description><see langword="false"/> → keep dialog open</description></item>
    /// </list>
    /// 
    /// If no callback is provided, the dialog host will close the dialog
    /// immediately using the selected result.
    /// </remarks>
    public Func<DialogResult, CancellationToken, Task<bool>>? ContinueAsync { get; set; }
}
