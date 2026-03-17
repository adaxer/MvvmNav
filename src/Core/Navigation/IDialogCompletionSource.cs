using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Core.ViewModels;

public interface IDialogCompletionSource
{
    Task<DialogResult> CompletionTask { get; }
    void ResetDialogCompletion();
}
