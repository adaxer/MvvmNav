using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Core.Tests.TestData;

public sealed class FakeDialogService : IDialogService
{
    public int ShowDialogCallCount { get; private set; }
    public object? LastDialog { get; private set; }
    public NavigationParameters? LastParameters { get; private set; }

    public async Task<DialogResult> ShowDialogAsync(
        IDialogController dialog,
        NavigationParameters parameters)
    {
        LastDialog = dialog;
        LastParameters = parameters;
        ShowDialogCallCount++;

        if (dialog is not IDialogCompletionSource completionSource)
        {
            throw new InvalidOperationException(
                $"Dialog '{dialog.GetType().FullName}' must implement {nameof(IDialogCompletionSource)}.");
        }

        completionSource.ResetDialogCompletion();

        return await completionSource.CompletionTask;
    }

    public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(
        IDialogController dialog,
        NavigationParameters parameters)
    {
        LastDialog = dialog;
        LastParameters = parameters;
        ShowDialogCallCount++;

        if (dialog is not IDialogCompletionSource completionSource)
        {
            throw new InvalidOperationException(
                $"Dialog '{dialog.GetType().FullName}' must implement {nameof(IDialogCompletionSource)}.");
        }

        completionSource.ResetDialogCompletion();

        var result = await completionSource.CompletionTask;

        if (result.IsConfirmed != true)
        {
            return new DialogResult<TResult>(false, default);
        }

        if (result is DialogResult<TResult> typedResult)
        {
            return typedResult;
        }

        throw new InvalidOperationException(
            $"Dialog result value cannot be cast to '{typeof(TResult).FullName}'.");
    }

    public Task<DialogResult> ConfirmAsync(object context, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("ConfirmAsync is not used by these dialog tests.");
    }
}
