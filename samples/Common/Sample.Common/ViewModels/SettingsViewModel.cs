using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Sample.Common.ViewModels;

public class SettingsViewModel : INavigationAware, ICanNavigateFrom
{
    public string Title => GetType().Name;

    public Task<NavigationGuardResult> CanNavigateFromAsync(NavigationRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NavigationGuardResult.AskUser("Are you sure that you are sure"));
    }

    public Task OnNavigatedToAsync(NavigationParameters context)
    {
        return Task.CompletedTask;
    }
}
