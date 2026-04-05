using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using ADaxer.MvvmNav.Core.Tests.TestData;
using MvvmNav.Core.Tests.TestData;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_Events__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly IDialogService _dialogService;
    private readonly TestNavigationTargetSequence _targetSequence;

    public NavigationService_Events__(
        NavigationService sut,
        IShellViewModel shell,
        IDialogService dialogService,
        TestNavigationTargetSequence targetSequence)
    {
        _sut = sut;
        _shell = shell;
        _dialogService = dialogService;
        _targetSequence = targetSequence;
    }

    [Test]
    public async Task NavigateAsync_AfterSuccessfulNavigation_ShouldRaiseNavigationStateChanged()
    {
        // Arrange
        var resolvedTarget = new TestNavigationTarget();
        var eventCallCount = 0;
        _targetSequence.Enqueue(resolvedTarget);

        _sut.NavigationStateChanged += (_, _) => eventCallCount++;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(eventCallCount).IsEqualTo(1);
            await Assert.That(resolvedTarget.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(resolvedTarget);
        }
    }

    [Test]
    public async Task GoBackAsync_AfterSuccessfulBackNavigation_ShouldRaiseNavigationStateChanged()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var eventCallCount = 0;

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        _sut.NavigationStateChanged += (_, _) => eventCallCount++;

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(eventCallCount).IsEqualTo(3);
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_WhenNavigatingToSameTargetAndSameKey_ShouldNotRaiseNavigationStateChanged()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new TestNavigationTarget();
        var eventCallCount = 0;

        _targetSequence.Enqueue(firstResolved, secondResolved);

        _sut.NavigationStateChanged += (_, _) => eventCallCount++;

        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(eventCallCount).IsEqualTo(1);
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task ShowDialogAsync_ShouldNotRaiseNavigationStateChanged()
    {
        // Arrange
        var eventCallCount = 0;
        _sut.NavigationStateChanged += (_, _) => eventCallCount++;

        // Act
        var showDialogTask = _sut.ShowDialogAsync<TestDialog>();

        var dialog = (TestDialog)((FakeDialogService)_dialogService).LastDialog!;
        dialog.CloseDialog(DialogResult.True);

        await showDialogTask;

        // Assert
        await Assert.That(eventCallCount).IsEqualTo(0);
    }
}
