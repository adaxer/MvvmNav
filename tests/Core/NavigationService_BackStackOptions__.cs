using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using MvvmNav.Core.Tests.TestData;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_BackStackOptions__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly TestNavigationTargetSequence _targetSequence;

    public NavigationService_BackStackOptions__(
        NavigationService sut,
        IShellViewModel shell,
        IDialogService dialogService,
        TestNavigationTargetSequence targetSequence)
    {
        _sut = sut;
        _shell = shell;
        _targetSequence = targetSequence;
    }

    [Test]
    public async Task NavigateAsync_WithAddToBackStackFalse_ShouldNotAddCurrentTargetToBackStack()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Act
        await _sut.NavigateAsync(
            typeof(AnotherTestNavigationTarget),
            options: new NavigationOptions
            {
                AddToBackStack = false
            });

        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(secondResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_WithAddToBackStackFalse_ShouldMakeGoBackUnavailable()
    {
        // Arrange
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Act
        await _sut.NavigateAsync(
            typeof(AnotherTestNavigationTarget),
            options: new NavigationOptions
            {
                AddToBackStack = false
            });

        var canGoBack = _sut.CanGoBack();

        // Assert
        await Assert.That(canGoBack).IsFalse();
    }

    [Test]
    public async Task NavigateAsync_WithClearBackStackTrue_ShouldClearExistingBackStack()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var thirdResolved = new TestNavigationTarget();

        _targetSequence.Enqueue(firstResolved, thirdResolved);
        _targetSequence.Enqueue(secondResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));

        // Act
        await _sut.NavigateAsync(
            typeof(TestNavigationTarget),
            options: new NavigationOptions
            {
                ClearBackStack = true
            });

        var canGoBackBefore = _sut.CanGoBack();

        await _sut.GoBackAsync();

        var canGoBackAfter = _sut.CanGoBack();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(canGoBackBefore).IsTrue();
            await Assert.That(thirdResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(secondResolved);
            await Assert.That(canGoBackAfter).IsFalse();
        }
    }

    [Test]
    public async Task NavigateAsync_WithClearBackStackTrue_AndThenNavigate_ShouldLeaveOnlyExpectedBackState()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var thirdResolved = new TestNavigationTarget();
        var fourthResolved = new AnotherTestNavigationTarget();

        _targetSequence.Enqueue(firstResolved, thirdResolved);
        _targetSequence.Enqueue(secondResolved, fourthResolved);

        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));
        await _sut.NavigateAsync(
            typeof(TestNavigationTarget),
            options: new NavigationOptions
            {
                ClearBackStack = true
            });

        // Act
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget));
        await _sut.GoBackAsync();

        var canGoBackBefore = _sut.CanGoBack();
        await _sut.GoBackAsync();


        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(thirdResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(fourthResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(secondResolved);
            await Assert.That(canGoBackBefore).IsTrue();
            await Assert.That(_sut.CanGoBack()).IsFalse();
        }
    }

    [Test]
    public async Task NavigateAsync_WithExplicitNavigationKey_ShouldUseItForBackStackIdentity()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new TestNavigationTarget();

        _targetSequence.Enqueue(firstResolved, secondResolved);

        await _sut.NavigateAsync(
            typeof(TestNavigationTarget),
            options: new NavigationOptions
            {
                NavigationKey = "first-key"
            });

        // Act
        await _sut.NavigateAsync(
            typeof(TestNavigationTarget),
            options: new NavigationOptions
            {
                NavigationKey = "second-key"
            });

        await _sut.GoBackAsync();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(2);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }
}
