using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Core.Tests.TestData;

public sealed class TestDialog : DialogViewModelBase
{
}

public sealed class TestStringDialogNavigationAware : DialogViewModelBase, INavigationAware
{
    public NavigationParameters? LastContext { get; private set; }

    public Task OnNavigatedToAsync(NavigationParameters context)
    {
        LastContext = context;
        return Task.CompletedTask;
    }
}

public sealed class DialogWithoutController
{
}

public sealed class DialogWithoutCompletionSource : IDialogController
{
    public DialogResult? ClosedWith { get; private set; }

    public void CloseDialog(DialogResult result)
    {
        ClosedWith = result;
    }
}
