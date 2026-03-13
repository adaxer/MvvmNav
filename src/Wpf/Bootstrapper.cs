using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Wpf;

/// <summary>
/// Provides helper methods for bootstrapping WPF applications that use MvvmNav.
/// </summary>
/// <remarks>
/// The bootstrapper creates a dependency injection container, registers the
/// core MvvmNav services, registers the shell view and shell view model,
/// and can optionally start the application shell.
/// </remarks>
public static class Bootstrapper
{
    /// <summary>
    /// Builds a service provider for a WPF application that uses MvvmNav.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The concrete shell view type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The concrete shell view model type.
    /// </typeparam>
    /// <param name="configureServices">
    /// An optional callback used to register additional application services.
    /// </param>
    /// <returns>
    /// A fully built <see cref="IServiceProvider"/> instance.
    /// </returns>
    /// <remarks>
    /// This method registers:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Required services via <see cref="ServiceCollectionExtensions.AddMvvmNavWpf(IServiceCollection)"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// The shell view as both <typeparamref name="TShellView"/> and <see cref="IShellView"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// The shell view model as both <typeparamref name="TShellViewModel"/> and <see cref="IShellViewModel"/>.
    /// </description>
    /// </item>
    /// </list>
    /// 
    /// The interface registrations reuse the concrete singleton instances,
    /// so only one shell view instance and one shell view model instance are created.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="configureServices"/> is <c>null</c> when explicitly provided
    /// through an overload that requires it.
    /// </exception>
    public static IServiceProvider Build<TShellView, TShellViewModel>(
        Action<IServiceCollection>? configureServices = null)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        var services = new ServiceCollection();

        services.AddMvvmNavWpf();

        services.AddSingleton<TShellView>();
        services.AddSingleton<IShellView>(sp => sp.GetRequiredService<TShellView>());

        //services.AddSingleton<TShellViewModel>();
        services.AddSingleton<IShellViewModel,TShellViewModel>();

        configureServices?.Invoke(services);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Resolves and displays the application shell.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The concrete shell view type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The concrete shell view model type.
    /// </typeparam>
    /// <param name="services">
    /// The service provider used to resolve the shell view and shell view model.
    /// </param>
    /// <returns>
    /// A tuple containing the resolved shell view and shell view model instances.
    /// </returns>
    /// <remarks>
    /// This method resolves the shell view and shell view model, assigns the
    /// shell view model to the shell view's <see cref="IShellView.DataContext"/>,
    /// and then calls <see cref="IShellView.Show()"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static (TShellView, TShellViewModel) Start<TShellView, TShellViewModel>(IServiceProvider services)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        ArgumentNullException.ThrowIfNull(services);

        var shell = services.GetRequiredService<IShellView>();
        var shellViewModel = services.GetRequiredService<IShellViewModel>();
        ArgumentNullException.ThrowIfNull(shell);
        ArgumentNullException.ThrowIfNull(shellViewModel);

        shell.DataContext = shellViewModel;
        shell.Show();

        return (shell as TShellView, shellViewModel as TShellViewModel)!;
    }

    /// <summary>
    /// Builds the service provider and immediately starts the application shell.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The concrete shell view type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The concrete shell view model type.
    /// </typeparam>
    /// <param name="configureServices">
    /// An optional callback used to register additional application services.
    /// </param>
    /// <returns>
    /// A tuple containing the built <see cref="IServiceProvider"/>, the
    /// resolved shell view instance, and the resolved shell view model instance.
    /// </returns>
    /// <remarks>
    /// This is a convenience method that combines
    /// <see cref="Build{TShellView, TShellViewModel}(Action{IServiceCollection}?)"/>
    /// and
    /// <see cref="Start{TShellView, TShellViewModel}(IServiceProvider)"/>.
    /// </remarks>
    public static (IServiceProvider, TShellView, TShellViewModel) BuildAndStart<TShellView, TShellViewModel>(
        Action<IServiceCollection>? configureServices = null)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        var services = Build<TShellView, TShellViewModel>(configureServices);
        var shell = Start<TShellView, TShellViewModel>(services);

        return (services, shell.Item1, shell.Item2);
    }
}
