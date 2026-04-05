using ADaxer.MvvmNav.Abstractions.Dialogs;

namespace ADaxer.MvvmNav.Core.ViewModels;

/// <summary>
/// Provides a convenience base class for dialog view models.
/// </summary>
/// <remarks>
/// This base class combines the common view model functionality from
/// <see cref="ViewModelBase"/> with the dialog contracts required by
/// <see cref="IDialogController"/> and <see cref="IDialogCompletionSource"/>.
/// </remarks>
public abstract class DialogViewModelBase : ViewModelBase, IDialogController, IDialogCompletionSource
{
    private TaskCompletionSource<DialogResult>? _completionSource;

    /// <summary>
    /// Gets the task that completes when the dialog closes.
    /// </summary>
    /// <remarks>
    /// When the dialog has not been prepared yet, a completed task with
    /// <see cref="DialogResult.None"/> is returned.
    /// </remarks>
    protected Task<DialogResult> CompletionTask =>
        _completionSource?.Task ?? Task.FromResult(DialogResult.None);

    Task<DialogResult> IDialogCompletionSource.CompletionTask => CompletionTask;

    /// <summary>
    /// Resets the internal completion source so the dialog can be shown again.
    /// </summary>
    void IDialogCompletionSource.ResetDialogCompletion()
    {
        _completionSource = new TaskCompletionSource<DialogResult>();
    }

    /// <summary>
    /// Closes the dialog and completes the pending dialog result.
    /// </summary>
    /// <param name="result">
    /// The dialog result.
    /// </param>
    public virtual void CloseDialog(DialogResult result)
    {
        _completionSource?.TrySetResult(result);
    }
}
