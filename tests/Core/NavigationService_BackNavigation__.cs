using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using ADaxer.MvvmNav.Core.Tests.TestData;
using Microsoft.Extensions.Logging;
using MvvmNav.Core.Tests.TestData;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_BackNavigation__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly IDialogService _dialogService;
    private readonly TestNavigationTargetSequence _targetSequence;

    public NavigationService_BackNavigation__(
        NavigationService sut,
        IShellViewModel shell,
        IDialogService dialogService,
        ILogger<NavigationService> logger,
        TestNavigationTargetSequence targetSequence)
    {
        _sut = sut;
        _shell = shell;
        _dialogService = dialogService;
        _targetSequence = targetSequence;
    }

    [Test]
    public async Task CanGoBack_WithoutPreviousNavigation_ShouldReturnFalse()
    {
        // Arrange

        // Act
        var canGoBack = _sut.CanGoBack();

        // Assert
        await Assert.That(canGoBack).IsFalse();
    }

    [Test]
    public async Task CanGoBack_AfterSecondNavigation_ShouldReturnTrue()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));
        var canGoBack = _sut.CanGoBack();

        // Assert
        await Assert.That(canGoBack).IsTrue();
    }

    [Test]
    public async Task GoBackAsync_AfterNavigatingToSecondTarget_ShouldRestorePreviousTarget()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task GoBackAsync_ShouldRestorePreviousParameters()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        var firstParameters = new NavigationParameters(("Id", 42), ("Mode", "Edit"));
        var secondParameters = new NavigationParameters(("Id", 99));

        await _sut.NavigateAsync(typeof(TestNavigationTarget), firstParameters);
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget), secondParameters);

        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(firstResolved.LastParameters).IsSameReferenceAs(firstParameters);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task GoBackAsync_WhenBackStackBecomesEmpty_ShouldUpdateCanGoBack()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        // Act
        await _sut.GoBackAsync();
        var canGoBack = _sut.CanGoBack();

        // Assert
        await Assert.That(canGoBack).IsFalse();
    }

    [Test]
    public async Task GoBackAsync_WhenCurrentTargetDisallowsNavigation_ShouldNotGoBack()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var guard = new GuardProbe(NavigationGuardDecision.Disallow);

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        _shell.CurrentModule = guard;

        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_sut.CanGoBack()).IsTrue();
            await Assert.That(guard.LastRequest).IsNotNull();
            await Assert.That(guard.LastRequest!.IsBackNavigation).IsTrue();
        }
    }

    [Test]
    public async Task GoBackAsync_WhenCurrentTargetRequestsConfirmation_AndUserCancels_ShouldNotGoBack()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var guard = new GuardProbe(NavigationGuardDecision.AskUser, new NavigationParameters(("Message", "Leave page?")));

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        _shell.CurrentModule = guard;

        ((FakeDialogService)_dialogService).NextConfirmationResult = DialogResult.None;
        
        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_sut.CanGoBack()).IsTrue();
            await Assert.That(guard.LastRequest).IsNotNull();
            await Assert.That(guard.LastRequest!.IsBackNavigation).IsTrue();
        }
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task GoBackAsync_WhenCurrentTargetAsksToSave_AndGetsTrueOrFalse_ShouldGoBack(
    bool confirmSave)
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var guard = new GuardProbe(NavigationGuardDecision.AskUser, new NavigationParameters(("Message", "Leave page?")));

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        _shell.CurrentModule = guard;

        ((FakeDialogService)_dialogService).NextConfirmationResult = confirmSave ? DialogResult.True : DialogResult.False;

        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_sut.CanGoBack()).IsFalse();
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
            await Assert.That(guard.LastRequest).IsNotNull();
            await Assert.That(guard.LastRequest!.IsBackNavigation).IsTrue();
        }
    }

    [Test]
    public async Task GoBackAsync_ShouldPassIsBackNavigationTrueToNavigationGuard()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var guard = new GuardProbe(NavigationGuardDecision.Allow);

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget), new NavigationParameters(("Id", 1)));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget), new NavigationParameters(("Id", 2)));

        _shell.CurrentModule = guard;

        // Act
        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(guard.LastRequest).IsNotNull();
            await Assert.That(guard.LastRequest!.IsBackNavigation).IsTrue();
            await Assert.That(guard.LastRequest.TargetType).IsEqualTo(typeof(TestNavigationTarget));
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }
}

