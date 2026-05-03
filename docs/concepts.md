# MvvmNav Concepts

This document explains the core ideas behind **ADaxer.MvvmNav**.

It is intended as a practical bridge between the short README and the deeper architecture documentation.

If you are new to MvvmNav, start here after reading the README.

---

## What MvvmNav Is

**MvvmNav** is a lightweight, ViewModel-first navigation framework for .NET UI applications.

It provides a consistent navigation model across:

- WPF
- Avalonia
- MAUI

Depending on the platform and UI framework, this means your application can run on:

- Windows
- Linux
- macOS
- Android
- iOS

The central idea is simple:

> Navigation targets ViewModels, not Views.

Views are resolved by the UI platform.

---

## Why ViewModel-First Navigation?

In many UI applications, navigation is tightly coupled to concrete pages, windows, controls or routes.

MvvmNav takes a different approach:

```csharp
await navigation.NavigateAsync<DetailsViewModel>();
```

The ViewModel describes the logical target.

The platform decides how the matching View is displayed.

This keeps application logic independent from the concrete UI technology.

---

## The Shell Concept

MvvmNav applications usually have a central shell.

The shell is the composition point of the application UI.

A shell ViewModel typically exposes:

```csharp
object? CurrentModule { get; }
object? CurrentDialog { get; }
```

`CurrentModule` contains the currently active screen or module.

`CurrentDialog` contains the currently active dialog, if dialogs are hosted inside the shell.

The shell View displays these properties using the platform's native mechanisms.

---

## Shell ViewModel

The shell ViewModel implements `IShellViewModel`.

It usually acts as the host for modules and dialogs.

Conceptually:

```text
ShellViewModel
 ├─ CurrentModule
 └─ CurrentDialog
```

The shell does not need to know concrete Views.

It only hosts ViewModels.

---

## Shell View

The shell View implements `IShellView`.

Its responsibility is purely visual.

It defines where the current module and optional dialog are rendered.

Example concept:

```xml
<ContentControl Content="{Binding CurrentModule}" />
```

For dialogs, an overlay area can bind to `CurrentDialog`.

---

## NavigationService

`INavigationService` is the central service used by application ViewModels.

Typical usage:

```csharp
public class ShellViewModel
{
    private readonly INavigationService _navigation;

    public ShellViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }

    public Task ShowSettingsAsync()
        => _navigation.NavigateAsync<SettingsViewModel>();
}
```

The NavigationService is responsible for:

- creating target ViewModels
- setting the current module
- managing the back stack
- passing navigation parameters
- invoking navigation lifecycle hooks
- evaluating navigation guards

It is the main orchestration point for navigation.

---

## DialogService

Dialogs are handled separately from normal navigation.

A dialog is shown using:

```csharp
var result = await navigation.ShowDialogAsync<AboutViewModel>();
```

Dialog ViewModels are also resolved through dependency injection.

Depending on the platform, dialogs can be shown:

- as shell overlays
- as separate windows, where supported

The important part is that dialog handling remains ViewModel-first as well.

---

## View Resolution

MvvmNav does not force a single view resolution strategy on every platform.

Instead, each platform uses its natural mechanism.

### WPF and Avalonia

WPF and Avalonia usually resolve Views via DataTemplates.

Conceptually:

```text
ViewModel
   ↓
ContentControl
   ↓
DataTemplate
   ↓
View
```

Example:

```xml
<DataTemplate DataType="{x:Type vm:SettingsViewModel}">
    <views:SettingsView />
</DataTemplate>
```

### MAUI

MAUI does not use DataTemplates in the same way for this scenario.

Therefore, MvvmNav uses an explicit View registration model:

```csharp
services.RegisterView<SettingsViewModel, SettingsView>();
services.RegisterDialog<AboutViewModel, AboutView>();
```

This keeps MAUI ViewModel-first without requiring route-based navigation.

---

## Navigation Parameters

Navigation can include parameters.

```csharp
await navigation.NavigateAsync<DetailsViewModel>(
    ("Id", 42),
    ("Mode", "Edit"));
```

The target ViewModel can read those parameters by implementing `INavigationAware`.

```csharp
public class DetailsViewModel : INavigationAware
{
    public Task OnNavigatedToAsync(NavigationParameters parameters)
    {
        var id = parameters.GetValueOrDefault<int>("Id");

        return Task.CompletedTask;
    }
}
```

Use parameters for value-like navigation context, such as:

- Id
- Mode
- Filter
- Page
- Search text

For complex identity scenarios, prefer a custom navigation key.

---

## Navigation Lifecycle

A ViewModel can implement `INavigationAware`.

This allows the ViewModel to react after navigation.

```csharp
public interface INavigationAware
{
    Task OnNavigatedToAsync(NavigationParameters parameters);
}
```

Typical use cases:

- load data
- apply navigation parameters
- initialize state
- start async setup

This keeps constructors lightweight and allows async initialization.

---

## Back Navigation

MvvmNav includes back stack support.

```csharp
await navigation.GoBackAsync();
```

The back stack stores semantic navigation targets, not just UI controls.

That means an entry contains:

- ViewModel type
- navigation parameters
- navigation key
- target instance

This allows MvvmNav to restore the previous logical navigation state.

---

## Navigation Guards

A ViewModel can prevent or delay navigation by implementing `ICanNavigateFrom`.

Typical use case:

> The user has unsaved changes and tries to leave the current screen.

A guard can return:

- `Allow`
- `Disallow`
- `AskUser`

Example concept:

```csharp
public Task<NavigationGuardResult> CanNavigateFromAsync(
    NavigationRequest request,
    CancellationToken cancellationToken = default)
{
    if (!IsDirty)
        return Task.FromResult(NavigationGuardResult.Allow());

    return Task.FromResult(
        NavigationGuardResult.AskUser(
            "There are unsaved changes. Continue?",
            OnConfirmedAsync));
}
```

This keeps the decision close to the ViewModel that owns the state.

---

## Dialog Results

Dialogs return a `DialogResult`.

This allows simple confirmation flows:

```csharp
var result = await navigation.ShowDialogAsync<ConfirmViewModel>();

if (result == DialogResult.True)
{
    // user confirmed
}
```

For more advanced scenarios, dialog ViewModels can provide command metadata through `IDialogExchange`.

That allows the dialog host to render buttons based on ViewModel-provided commands.

---

## Dependency Injection

MvvmNav is designed to work with `Microsoft.Extensions.DependencyInjection`.

Application ViewModels, services and navigation infrastructure are registered in the DI container.

Typical setup:

```csharp
services
    .AddMvvmNav()
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>();
```

Application-specific registrations can be added as usual:

```csharp
services.AddSingleton<CustomerService>();
services.AddTransient<DetailsViewModel>();
```

The goal is to integrate naturally into existing application startup code.

---

## Logging

MvvmNav supports `Microsoft.Extensions.Logging`.

The framework can use `ILogger<T>` internally, but it does not force a concrete logging provider.

The application decides whether to use:

- console logging
- Debug / Trace logging
- Serilog
- another logging provider

This keeps the framework infrastructure-neutral.

---

## Platform Integration

Each platform integrates MvvmNav slightly differently, but follows the same conceptual model.

### WPF

WPF uses a fluent host builder.

```csharp
WpfNavigationHostBuilder
    .Default()
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>()
    .Build();

await host.StartAsync();
```

### Avalonia

Avalonia uses an `IMvvmNavStarter`. Its structure makes it necessary to also use some Builder code in the platform specific Bootstrapper.

``` csharp
// In Program.cs (Desktop)
// Or in MainApplication.CustomizeAppBuilder() (Android)
// Or in AppDelegate.CustomizeAppBuilder() (iOS)
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

The starter is resolved from DI and started during application initialization.

### MAUI

MAUI also uses an `IMvvmNavStarter`. And some bootstrapping in MauiApplication

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

The starter creates the main window and triggers startup navigation.

This keeps platform-specific startup complexity out of application ViewModels.

---

## Base Classes

MvvmNav follows an interface-first design.

Base classes are optional helpers, not a requirement.

For example, you can use:

- `ViewModelBase`
- `DialogViewModelBase`

But you can also implement the interfaces directly.

This keeps the framework flexible and avoids unnecessary inheritance chains.

---

## Typical Application Shape

A small MvvmNav application usually contains:

```text
App startup
 └─ registers services and MvvmNav

ShellView
 └─ displays CurrentModule and CurrentDialog

ShellViewModel
 └─ implements IShellViewModel

Feature ViewModels
 ├─ HomeViewModel
 ├─ SettingsViewModel
 └─ DetailsViewModel

Views
 ├─ HomeView
 ├─ SettingsView
 └─ DetailsView
```

Navigation happens between the ViewModels.

Views are attached by the platform.

---

## Mental Model

The mental model can be summarized like this:

```text
ViewModel requests navigation
        ↓
INavigationService resolves target ViewModel
        ↓
ShellViewModel.CurrentModule changes
        ↓
Platform renders the matching View
```

Dialogs follow the same idea:

```text
ViewModel requests dialog
        ↓
IDialogService resolves dialog ViewModel
        ↓
ShellViewModel.CurrentDialog changes
        ↓
Platform renders dialog UI
        ↓
DialogResult is returned
```

---

## Key Takeaways

- Navigation targets ViewModels.
- Views are resolved by the UI platform.
- The shell hosts the active module and optional dialog.
- `INavigationService` orchestrates navigation.
- `IDialogService` handles dialogs.
- Parameters are passed through `NavigationParameters`.
- Guards allow ViewModels to control whether navigation may continue.
- DI and logging integrate with standard .NET infrastructure.
- Base classes are optional.
- The framework is intentionally small and understandable.

---

## Next Steps

- Read the platform-specific setup in the README.
- Explore the sample application.
- Continue with the deeper architecture documentation if you want to understand the internal design.
