
using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Core.ViewModels;

public abstract class DialogViewModelBase : ViewModelBase, IDialogAware, IDialogCompletionSource
{
    private TaskCompletionSource<bool>? _completionSource;

    protected DialogViewModelBase()
    {
        Title = GetType().Name;
    }

    Task<bool> IDialogCompletionSource.CompletionTask =>
        _completionSource?.Task ?? Task.FromResult(false);

    void IDialogCompletionSource.ResetDialogCompletion()
    {
        _completionSource = new TaskCompletionSource<bool>();
    }

    public void CloseDialog(bool result)
    {
        _completionSource?.TrySetResult(result);
    }
}
