using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using ADaxer.MvvmNav.Core.Tests.TestData;
using MvvmNav.Core.Tests.TestData;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_Activation__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly TestNavigationTargetSequence _targetSequence;

    public NavigationService_Activation__(
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
    public async Task NavigateAsync_ShouldSetShellCurrentModuleToResolvedTarget()
    {
        // Arrange
        var resolvedTarget = new TestNavigationTarget();
        _targetSequence.Enqueue(resolvedTarget);

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        await Assert.That(_shell.CurrentModule).IsSameReferenceAs(resolvedTarget);
    }

    [Test]
    public async Task NavigateAsync_WithNavigationAwareTarget_ShouldPassParametersToOnNavigatedToAsync()
    {
        // Arrange
        var resolvedTarget = new TestNavigationTarget();
        var parameters = new NavigationParameters(("Id", 42), ("Mode", "Edit"));
        _targetSequence.Enqueue(resolvedTarget);

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget), parameters);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(resolvedTarget.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(resolvedTarget.LastParameters).IsSameReferenceAs(parameters);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(resolvedTarget);
        }
    }

    [Test]
    public async Task NavigateAsync_WithTargetNotImplementingNavigationAware_ShouldStillActivate()
    {
        // Arrange

        // Act
        await _sut.NavigateAsync(typeof(DialogWithoutController));

        // Assert
        await Assert.That(_shell.CurrentModule).IsTypeOf<DialogWithoutController>();
    }

    [Test]
    public async Task GoBackAsync_WithNavigationAwareTarget_ShouldPassRestoredParametersToOnNavigatedToAsync()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        var firstParameters = new NavigationParameters(("Id", 42), ("Mode", "Edit"));
        var secondParameters = new NavigationParameters(("Id", 99));

        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

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
}
