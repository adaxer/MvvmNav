using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Core.Navigation;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Maui.Navigation;

/// <summary>
/// MAUI-specific navigation service that currently reuses the shared core navigation behavior.
/// </summary>
public class MauiNavigationService : NavigationService
{
    /// <summary>
    /// Initializes the service with the dependencies used by the shared navigation pipeline.
    /// </summary>
    /// <param name="services">
    /// The application service provider.
    /// </param>
    /// <param name="dialogService">
    /// The dialog service used for dialog requests and guard confirmation flows.
    /// </param>
    /// <param name="logger">
    /// The logger used by the base navigation service.
    /// </param>
    public MauiNavigationService(
        IServiceProvider services,
        IDialogService dialogService,
        ILogger<NavigationService> logger)
        : base(services, dialogService, logger)
    {
    }
}
