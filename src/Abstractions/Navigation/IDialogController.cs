namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents a dialog that can actively close itself and provide a result.
/// </summary>
/// <remarks>
/// This interface is intended to be implemented by dialog view models.
/// Calling <see cref="CloseDialog(DialogResult)"/> signals that the dialog
/// has finished and provides the result to the dialog service.
///
/// The actual completion mechanism is handled internally via
/// <see cref="IDialogCompletionSource"/> and is typically not implemented directly
/// by consumers. Instead, inheriting from <see cref="DialogViewModelBase"/> is recommended.
/// </remarks>
public interface IDialogController
{
    /// <summary>
    /// Closes the dialog and provides the result to the awaiting dialog service.
    /// </summary>
    /// <param name="result">
    /// The result of the dialog interaction.
    /// </param>
    /// <remarks>
    /// This method should be called when the user completes or cancels the dialog.
    /// Internally, this will complete the dialog's asynchronous result task.
    /// </remarks>
    void CloseDialog(DialogResult result);
}
