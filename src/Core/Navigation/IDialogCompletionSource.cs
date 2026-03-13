namespace ADaxer.MvvmNav.Core.ViewModels;

public interface IDialogCompletionSource
{
    Task<bool> CompletionTask { get; }
    void ResetDialogCompletion();
}
