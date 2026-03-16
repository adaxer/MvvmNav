
using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Core.ViewModels;

public abstract class DialogViewModelBase : ViewModelBase, IDialogAware, IDialogCompletionSource
{
    private TaskCompletionSource<DialogResult>? _completionSource;

    protected DialogViewModelBase()
    {
        Title = GetType().Name;
    }

    Task<DialogResult> IDialogCompletionSource.CompletionTask =>
        _completionSource?.Task ?? Task.FromResult(DialogResult.None);

    void IDialogCompletionSource.ResetDialogCompletion()
    {
        _completionSource = new TaskCompletionSource<DialogResult>();
    }

    public virtual void CloseDialog(DialogResult result)
    {
        _completionSource?.TrySetResult(result);
    }
}
