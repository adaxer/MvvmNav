
using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;
using ADaxer.MvvmNav.Maui;
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
    public static IServiceCollection AddMvvmNavMaui(this IServiceCollection services)
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

        // 👉 Resources registrieren
        if (Application.Current is not null)
        {
            var resources = Application.Current.Resources;

            // vermeiden von Doppel-Registrierung
            if (!resources.MergedDictionaries.OfType<MauiResources>().Any())
            {
                resources.MergedDictionaries.Add(new MauiResources());
            }
        }

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
}
