using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Maui.Navigation;

public class MauiNavigationService : NavigationService
{
    public MauiNavigationService(
        IServiceProvider services,
        IDialogService dialogService,
        ILogger<NavigationService> logger)
        : base(services, dialogService, logger)
    {
    }
}
