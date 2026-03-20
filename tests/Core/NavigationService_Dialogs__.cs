using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.DI;
using ADaxer.MvvmNav.Core.Tests.TestData;
using NSubstitute;

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

[ClassConstructor<DIClassConstructor>]
public sealed class NavigationService_Dialogs__
{
    private readonly NavigationService _sut;
    private readonly IShellViewModel _shell;
    private readonly FakeDialogService _dialogService;
    private readonly TestDialog _dialog;
    private readonly TestStringDialogNavigationAware _typedDialog;

    public NavigationService_Dialogs__(
        NavigationService sut,
        IShellViewModel shell,
        IDialogService dialogService,
        TestDialog dialog,
        TestStringDialogNavigationAware typedDialog)
    {
        _sut = sut;
        _shell = shell;
        _dialogService = (FakeDialogService)dialogService;
        _dialog = dialog;
        _typedDialog = typedDialog;
    }

    [Test]
    public async Task ShowDialogAsync_ShouldResolveDialog_AndReturnDialogResult()
    {
        // Arrange
        var context = new NavigationParameters(("Id", 42));

        // Act
        var task = _sut.ShowDialogAsync<TestDialog>(context);

        // noch nicht abgeschlossen → wartet auf Completion
        await Assert.That(task.IsCompleted).IsFalse();

        _dialog.CloseDialog(new DialogResult(true));

        var result = await task;

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(result.IsConfirmed).IsTrue();
            await Assert.That(_dialogService.ShowDialogCallCount).IsEqualTo(1);
            await Assert.That(_dialogService.LastDialog).IsSameReferenceAs(_dialog);
            await Assert.That(_dialogService.LastParameters).IsSameReferenceAs(context);
        }
    }

    [Test]
    public async Task ShowDialogAsync_WithTypedResult_ShouldResolveDialog_AndReturnTypedDialogResult()
    {
        // Arrange
        var context = new NavigationParameters(("Query", "abc"));

        // Act
        var task = _sut.ShowDialogAsync<TestStringDialogNavigationAware, string>(context);

        await Assert.That(task.IsCompleted).IsFalse();

        _typedDialog.CloseDialog(new DialogResult<string>(true, "Hello"));

        var result = await task;

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(result).IsTypeOf<DialogResult<string>>();
            await Assert.That(result.IsConfirmed).IsTrue();
            await Assert.That(result.Value).IsEqualTo("Hello");
        }
    }

    [Test]
    public async Task ShowDialogAsync_WithNullContext_ShouldUseNavigationParametersEmpty()
    {
        // Arrange

        // Act
        var task = _sut.ShowDialogAsync<TestDialog>();

        _dialog.CloseDialog(new DialogResult(true));

        await task;

        // Assert
        await Assert.That(_dialogService.LastParameters)
            .IsSameReferenceAs(NavigationParameters.Empty);
    }

    [Test]
    public async Task ShowDialogAsync_WithDialogMissingController_ShouldThrow()
    {
        // Arrange
        Func<Task> act = () => _sut.ShowDialogAsync<DialogWithoutController>();

        // Act & Assert
        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShowDialogAsync_WithDialogMissingCompletionSource_ShouldThrow()
    {
        // Arrange
        Func<Task> act = () => _sut.ShowDialogAsync<DialogWithoutCompletionSource>();

        // Act & Assert
        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShowDialogAsync_ShouldNotChangeShellCurrentModule()
    {
        // Arrange
        var current = new object();
        _shell.CurrentModule.Returns(current);

        // Act
        var task = _sut.ShowDialogAsync<TestDialog>();

        _dialog.CloseDialog(new DialogResult(true));

        await task;

        // Assert
        await Assert.That(_shell.CurrentModule).IsSameReferenceAs(current);
    }

    [Test]
    public async Task ShowDialogAsync_ShouldNotRaiseNavigationStateChanged()
    {
        // Arrange
        var raised = 0;
        _sut.NavigationStateChanged += (_, _) => raised++;

        // Act
        var task = _sut.ShowDialogAsync<TestDialog>();

        _dialog.CloseDialog(new DialogResult(true));

        await task;

        // Assert
        await Assert.That(raised).IsEqualTo(0);
    }

    [Test]
    public async Task ShowDialogAsync_ShouldAwaitUntilDialogIsClosed()
    {
        // Arrange

        // Act
        var task = _sut.ShowDialogAsync<TestDialog>();

        // noch offen → kein Ergebnis
        await Assert.That(task.IsCompleted).IsFalse();

        // jetzt schließen
        _dialog.CloseDialog(new DialogResult(true));

        var result = await task;

        // Assert
        await Assert.That(result.IsConfirmed).IsTrue();
    }

    [Test]
    public async Task ShowDialogAsync_WithNavigationAwareDialog_ShouldPassParametersToOnNavigatedToAsync()
    {
        // Arrange
        var context = new NavigationParameters(
            ("Id", 42),
            ("Mode", "Edit"));

        // Act
        var task = _sut.ShowDialogAsync<TestStringDialogNavigationAware, string>(context);

        await Assert.That(task.IsCompleted).IsFalse();

        _typedDialog.CloseDialog(new DialogResult<string>(true, "Hello"));

        var result = await task;

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(result.IsConfirmed).IsTrue();
            await Assert.That(result.Value).IsEqualTo("Hello");
            await Assert.That(_typedDialog.LastContext?["Id"]).IsEqualTo(42);
            await Assert.That(_typedDialog.LastContext?["Mode"]).IsEqualTo("Edit");
        }
    }

    [Test]
    public async Task ShowDialogAsync_WithNotNavigationAwareDialog_ShouldStillWork()
    {
        // Arrange
        var context = new NavigationParameters(
            ("Id", 42),
            ("Mode", "Edit"));

        // Act
        var task = _sut.ShowDialogAsync<TestDialog>(context);

        await Assert.That(task.IsCompleted).IsFalse();

        _dialog.CloseDialog(new DialogResult(true));

        var result = await task;

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(result.IsConfirmed).IsTrue();
        }
    }

}
