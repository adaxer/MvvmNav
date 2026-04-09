
using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Maui;
using ADaxer.MvvmNav.Maui.Hosting;
using ADaxer.MvvmNav.Maui.Navigation;
using ADaxer.MvvmNav.Maui.Views;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the MvvmNav services
/// required for MAUI applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the MvvmNav navigation services for MAUI applications.
    /// </summary>
    public static IServiceCollection AddMvvmNav(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var locator = new ViewLocator();
        ViewLocator.Current = locator;

        services.AddSingleton<IViewLocator>(locator);

        services.AddMvvmNavCore();
        services.AddSingleton<IDialogService, MauiDialogService>();
        services.AddSingleton<INavigationService, MauiNavigationService>();
        services.RegisterView<MessageViewModel, MessageView>();
        services.RegisterDialog<DialogViewModelBase, MauiDialog>();
        services.AddSingleton(new StartupOptions());
        services.AddSingleton<IMauiMvvmNavStarter, MauiMvvmNavStarter>();

        return services;
    }

    /// <summary>
    /// Registers a normal content view for a view model.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The view model type.
    /// </typeparam>
    /// <typeparam name="TView">
    /// The corresponding MAUI view type.
    /// </typeparam>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection RegisterView<TViewModel, TView>(this IServiceCollection services)
        where TViewModel : class
        where TView : View
    {
        ViewLocator.Current.RegisterView(typeof(TViewModel), typeof(TView));
        return services;
    }

    /// <summary>
    /// Registers a dialog host view for a dialog view model or base type.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The dialog view model type or base type.
    /// </typeparam>
    /// <typeparam name="TView">
    /// The corresponding MAUI dialog host view type.
    /// </typeparam>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection RegisterDialog<TViewModel, TView>(this IServiceCollection services)
        where TViewModel : class
        where TView : View
    {
        ViewLocator.Current.RegisterDialog(typeof(TViewModel), typeof(TView));
        return services;
    }

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

        return services;
    }

    public static IServiceCollection WithStartupNavigation<TViewModel>(this IServiceCollection services)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = GetRequiredOptions(services);
        options.StartupNavigationType = typeof(TViewModel);

        return services;
    }

    private static StartupOptions GetRequiredOptions(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var descriptor = services.LastOrDefault(x => x.ServiceType == typeof(StartupOptions));

        if (descriptor?.ImplementationInstance is StartupOptions options)
            return options;

        throw new InvalidOperationException(
            $"{nameof(StartupOptions)} must be registered as an implementation instance. " +
            $"Call AddMvvmNavMaui() before using the fluent With... methods.");
    }
}
