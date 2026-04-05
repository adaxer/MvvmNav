using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Core.Tests.TestData;

public sealed class GuardProbe : ICanNavigateFrom
{
    private readonly NavigationGuardDecision _decision;
    private readonly NavigationParameters? _context;

    public GuardProbe(
        NavigationGuardDecision decision,
        NavigationParameters? context = null)
    {
        _decision = decision;
        _context = context;
    }

    public NavigationRequest? LastRequest { get; private set; }

    public DialogResult? ContinuedWith { get; private set; }

    public Task<NavigationGuardResult> CanNavigateFromAsync(NavigationRequest request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        Func<DialogResult, CancellationToken, Task>? continuation = (result, _) =>
        {
            ContinuedWith = result;
            return Task.CompletedTask;
        };

        return Task.FromResult(new NavigationGuardResult(_decision, _context, continuation, cancellationToken));
    }
}
