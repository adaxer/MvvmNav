namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents a typed dialog result payload.
/// </summary>
/// <typeparam name="TResult">
/// The payload type.
/// </typeparam>
public interface IDialogResult<TResult>
{
    /// <summary>
    /// Gets the value returned by the dialog.
    /// </summary>
    TResult? Value { get; }
}
