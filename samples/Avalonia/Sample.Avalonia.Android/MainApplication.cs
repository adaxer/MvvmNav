using ADaxer.MvvmNav.Sample.Avalonia.Android.Views;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using Android.Runtime;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Avalonia.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Sample.Avalonia.Android;

[Application]
public class MainApplication : AvaloniaAndroidApplication<App>
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var services = new ServiceCollection();

        services.AddMvvmNav()
            .WithShell<ShellView, ShellViewModel>()
            .WithStartupNavigation<HomeViewModel>()
            .RegisterCommonServices()
            .RegisterAvaloniaSpecificServices()
            .RegisterAndroidServices();

        var serviceProvider = services.BuildServiceProvider();

        var result = AppBuilder.Configure<App>(() => new App { ServiceProvider = serviceProvider })
            .UseAndroid()
            .WithInterFont()
            .WithDeveloperTools()
            .LogToTrace();

        return result;
    }
}
