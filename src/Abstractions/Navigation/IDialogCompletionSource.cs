namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Provides a completion mechanism for dialogs that allows the <see cref="IDialogService"/>
/// to asynchronously await the result of a dialog interaction.
/// </summary>
/// <remarks>
/// Implementations are expected to internally use a <see cref="TaskCompletionSource{TResult}"/>
/// to signal when the dialog has been closed by the user.
///
/// The <see cref="CompletionTask"/> is awaited by the dialog service after the dialog has been shown.
/// Once the dialog is closed, the implementation must complete the task with a corresponding <see cref="DialogResult"/>.
/// <see cref="ResetDialogCompletion"/> must be called before showing the dialog again to ensure a fresh completion state.
///
/// This interface is implemented by a base class for DialogViewModels. If you want to use your own class, it must implement it on its own.
/// </remarks>
public interface IDialogCompletionSource
{
    /// <summary>
    /// Gets a task that completes when the dialog is closed, providing the resulting <see cref="DialogResult"/>.
    /// </summary>
    Task<DialogResult> CompletionTask { get; }

    /// <summary>
    /// Resets the internal completion mechanism so that the dialog can be shown again.
    /// </summary>
    /// <remarks>
    /// This should recreate the underlying <see cref="TaskCompletionSource{TResult}"/>.
    /// It is typically called by the dialog service before displaying the dialog.
    /// </remarks>
    void ResetDialogCompletion();
}
