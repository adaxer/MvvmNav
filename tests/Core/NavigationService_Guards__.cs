using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using ADaxer.MvvmNav.Core.Tests.TestData;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_Guards__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly IDialogService _dialogService;
    private readonly TestNavigationTargetSequence _targetSequence;

    public NavigationService_Guards__(
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
    public async Task NavigateAsync_WhenCurrentTargetAllowsNavigation_ShouldNavigate()
    {
        // Arrange
        var currentTarget = new GuardProbe(NavigationGuardDecision.Allow);
        var nextResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(nextResolved);

        _shell.CurrentModule = currentTarget;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(currentTarget.LastRequest).IsNotNull();
            await Assert.That(currentTarget.LastRequest!.IsBackNavigation).IsFalse();
            await Assert.That(currentTarget.LastRequest.TargetType).IsEqualTo(typeof(TestNavigationTarget));
            await Assert.That(nextResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(nextResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_WhenCurrentTargetDisallowsNavigation_ShouldNotNavigate()
    {
        // Arrange
        var currentTarget = new GuardProbe(NavigationGuardDecision.Disallow);
        var nextResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(nextResolved);

        _shell.CurrentModule = currentTarget;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(currentTarget.LastRequest).IsNotNull();
            await Assert.That(currentTarget.LastRequest!.IsBackNavigation).IsFalse();
            await Assert.That(nextResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(currentTarget);
            await Assert.That(_sut.CanGoBack()).IsFalse();
        }
    }

    [Test]
    public async Task NavigateAsync_WhenCurrentTargetRequestsConfirmation_AndUserCancels_ShouldNotNavigate()
    {
        // Arrange
        var currentTarget = new GuardProbe(
            NavigationGuardDecision.AskUser,
            new NavigationParameters(("Message", "Save changes?")));

        var nextResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(nextResolved);

        _shell.CurrentModule = currentTarget;
        ((FakeDialogService)_dialogService).NextConfirmationResult = DialogResult.None;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(currentTarget.LastRequest).IsNotNull();
            await Assert.That(currentTarget.LastRequest!.IsBackNavigation).IsFalse();
            await Assert.That(nextResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(currentTarget);
            await Assert.That(((FakeDialogService)_dialogService).ConfirmCallCount).IsEqualTo(1);
            await Assert.That(_sut.CanGoBack()).IsFalse();
        }
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task NavigateAsync_WhenCurrentTargetAsksToSave_AndGetsTrueOrFalse_ShouldNavigate(
        bool confirmSave)
    {
        // Arrange
        var currentTarget = new GuardProbe(
            NavigationGuardDecision.AskUser,
            new NavigationParameters(("Message", "Save changes?")));

        var nextResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(nextResolved);

        _shell.CurrentModule = currentTarget;
        ((FakeDialogService)_dialogService).NextConfirmationResult =
            confirmSave ? DialogResult.True : DialogResult.False;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(currentTarget.LastRequest).IsNotNull();
            await Assert.That(currentTarget.LastRequest!.IsBackNavigation).IsFalse();
            await Assert.That(nextResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(nextResolved);
            await Assert.That(((FakeDialogService)_dialogService).ConfirmCallCount).IsEqualTo(1);
        }
    }

    [Test]
    public async Task NavigateAsync_WhenGuardRequestsConfirmationWithoutContext_ShouldThrow()
    {
        // Arrange
        var currentTarget = new GuardProbe(NavigationGuardDecision.AskUser);
        _shell.CurrentModule = currentTarget;

        // Act
        var action = () => _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        await Assert.That(action).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task NavigateAsync_WhenNavigationIsBlocked_ShouldNotRaiseNavigationStateChanged()
    {
        // Arrange
        var currentTarget = new GuardProbe(NavigationGuardDecision.Disallow);
        var eventCallCount = 0;

        _shell.CurrentModule = currentTarget;
        _sut.NavigationStateChanged += (_, _) => eventCallCount++;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        await Assert.That(eventCallCount).IsEqualTo(0);
    }

    [Test]
    public async Task NavigateAsync_WhenNavigationIsBlocked_ShouldNotPushCurrentTargetToBackStack()
    {
        // Arrange
        var currentTarget = new GuardProbe(NavigationGuardDecision.Disallow);
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        _shell.CurrentModule = currentTarget;

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(currentTarget);
            await Assert.That(_sut.CanGoBack()).IsFalse();
        }
    }
}
