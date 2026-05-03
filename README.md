# ADaxer.MvvmNav

[![Downloads](https://img.shields.io/nuget/dt/ADaxer.MvvmNav.Core?label=downloads&color=green)](https://www.nuget.org/packages/ADaxer.MvvmNav.Core)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue)](LICENSE)

Supports: **WPF [![NuGet](https://img.shields.io/nuget/v/ADaxer.MvvmNav.Wpf?label=nuget&color=blue)](https://www.nuget.org/packages/ADaxer.MvvmNav.Wpf)
· Avalonia [![NuGet](https://img.shields.io/nuget/v/ADaxer.MvvmNav.Avalonia?label=nuget&color=blue)](https://www.nuget.org/packages/ADaxer.MvvmNav.Avalonia)
· MAUI [![NuGet](https://img.shields.io/nuget/v/ADaxer.MvvmNav.Maui?label=nuget&color=blue)](https://www.nuget.org/packages/ADaxer.MvvmNav.Maui)**

A lightweight, ViewModel-first navigation framework for .NET UI
applications.

Supports WPF, Avalonia and MAUI with a consistent mental model and
minimal setup.

------------------------------------------------------------------------

## ✨ Why MvvmNav?

-   ViewModel-first navigation
-   Clean separation of concerns
-   Minimal infrastructure (no heavy frameworks)
-   Cross-platform core
-   Fully DI and logging compatible
-   Works with existing applications

------------------------------------------------------------------------

## 🧠 Design Goals

-   Small and understandable
-   Platform-native integration
-   No overengineering
-   Incremental evolution

------------------------------------------------------------------------

## 🧭 Core Concepts

-   Navigation happens between **ViewModels**
-   Views are resolved by the platform
-   Navigation is handled by a central `INavigationService`
-   Dialogs are handled by `IDialogService`
-   Optional base classes, interface-first design

------------------------------------------------------------------------

## 🚀 Quick Start

Each platform provides a simple entry point:

``` csharp
services.AddMvvmNav(...)
```

> This will add all necessary classes like the platform specific Navigation Service, Dialog Service, Startup helpers (for Avalonia and Maui) into the DI container

- Mark your main ViewModel class with IShellViewModel and deliver properties for CurrentModule (and if you want dialogs, CurrentDialog), 
- Mark your main Window/Page with the IShellView marker interface. 
- Inject INavigationService into your viewmodels and you are good to go. 

Here comes what this looks like in the different platforms.

------------------------------------------------------------------------

# 🖥️ WPF

## Setup

``` csharp
// In App.cs OnStartup
WpfNavigationHostBuilder
    .Default()
    .WithServices(services =>
    {
        services.RegisterMyStuff();
    })
    .WithLogging( <your logging configuration>)
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>()
    .WithDialogMode(<in Wpf Dialogs can be embedded or modal top level windows>)
    .Build();

await host.StartAsync();
```



## Navigate

``` csharp
await navigation.NavigateAsync<HomeViewModel>();
```

## Dialog

``` csharp
var result = await navigation.ShowDialogAsync<AboutViewModel>();

if (result == DialogResult.True)
{
    // handle result
}
```

------------------------------------------------------------------------

# 🧩 Avalonia

## Setup

``` csharp
// In Program.cs (Desktop)
// Or in MainApplication.CustomizeAppBuilder() (Android)
public static AppBuilder BuildAvaloniaApp()
{
    var services = new ServiceCollection();

    services.AddMvvmNav()
        .WithShell<ShellWindow, ShellViewModel>()
        .WithStartupNavigation<HomeViewModel>()
        .RegisterMyStuff();

    var serviceProvider = services.BuildServiceProvider();

    var result = AppBuilder.Configure<App>(()=>new App { ServiceProvider = serviceProvider })
        .UsePlatformDetect()
        .WithInterFont()
        .WithDeveloperTools()
        .LogToTrace();

    return result;
}

// In App.cs
public override async void OnFrameworkInitializationCompleted()
{
    var starter = Services.GetRequiredService<IMvvmNavStarter>();

    starter.Initialize(this);
    await starter.StartAsync();
}
```

## Navigate

``` csharp
await navigation.NavigateAsync<HomeViewModel>();
```

## Dialog

``` csharp
var result = await navigation.ShowDialogAsync<AboutViewModel>();
```

------------------------------------------------------------------------

# 📱 MAUI

## Setup

``` csharp
// In MauiProgram
builder.Services
    .AddMvvmNav()
    .WithShell<ShellPage, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>()
    .RegisterMyStuff();


// In App.cs
public partial class App : Application
{
    private readonly IMvvmNavStarter _starter;

    public App(IMvvmNavStarter starter)
    {
        InitializeComponent();
        _starter = starter;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => _starter.CreateWindow();

    protected override async void OnStart()
        => await _starter.StartAsync();
}
```

## Navigate

``` csharp
await navigation.NavigateAsync<HomeViewModel>();
```

## Dialog

``` csharp
var result = await navigation.ShowDialogAsync<AboutViewModel>();
```

------------------------------------------------------------------------

## 🔄 Navigation with Parameters

``` csharp
await navigation.NavigateAsync<DetailsViewModel>(
    ("Id", 42),
    ("Mode", "Edit"));
```

------------------------------------------------------------------------

## 🎯 Initialize Navigation Target

ViewModels can react to navigation by implementing `INavigationAware`.

This is typically used to initialize state based on navigation parameters.

### Example

```csharp
public class DetailsViewModel : INavigationAware
{
    public int Id { get; private set; }

    public Task OnNavigatedToAsync(NavigationParameters parameters)
    {
        Id = parameters.GetValueOrDefault<int>("Id");

        // load data, initialize state, etc.
        return Task.CompletedTask;
    }
}
```

------------------------------------------------------------------------

## 🛑 Navigation Guards

ViewModels can prevent or delay navigation by implementing `ICanNavigateFrom`.

> This is typically used when a ViewModel has unsaved changes. It can allow navigation, block it, or ask the user for confirmation before leaving the current page.

### Example

```csharp
public class SettingsViewModel : ICanNavigateFrom
{
    public bool IsDirty { get; private set; }
    public string State { get; private set; } = string.Empty;
    public bool? IsChecked { get; private set; }

    public Task<NavigationGuardResult> CanNavigateFromAsync(
        NavigationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = IsDirty
           ? NavigationGuardResult.AskUser(
               "There are changes. Do you want to keep them? \nHit Cancel to stay on this page.",
               OnConfirmedOrNotAsync)
           : NavigationGuardResult.Allow();

        return Task.FromResult(result);
    }

    private async Task OnConfirmedOrNotAsync(DialogResult result, CancellationToken token)
    {
        Trace.TraceInformation(
            "Confirmation Dialog said: {0}",
            result.IsConfirmed.HasValue ? result.IsConfirmed : "None");

        if (result.IsConfirmed == false)
        {
            State = string.Empty;
            IsChecked = null;
        }

        if (result.IsConfirmed.HasValue)
        {
            IsDirty = false;
        }
    }
}
```

`Allow()` proceeds with navigation immediately. `Disallow()` keeps the user on the current ViewModel. `AskUser(...)` shows a confirmation dialog and continues based on the returned `DialogResult`.

------------------------------------------------------------------------

## 📦 Packages

-   ADaxer.MvvmNav.Core
-   ADaxer.MvvmNav.Wpf
-   ADaxer.MvvmNav.Avalonia
-   ADaxer.MvvmNav.Maui
## 📸 Sample Application

The repository contains a cross-platform sample application demonstrating navigation, dialogs, and guards in real-world scenarios.

---

### 🖥️ WPF

<p align="center">
  <img src="assets/images/Sample.Wpf.jpg" width="700"/>
</p>

> Desktop sample using WPF with DataTemplate-based view resolution and fluent host builder setup.

---

### 🧩 Avalonia

<p align="center">
  <img src="assets/images/Sample.Avalonia.Win.jpg" width="30%"/>
  <img src="assets/images/Sample.Avalonia.Linux.jpg" width="30%"/>
  <img src="assets/images/Sample.Avalonia.MacOS.jpg" width="30%"/>
</p>

<p align="center">
  <img src="assets/images/Sample.Avalonia.Android.jpg" width="30%"/>
  <img src="assets/images/Sample.Avalonia.iOS.jpg" width="30%"/>
</p>

> Cross-platform UI with Avalonia, sharing the same ViewModel logic across Desktop and Mobile.

---

### 📱 MAUI

<p align="center">
  <img src="assets/images/Sample.Maui.Win.jpg" width="30%"/>
  <img src="assets/images/Sample.Maui.Android.jpg" width="30%"/>
</p>

> Native cross-platform application using .NET MAUI with a ViewModel-first navigation approach and shell-based dialog overlays.

------------------------------------------------------------------------

## 🧪 Sample App Features

-   Navigation & back stack
-   Dialog handling
-   Navigation guards (save/discard)
-   Markdown-based help pages
-   Cross-platform shell

------------------------------------------------------------------------

## 🛠️ Status

Actively developed.

API is stabilizing toward v1.

------------------------------------------------------------------------

## 🎯 Roadmap

### Current Featureset

-   **ViewModel-first navigation**\
    Navigate between ViewModels without coupling them to concrete views.

-   **Platform-agnostic core**\
    The navigation engine resides in a UI-independent Core library.

-   **Native / platform-appropriate view resolution**\
    Views are resolved using the platform's natural mechanism (e.g. WPF
    `DataTemplate`, MAUI registered view locator).

-   **Navigation parameters**\
    Pass parameters when navigating between ViewModels.

-   **Back navigation with stack management**\
    Built-in back stack with support for clearing or suppressing
    entries.

-   **Navigation guards**\
    ViewModels can intercept navigation using `ICanNavigateFrom` to
    allow, deny, or request user confirmation.

-   **Dialog integration**\
    Unified dialog workflow via `IDialogService` with typed results.

-   **Async navigation lifecycle**\
    `INavigationAware` enables asynchronous initialization after
    navigation.

-   **Generic factory support (`IFactory<T>`)**\
    Provides a reusable DI-backed factory abstraction for creating
    ViewModels or other services.

-   **Microsoft.Extensions.DependencyInjection integration**\
    Designed to work naturally with the standard .NET dependency
    injection container.

-   **Microsoft.Extensions.Logging support**\
    Built-in logging using the standard .NET logging abstractions.

-   **Navigation state notifications**\
    The navigation service exposes events when the navigation state
    changes, enabling UI elements (e.g. back buttons) to update
    automatically.

-   **Optional WPF host builder**\
    `WpfNavigationHostBuilder` enables quick setup with fluent
    configuration.

-   **Framework-agnostic usage**\
    Can be integrated into existing bootstrapping processes without
    using the host builder.

### Planned

-   **CanNavigateTo with Authorization and Redirect**\
    Allows ViewModels to control whether navigation to a target is
    permitted and optionally redirect to another ViewModel (e.g. login
    flow).

-   **Initialization Lifecycle (early/late, sync/async)**\
    Supports structured ViewModel initialization stages before and after
    navigation with both synchronous and asynchronous execution.

-   **SaveState / RestoreState**\
    Enables persisting and restoring ViewModel or navigation state,
    useful for application restart or suspend/resume scenarios.

-   **IPubSub Service**\
    Lightweight publish/subscribe messaging service for decoupled
    communication between ViewModels.

-   **Navigation Events**\
    Exposes events such as `Navigating`, `Navigated`, and
    `NavigationFailed` for diagnostics, telemetry, and external
    orchestration.

-   **Typed Navigation Parameters**\
    Supports strongly typed navigation parameters to avoid string-based
    parameter keys.

-   **Integrating Mobile specific Back Navigation**\
    Support Androids intrinsic BackNavigation and offer an iOS
    consistent look for back navigation.


### Under Consideration

-   **Uno Platform** support (Desktop and Mobile)

-   **Navigation Scopes**\
    Allows creating scoped navigation contexts (e.g. wizard flows or
    multi-step processes) with isolated DI scopes.

-   **Navigation History API**\
    Exposes navigation history and back stack information for debugging,
    breadcrumbs, or custom navigation UI.

-   **Navigation Cancellation Support**\
    Adds `CancellationToken` support to navigation operations to cancel
    long-running initialization tasks.

-   **Diagnostics / Debug View**\
    Provides a developer-oriented diagnostics API to inspect current
    navigation state, back stack, and active ViewModels.

-   **Optional Route-Based Navigation**\
    Allows mapping routes (e.g. `/orders/42`) to ViewModels for
    scenarios where route-style navigation is desirable.

-   **ViewModel Activation Policies**\
    Supports controlling ViewModel lifetime (e.g. reuse existing
    instances, single-instance ViewModels).

-   **SplashScreen**\
    Show an interactive Splashscreen during startup to show an image,
    progress and messages.

------------------------------------------------------------------------

## ❤️ Support

If this project helps you, consider supporting its development:

-   GitHub Sponsors: https://github.com/sponsors/adaxer

------------------------------------------------------------------------

## 📄 License

Apache License 2.0


------------------------------------------------------------------------
