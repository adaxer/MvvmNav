using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using Microsoft.Extensions.Logging;
using MvvmNav.Core.Tests.TestData;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_Parameters__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly IDialogService _dialogService;
    private readonly ILogger<NavigationService> _logger;
    private readonly TestNavigationTargetSequence _targetSequence;

    public NavigationService_Parameters__(
        NavigationService sut,
        IShellViewModel shell,
        IDialogService dialogService,
        ILogger<NavigationService> logger,
        TestNavigationTargetSequence targetSequence)
    {
        _sut = sut;
        _shell = shell;
        _dialogService = dialogService;
        _logger = logger;
        _targetSequence = targetSequence;
    }

    [Test]
    public async Task NavigateAsync_SameType_AndSameParametersInDifferentOrder_ShouldBlockSecondNavigation()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(firstResolved, secondResolved);

        var parameters1 = new NavigationParameters(("b", 2), ("a", 1));
        var parameters2 = new NavigationParameters(("a", 1), ("b", 2));

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget), parameters1);
        await _sut.NavigateAsync(typeof(TestNavigationTarget), parameters2);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_SameType_ButDifferentParameters_ShouldAllowSecondNavigation()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(firstResolved, secondResolved);

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget),
            new NavigationParameters(("Id", 10)));

        await _sut.NavigateAsync(typeof(TestNavigationTarget),
            new NavigationParameters(("Id", 11)));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(secondResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_SameType_WithoutParameters_ShouldBlockSecondNavigation()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(firstResolved, secondResolved);

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget));
        await _sut.NavigateAsync(typeof(TestNavigationTarget));

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_CustomNavigationKey_ShouldOverrideDifferentParameters_AndBlockSecondNavigation()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new TestNavigationTarget();
        _targetSequence.Enqueue(firstResolved, secondResolved);

        var options = new NavigationOptions
        {
            NavigationKey = "detail-fixed-key"
        };

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget),
            new NavigationParameters(("Id", 10)), options);

        await _sut.NavigateAsync(typeof(TestNavigationTarget),
            new NavigationParameters(("Id", 11)), options);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(0);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(firstResolved);
        }
    }

    [Test]
    public async Task NavigateAsync_DifferentType_WithSameParameters_ShouldAllowNavigation()
    {
        // Arrange
        var firstResolved = new TestNavigationTarget();
        var secondResolved = new AnotherTestNavigationTarget();
        _targetSequence.Enqueue(firstResolved);
        _targetSequence.Enqueue(secondResolved);

        var parameters = new NavigationParameters(("Id", 10));

        // Act
        await _sut.NavigateAsync(typeof(TestNavigationTarget), parameters);
        await _sut.NavigateAsync(typeof(AnotherTestNavigationTarget), parameters);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(firstResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(secondResolved.OnNavigatedToCallCount).IsEqualTo(1);
            await Assert.That(_shell.CurrentModule).IsSameReferenceAs(secondResolved);
        }
    }
}
