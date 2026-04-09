using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Avalonia.Hosting;
using ADaxer.MvvmNav.Avalonia.Navigation;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides dependency injection extensions for the Avalonia integration of MvvmNav.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Avalonia integration of MvvmNav.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddMvvmNav(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMvvmNavCore();
        services.AddSingleton(new AvaloniaMvvmNavOptions());
        services.AddSingleton<IMvvmNavStarter, MvvmNavStarter>();
        services.AddSingleton<IDialogService, AvaloniaDialogService>();

        return services;
    }

    /// <summary>
    /// Configures the shell view and shell view model used by Avalonia MvvmNav startup.
    /// </summary>
    /// <typeparam name="TShellView">The shell view type.</typeparam>
    /// <typeparam name="TShellViewModel">The shell view model type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection WithShell<TShellView, TShellViewModel>(this IServiceCollection services)
        where TShellView : class, IShellView
        where TShellViewModel : class
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = GetRequiredOptions(services);
        options.ShellViewType = typeof(TShellView);
        options.ShellViewModelType = typeof(TShellViewModel);

        services.AddSingleton(typeof(TShellView));
        services.AddSingleton(typeof(TShellViewModel));
        services.AddSingleton(typeof(IShellView), sp => sp.GetRequiredService<TShellView>());

        services.AddLogging(logging =>
        {
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
        });


        return services;
    }

    /// <summary>
    /// Configures the initial navigation target to be navigated to after startup.
    /// </summary>
    /// <typeparam name="TViewModel">The startup target view model type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection WithStartupNavigation<TViewModel>(this IServiceCollection services)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = GetRequiredOptions(services);
        options.StartupNavigationType = typeof(TViewModel);

        return services;
    }

    private static AvaloniaMvvmNavOptions GetRequiredOptions(IServiceCollection services)
    {
        var descriptor = services.LastOrDefault(x => x.ServiceType == typeof(AvaloniaMvvmNavOptions));

        if (descriptor?.ImplementationInstance is AvaloniaMvvmNavOptions options)
            return options;

        throw new InvalidOperationException(
            $"{nameof(AvaloniaMvvmNavOptions)} must be registered as an implementation instance. " +
            $"Call AddMvvmNav() before using the fluent With... methods.");
    }
}
