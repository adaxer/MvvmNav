using ADaxer.MvvmNav.Abstractions.Navigation;

namespace MvvmNav.Core.Tests.TestData;

public sealed class TestNavigationTarget : INavigationAware
{
    public int OnNavigatedToCallCount { get; private set; }

    public NavigationParameters? LastParameters { get; private set; }

    public Task OnNavigatedToAsync(NavigationParameters parameters)
    {
        OnNavigatedToCallCount++;
        LastParameters = parameters;
        return Task.CompletedTask;
    }
}

public sealed class AnotherTestNavigationTarget : INavigationAware
{
    public int OnNavigatedToCallCount { get; private set; }

    public NavigationParameters? LastParameters { get; private set; }

    public Task OnNavigatedToAsync(NavigationParameters parameters)
    {
        OnNavigatedToCallCount++;
        LastParameters = parameters;
        return Task.CompletedTask;
    }
}
public sealed class TestNavigationTargetSequence
{
    private readonly Queue<TestNavigationTarget> _testTargets = new();
    private readonly Queue<AnotherTestNavigationTarget> _otherTargets = new();

    public void Enqueue(params TestNavigationTarget[] targets)
    {
        foreach (var target in targets)
        {
            _testTargets.Enqueue(target);
        }
    }

    public void Enqueue(params AnotherTestNavigationTarget[] targets)
    {
        foreach (var target in targets)
        {
            _otherTargets.Enqueue(target);
        }
    }

    public TestNavigationTarget GetNextTestNavigationTarget()
    {
        return _testTargets.Count > 0
            ? _testTargets.Dequeue()
            : new TestNavigationTarget();
    }

    public AnotherTestNavigationTarget GetNextAnotherTestNavigationTarget()
    {
        return _otherTargets.Count > 0
            ? _otherTargets.Dequeue()
            : new AnotherTestNavigationTarget();
    }
}
