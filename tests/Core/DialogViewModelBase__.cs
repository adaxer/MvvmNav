using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Tests.TestData;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Core.Tests.ViewModels;

public class DialogViewModelBase__
{
    [Test]
    public async Task CompletionTask_WithoutReset_ShouldReturnNone()
    {
        // Arrange
        var sut = new TestDialog();
        var completionSource = (IDialogCompletionSource)sut;

        // Act
        var result = await completionSource.CompletionTask;

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.IsConfirmed).IsEqualTo(DialogResult.None.IsConfirmed);
    }

    [Test]
    public async Task ResetDialogCompletion_ShouldCreateFreshCompletionTask()
    {
        // Arrange
        var sut = new TestDialog();
        var completionSource = (IDialogCompletionSource)sut;

        // Act
        completionSource.ResetDialogCompletion();
        var firstTask = completionSource.CompletionTask;

        completionSource.ResetDialogCompletion();
        var secondTask = completionSource.CompletionTask;

        // Assert
        await Assert.That(ReferenceEquals(firstTask, secondTask)).IsFalse();
        await Assert.That(firstTask.IsCompleted).IsFalse();
        await Assert.That(secondTask.IsCompleted).IsFalse();
    }

    [Test]
    public async Task CloseDialog_ShouldCompleteCompletionTask_WithResult()
    {
        // Arrange
        var sut = new TestDialog();
        var completionSource = (IDialogCompletionSource)sut;
        completionSource.ResetDialogCompletion();

        // Act
        sut.CloseDialog(DialogResult.True);
        var result = await completionSource.CompletionTask;

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.IsConfirmed).IsEqualTo(DialogResult.True.IsConfirmed);
    }

    [Test]
    public async Task CloseDialog_WithoutReset_ShouldNotThrow()
    {
        // Arrange
        var sut = new TestDialog();
        var completionSource = (IDialogCompletionSource)sut;

        // Act
        var action = () => sut.CloseDialog(DialogResult.True);

        // Assert
        await Assert.That(action).ThrowsNothing();

        var result = await completionSource.CompletionTask;
        await Assert.That(result.IsConfirmed).IsEqualTo(DialogResult.None.IsConfirmed);
    }
}
