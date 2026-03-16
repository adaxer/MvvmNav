using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public class AboutViewModel : DialogViewModelBase
{
    public async Task OnNavigatedToAsync(NavigationParameters context)
    {
    }

    public override void CloseDialog(DialogResult result)
    {
        Title = result.ToString();
    }
}
