namespace ADaxer.MvvmNav.Abstractions.Dialogs;

/// <summary>
/// Represents the outcome of a dialog interaction.
/// </summary>
/// <remarks>
/// The <see cref="IsConfirmed"/> value uses a three-state model:
/// <list type="bullet">
/// <item><description><see langword="true"/> for a positive/confirming result</description></item>
/// <item><description><see langword="false"/> for a negative result that still completed the dialog</description></item>
/// <item><description><see langword="null"/> for cancellation or no decision</description></item>
/// </list>
/// </remarks>
public class DialogResult
{
    /// <summary>
    /// Gets a dialog result representing cancellation or no decision.
    /// </summary>
    public static DialogResult None { get; } = new(default);

    /// <summary>
    /// Gets a dialog result representing a negative decision.
    /// </summary>
    public static DialogResult False { get; } = new(false);

    /// <summary>
    /// Gets a dialog result representing a positive decision.
    /// </summary>
    public static DialogResult True { get; } = new(true);

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogResult"/> class.
    /// </summary>
    /// <param name="isConfirmed">
    /// The dialog confirmation state.
    /// </param>
    public DialogResult(bool? isConfirmed)
    {
        IsConfirmed = isConfirmed;
    }

    /// <summary>
    /// Gets the dialog confirmation state.
    /// </summary>
    public bool? IsConfirmed { get; }
}

/// <summary>
/// Represents the outcome of a dialog interaction including a typed payload.
/// </summary>
/// <typeparam name="TResult">
/// The dialog payload type.
/// </typeparam>
public sealed class DialogResult<TResult> : DialogResult, IDialogResult<TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogResult{TResult}"/> class.
    /// </summary>
    /// <param name="isConfirmed">
    /// The dialog confirmation state.
    /// </param>
    /// <param name="value">
    /// The optional payload returned by the dialog.
    /// </param>
    public DialogResult(bool? isConfirmed, TResult? value = default)
        : base(isConfirmed)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogResult{TResult}"/> class
    /// from an existing non-typed dialog result.
    /// </summary>
    /// <param name="fromResult">
    /// The source dialog result.
    /// </param>
    /// <param name="value">
    /// The optional payload returned by the dialog.
    /// </param>
    public DialogResult(DialogResult fromResult, TResult? value = default)
        : base(fromResult.IsConfirmed)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the payload returned by the dialog.
    /// </summary>
    public TResult? Value { get; }
}
