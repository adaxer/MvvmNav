using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Core.Tests.TestData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvvmNav.Core.Tests.TestData;
using NSubstitute;
using TUnit.Core.Interfaces;

namespace ADaxer.MvvmNav.Core.Tests.DI;

public sealed class DIClassConstructor : IClassConstructor
{
    public Task<object> Create(Type type, ClassConstructorMetadata metadata)
    {
        var services = new ServiceCollection();

        // --- fresh substitutes per test class instance
        var shell = Substitute.For<IShellViewModel>();
        var logger = Substitute.For<ILogger<NavigationService>>();

        var targetSequence = new TestNavigationTargetSequence();

        services.AddSingleton(shell);
        services.AddSingleton<IDialogService, FakeDialogService>();
        services.AddSingleton(logger);
        services.AddSingleton(targetSequence);

        // SUT
        services.AddTransient<NavigationService>();

        // navigation targets
        services.AddTransient<TestNavigationTarget>(sp =>
            sp.GetRequiredService<TestNavigationTargetSequence>()
              .GetNextTestNavigationTarget());

        services.AddTransient<AnotherTestNavigationTarget>(sp =>
            sp.GetRequiredService<TestNavigationTargetSequence>()
              .GetNextAnotherTestNavigationTarget());

        // dialog test data
        services.AddSingleton<TestDialog>();
        services.AddSingleton<TestStringDialogNavigationAware>();
        services.AddTransient<DialogWithoutController>();
        services.AddTransient<DialogWithoutCompletionSource>();

        var provider = services.BuildServiceProvider();

        var instance = ActivatorUtilities.CreateInstance(provider, type);

        return Task.FromResult(instance);
    }
}
