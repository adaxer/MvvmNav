using ADaxer.MvvmNav.Sample.Avalonia.iOS.Views;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using Avalonia;
using Avalonia.iOS;
using Foundation;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Sample.Avalonia.iOS;
// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public partial class AppDelegate : AvaloniaAppDelegate<App>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var services = new ServiceCollection();

        services.AddMvvmNav()
            .WithShell<ShellView, ShellViewModel>()
            .WithStartupNavigation<HomeViewModel>()
            .RegisterCommonServices()
            .RegisterAvaloniaSpecificServices()
            .RegisterIOSServices();

        var serviceProvider = services.BuildServiceProvider();

        var result = AppBuilder.Configure<App>(() => new App { ServiceProvider = serviceProvider })
            .UseiOS()
            .WithInterFont()
            .LogToTrace();

        return result;
    }        
}
