# ADaxer.MvvmNav -- Context Summary

This document reflects the current architectural direction and recent design decisions.

## Project Goal

**ADaxer.MvvmNav** is a lightweight MVVM navigation framework for .NET UI applications.

It is a nuget package, and the project site is OSS on 
https://github.com/adaxer/MvvmNav

It focuses on simplicity, clear responsibilities, and minimal infrastructure while integrating naturally with the UI platform.

The framework is designed to work across multiple UI technologies.

### Supported Platforms (Target)

- **Avalonia** (Desktop and Mobile)
- **WPF**
- **MAUI**

The core navigation logic is platform-agnostic, while the UI layer uses the native mechanisms of the respective platform.

------------------------------------------------------------------------

# Architecture Overview

## Core Components

### NavigationService

Central orchestrator responsible for:

- navigation between ViewModels
- maintaining a navigation back stack
- evaluating navigation guards (`ICanNavigateFrom`)
- integrating dialog results
- creating ViewModels via dependency injection

The NavigationService intentionally acts as the **single orchestration point**.

------------------------------------------------------------------------

### DialogService

Responsible for:

- showing modal dialogs
- hosting dialog views
- returning a `DialogResult`

Dialog views are resolved using the platform's native mechanism.

## Recent Decisions / Current State

### Logging
- The framework supports `Microsoft.Extensions.Logging`.
- Core services (e.g. `NavigationService`) may use `ILogger<T>`.
- Concrete logging providers are configured at application/bootstrap level.
- The framework itself does not enforce a specific logging provider.

### Registration Order
- Recommended registration order:
  1. Core
  2. Platform
  3. Application
- Later registrations override earlier ones.
- This allows application-level customization without modifying the framework.

### WPF Host Builder
- WPF provides an optional fluent host builder:
  `WpfNavigationHostBuilder`
- Intended usage:
  - `WpfNavigationHostBuilder.BuildDefault<TShellView, TShellViewModel>().Start()`
  - `WpfNavigationHostBuilder.Build<TShellView, TShellViewModel>().WithLogging(...).WithServices(...).Start()`
- The host builder is optional and serves as a convenience layer.
- Existing applications can integrate MvvmNav without using it.

### Navigation State Notifications
- `INavigationService` exposes navigation state change notifications.
- Raised after:
  - successful navigation
  - successful back navigation
- Not raised when:
  - navigation is blocked by a guard
  - navigation is cancelled (e.g. AskUser → None)
  - dialogs are shown
- Typical use case:
  - updating shell commands (e.g. Back button)
  - refreshing `CanExecute` state

### Navigation Parameter Convenience
- Navigation can be invoked using tuple-based parameters:
  `NavigateAsync<TTarget>(("Key", value), ("Other", 42))`
- This avoids explicit construction of `NavigationParameters` in common scenarios.
- Implemented via extension methods on `INavigationService`.

------------------------------------------------------------------------

# View Resolution Strategy

The framework uses the native UI platform mechanism where possible.

For WPF / Avalonia / Uno this remains template-based:

    ViewModel
       ↓
    ContentControl
       ↓
    DataTemplate
       ↓
    View

Example:

    <DataTemplate DataType="{x:Type vm:SettingsViewModel}">
        <views:SettingsView/>
    </DataTemplate>

For MAUI, a lightweight explicit registration-based locator is currently used:

- `IViewLocator` lives in Abstractions
- MAUI provides a concrete implementation
- views are registered explicitly via:
  - `RegisterView<TViewModel, TView>()`
  - `RegisterDialog<TViewModel, TView>()`

This keeps the framework small while allowing MAUI to stay ViewModel-first without relying on Shell routing.

------------------------------------------------------------------------

# Navigation Guards

ViewModels can prevent navigation via:

    ICanNavigateFrom

Return type:

    NavigationGuardResult

Possible decisions:

- `Allow`
- `Disallow`
- `AskUser`

------------------------------------------------------------------------

## AskUser Flow

When a ViewModel returns `AskUser`:

1. `NavigationService` shows a dialog
2. The dialog returns a `DialogResult`
3. The result is passed back to the ViewModel through a continuation callback

Callback signature:

    Func<DialogResult, CancellationToken, Task>

This allows the ViewModel to continue the decision asynchronously (e.g. saving changes).

When a navigation guard returns `AskUser`, the result of the confirmation dialog determines the outcome:

- `DialogResult.True` → proceed with navigation using a True Result
- `DialogResult.False` → proceed with navigation using a False Result
- `DialogResult.None` → cancel navigation

This enables scenarios like:
- Save changes (True)
- Discard changes (False)
- Cancel navigation (None)

------------------------------------------------------------------------

# Dialog Model

Dialog ViewModels typically derive from:

    DialogViewModelBase

The base class encapsulates dialog completion infrastructure.

Public interaction remains centered around:

    CloseDialog(DialogResult result)

Technical dialog completion is handled internally by:

    IDialogCompletionSource

which encapsulates the completion task machinery.

## Dialog Hosting Direction

The current direction is toward a unified shell-hosted dialog model across platforms.

### Shell Integration
- `IShellViewModel` derives from both:
  - `IModuleHost`
  - `IDialogHost`
- The shell therefore hosts both:
  - `CurrentModule`
  - `CurrentDialog`

### Overlay Hosting
- MAUI currently hosts dialogs as an overlay inside the shell.
- The dialog overlay uses a `ContentControl` with `IsDialog="True"`.
- The dialog shell view in MAUI is `MauiDialog`.
- `MauiDialog` hosts the actual dialog content through an inner `ContentControl`.

This keeps dialogs inside the application's visual composition instead of requiring a separate native window.

### WPF Direction
WPF supports two dialog hosting modes: shell overlay and separate window hosting.
- Overlay is the default mode.
- Window hosting uses WpfDialogWindow.

## Dialog Command Exchange

A dialog ViewModel may implement:

    IDialogExchange

It exposes:

    DialogExchangeInfo DialogExchange { get; }

`DialogExchangeInfo` currently provides:

- `Commands : IReadOnlyList<DialogCommandInfo>`
- `ContinueAsync : Func<DialogResult, CancellationToken, Task<bool>>?`

`DialogCommandInfo` currently provides:

- `Text`
- `IsPrimary`
- `DialogResult`

Semantics:

- the dialog host renders command buttons from the ViewModel metadata
- clicking a command passes its `DialogResult` back to the ViewModel through `ContinueAsync`
- the callback returns whether the dialog should actually close
- if no exchange info is provided, the host falls back to a default single `OK` command

This avoids old fixed button concepts like `YesNoCancel` while still supporting validation-driven scenarios such as login dialogs or later wizard-like flows.

------------------------------------------------------------------------

# Base Classes

Base classes are **optional convenience helpers**.

The framework itself works primarily with interfaces.

## ViewModelBase

Derived from:

    ObservableObject (CommunityToolkit.Mvvm)

Provides commonly useful properties:

- `Title`
- `IsBusy`

------------------------------------------------------------------------

## DialogViewModelBase

Derived from:

    ViewModelBase

Implements dialog completion infrastructure and acts as the common base for dialog ViewModels such as `AboutViewModel` or `LoginViewModel`.

------------------------------------------------------------------------

# Navigation Lifecycle

## Navigation Semantics

- Navigation is ViewModel-to-ViewModel via `NavigateAsync(...)`.
- Back navigation uses `GoBackAsync()`.
- Dialogs are separate via `ShowDialogAsync(...)` (no modal flag on navigation).

### Target Identity

A navigation target is identified by:
- `TargetType`
- `NavigationKey`

Navigation to the same target is blocked only if **both match**.

Examples:
- Same type, different key (e.g. different Id) → allowed
- Same type, same key → blocked

### NavigationKey

- Default: `TargetType + normalized NavigationParameters`
- Custom: `NavigationOptions.WithKey("...")`

Use a custom key when:
- only a subset of parameters defines identity
- parameters contain complex objects
- `ToString()` is not stable enough

### NavigationParameters

- Immutable parameter bag
- Used to pass context and (by default) define identity
- Prefer primitive/value-like data (Id, Filter, Page, Mode)
- For complex cases, provide a custom `NavigationKey`

### Back Stack

Each entry stores:
- Target instance
- TargetType
- Parameters
- NavigationKey

`GoBackAsync()` restores the original semantic target (type + parameters + key).

#### Back Stack Options Interaction

- `ClearBackStack = true` clears all existing back stack entries.
- `AddToBackStack = true` is still evaluated independently.

If both are set:
- The previous back stack is cleared
- The current entry is added as the sole back stack entry

This allows scenarios like:
- Reset navigation history but still allow a single back navigation

### Detail Paging Scenario

- Same ViewModel type is allowed if the target identity changes
- Example: `Detail(Id=10)` → `Detail(Id=11)` allowed
- `Detail(Id=10)` → `Detail(Id=10)` blocked

ViewModels may implement:

    INavigationAware

Lifecycle method:

    Task OnNavigatedToAsync(NavigationParameters parameters)

The same hook is used for both normal navigation and dialog navigation.

### Navigation Event
When navigation is blocked due to identical target identity (same type + same `NavigationKey`):

- Navigation is not performed
- `NavigationStateChanged` is NOT raised

------------------------------------------------------------------------

# Platform Integration

Platform integration should be made as easy as possible, using a fluent and consistent approach across platforms.

The core idea is:

> Each platform provides a lightweight IMvvmNavStarter that encapsulates all platform-specific bootstrapping.

This keeps the user-facing integration minimal while preserving full flexibility.

---

## Starter Concept (MAUI / Avalonia)

Each platform provides its own implementation of:

IMvvmNavStarter

Responsibilities:

- initialize platform-specific resources (styles, templates, etc.)
- resolve and attach the shell
- trigger startup navigation
- handle platform-specific lifetime differences

The starter is resolved via DI and invoked at the appropriate lifecycle point of the platform.

---

## WPF

WPF uses a dedicated fluent builder:

WpfNavigationHostBuilder
    .Default()
    .WithServices(...)
    .WithLogging(...)
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>()
    .Build();

await host.StartAsync();

WPF does not use IMvvmNavStarter, because the builder already encapsulates the startup process.

---

## Avalonia

Avalonia uses IMvvmNavStarter and integrates into:

OnFrameworkInitializationCompleted()

Example:

public override async void OnFrameworkInitializationCompleted()
{
    var starter = Services.GetRequiredService<IMvvmNavStarter>();

    starter.Initialize(this);
    await starter.StartAsync();

    base.OnFrameworkInitializationCompleted();
}

### Notes

- Application must have a parameterless constructor (platform constraint)
- services are injected via property (not constructor)
- starter handles:
  - lifetime detection:
    - Desktop → MainWindow
    - iOS → MainView
    - Android → MainViewFactory
  - resource loading (Styles + ResourceDictionary)
  - shell resolution

---

## MAUI

MAUI also uses IMvvmNavStarter, resolved via DI inside App.

Example:

public partial class App : Application
{
    private readonly IMvvmNavStarter _starter;

    public App(IMvvmNavStarter starter)
    {
        InitializeComponent();
        _starter = starter;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return _starter.CreateWindow();
    }

    protected override async void OnStart()
    {
        await _starter.StartAsync();
    }
}

### Current Direction

- custom shell host (no AppShell)
- CurrentModule rendered via custom ContentControl
- CurrentDialog rendered as overlay
- ViewModel-first navigation
- no route-based navigation required

---

## Shell Integration

The shell is optional at configuration time, but required at runtime.

- If configured:
  - starter resolves and attaches it
- If missing:
  - startup throws a clear exception

Shell behavior:

- IShellViewModel
  - may implement IDialogHost
- framework detects capabilities automatically

---

## View Resolution (All Platforms)

Example:

<ContentControl Content="{Binding CurrentModule}" />

The DataTemplate associated with the ViewModel resolves the View automatically.

---

## Dialogs

- WPF:
  - Overlay or Window mode
- MAUI / Avalonia:
  - Overlay only (recommended default)

Dialogs are bound via:

IDialogHost.CurrentDialog

---

## Design Goals

- minimal required setup in user code
- consistent mental model across platforms
- platform-specific complexity hidden inside starter
- no overengineering

------------------------------------------------------------------------

# Messaging / PubSub

Loose communication between components is implemented using:

    CommunityToolkit.Mvvm IMessenger

Typical use case:

- status messages
- navigation notifications
- dialog results

Example: updating a shell status bar.

------------------------------------------------------------------------

# Roadmap

## Featureset v1

- **ViewModel-first navigation**  
  Navigate between ViewModels without coupling them to concrete views.

- **Platform-agnostic core**  
  The navigation engine resides in a UI-independent Core library.

- **Native / platform-appropriate view resolution**  
  Views are resolved using the platform’s natural mechanism (e.g. WPF `DataTemplate`, MAUI registered view locator).

- **Navigation parameters**  
  Pass parameters when navigating between ViewModels.

- **Back navigation with stack management**  
  Built-in back stack with support for clearing or suppressing entries.

- **Navigation guards**  
  ViewModels can intercept navigation using `ICanNavigateFrom` to allow, deny, or request user confirmation.

- **Dialog integration**  
  Unified dialog workflow via `IDialogService` with typed results.

- **Async navigation lifecycle**  
  `INavigationAware` enables asynchronous initialization after navigation.

- **Generic factory support (`IFactory<T>`)**  
  Provides a reusable DI-backed factory abstraction for creating ViewModels or other services.

- **Microsoft.Extensions.DependencyInjection integration**  
  Designed to work naturally with the standard .NET dependency injection container.

- **Microsoft.Extensions.Logging support**  
  Built-in logging using the standard .NET logging abstractions.

- **Navigation state notifications**  
  The navigation service exposes events when the navigation state changes, enabling UI elements (e.g. back buttons) to update automatically.

- **Optional WPF host builder**  
  `WpfNavigationHostBuilder` enables quick setup with fluent configuration.

- **Framework-agnostic usage**  
  Can be integrated into existing bootstrapping processes without using the host builder.

## Planned

- **CanNavigateTo with Authorization and Redirect**  
  Allows ViewModels to control whether navigation to a target is permitted and optionally redirect to another ViewModel (e.g. login flow).

- **Initialization Lifecycle (early/late, sync/async)**  
  Supports structured ViewModel initialization stages before and after navigation with both synchronous and asynchronous execution.

- **SaveState / RestoreState**  
  Enables persisting and restoring ViewModel or navigation state, useful for application restart or suspend/resume scenarios.

- **IPubSub Service**  
  Lightweight publish/subscribe messaging service for decoupled communication between ViewModels.

- **Navigation Events**  
  Exposes events such as `Navigating`, `Navigated`, and `NavigationFailed` for diagnostics, telemetry, and external orchestration.

- **Typed Navigation Parameters**  
  Supports strongly typed navigation parameters to avoid string-based parameter keys.

---

### Under Consideration
- **Uno Platform** support (Desktop and Mobile)

- **Navigation Scopes**  
  Allows creating scoped navigation contexts (e.g. wizard flows or multi-step processes) with isolated DI scopes.

- **Navigation History API**  
  Exposes navigation history and back stack information for debugging, breadcrumbs, or custom navigation UI.

- **Navigation Cancellation Support**  
  Adds `CancellationToken` support to navigation operations to cancel long-running initialization tasks.

- **Diagnostics / Debug View**  
  Provides a developer-oriented diagnostics API to inspect current navigation state, back stack, and active ViewModels.

- **Optional Route-Based Navigation**  
  Allows mapping routes (e.g. `/orders/42`) to ViewModels for scenarios where route-style navigation is desirable.

- **ViewModel Activation Policies**  
  Supports controlling ViewModel lifetime (e.g. reuse existing instances, single-instance ViewModels).

- **SplashScreen**  
  Show an interactive Splashscreen during startup to show an image, progress and messages.

------------------------------------------------------------------------

# Design Principles

Key architectural principles:

1. **Interface-first design**  
   Base classes are optional.

2. **Use platform mechanisms**  
   The framework stays close to how the target UI platform wants to work.

3. **Minimal infrastructure**  
   Only a few core services exist:
   - `NavigationService`
   - `DialogService`

4. **ViewModel-first navigation**  
   Navigation always targets ViewModels.

5. **Small and understandable framework**  
   The goal is not to replicate large frameworks like Prism.

6. **Step-by-step evolution**  
   Architectural changes should be introduced incrementally and evaluated in sample applications before broader generalization.

------------------------------------------------------------------------

# Sample Application Goals

The sample application demonstrates all major features in a simple way.

## Modules

### Shell

Shows:

- navigation
- back stack
- status bar
- dialog overlay hosting

------------------------------------------------------------------------

### Home

Landing page introducing navigation.

------------------------------------------------------------------------

### Details

Demonstrates:

- `NavigationParameters`
- `INavigationAware`

------------------------------------------------------------------------

### Settings

Demonstrates:

- dirty state tracking
- `ICanNavigateFrom`
- `AskUser` confirmation dialog
- save / discard / cancel scenarios

------------------------------------------------------------------------

### About

Demonstrates:

- simple dialog
- `DialogViewModelBase`

------------------------------------------------------------------------

### PlainViewModel Example

Demonstrates usage **without framework base classes**, using only interfaces.

## Sample App – Current Direction

### Sample App as Design Driver

- The sample application is not only a demonstration, but actively drives architectural decisions.
- Design choices are validated against real UI scenarios rather than theoretical abstraction.
- This approach helps to avoid premature generalization.

### General Approach
- Prefer a single conceptual sample application shared across platforms.
- Platform-specific integration is demonstrated per platform:
  - WPF: `WpfNavigationHostBuilder` + Serilog
  - Avalonia: default builder
  - MAUI: integration into existing app bootstrap with custom shell page
  - Uno: planned after WPF/Avalonia/MAUI

### Shell Navigation
- The shell provides a left navigation menu.
- Each menu item consists of:
  - Title
  - Subtitle
  - Command
- Current modules:
  - Home
  - About
  - Settings
  - Features

### Markdown-based Explanations
- Sample pages use markdown for inline documentation.
- Markdown is bound via a ViewModel property (`string Markdown`).
- Each platform uses a platform-specific markdown renderer.
- Feature detail pages load markdown files based on a navigation parameter key.

### Sample Content Focus
- Home:
  - MvvmNav SampleApp overview
  - ViewModel-first navigation
  - ShellView / ShellViewModel only require framework interfaces
  - NavigationService is injected into the ShellViewModel
  - Views are resolved via DataTemplates or MAUI view registration
- About:
  - demonstrates dialog usage
- Settings:
  - demonstrates navigation and back navigation
- Features:
  - overview page with links to detail pages

------------------------------------------------------------------------

# Optional Sample Features

### Status Bar

Implemented using:

    IMessenger

Displays:

- navigation events
- dialog results
- guard cancellations

------------------------------------------------------------------------

### Factory Pattern

Optional example:

    IFactory<T>

Example use case:

    EditCustomerViewModel

------------------------------------------------------------------------

# Key Takeaways

Core ideas of the framework:

- Navigation is orchestrated by `NavigationService`
- Views are resolved in a platform-appropriate way
- Dialogs are hosted by `DialogService`
- Navigation guards control navigation flow
- Base classes are optional helpers
- CommunityToolkit.Mvvm provides MVVM infrastructure

------------------------------------------------------------------------

# Result

The framework aims to be:

- small
- understandable
- platform-agnostic in the core
- extensible
- easy to integrate

This constitutes a solid evolving **version 1 architecture**.

------------------------------------------------------------------------

# Testing

## TestFramework
TestFramework is TUnit

## Conventions
- Test classes are to be named like the type to be tested with two trailing underscores, e.g. `DialogViewModelBase__`
- When external references are necessary, they should be injected into the test class; here the `DIClassConstructor` is to be adapted and used
- The comments `// Arrange`, `// Act` and `// Assert` are to be used and put together if those states overlap

## Testing Strategy – Suggested Additions

### 1. Public API / Guard Tests

Goal: Ensure stability of the public API

- Constructor guards (`ArgumentNullException`)
- `NavigateAsync(null)` → throws
- `NavigationOptions.WithKey(null/empty)` → throws
- Extension methods (`AddMvvmNavCore`, etc.) validate parameters

---

### 2. Dependency Injection / Registration

Goal: Ensure correct framework setup

- `AddMvvmNavCore` registers:
  - `INavigationService`
  - `IFactory<T>`
- Registration order: later registrations override earlier ones
- Platform-specific extensions:
  - WPF: resources are merged correctly
  - MAUI: `ViewLocator` is set and usable
- Multiple registrations do not lead to inconsistent state

---

### 3. Base Classes

Goal: Verify behavior of helper classes

- `GenericFactory<T>`
  - resolves instances correctly
  - throws when dependency is missing

- `ViewModelBase`
  - `Title` initialized meaningfully
  - `IsBusy` default value correct

- `DialogViewModelBase`
  - `CompletionTask` initially returns `None`
  - `ResetDialogCompletion()` works as expected
  - multiple `CloseDialog` calls are idempotent
  - dialog can be reused after reset

- `MessageViewModel`
  - `DialogExchange` correctly constructed
  - commands exposed as expected

---

### 4. NavigationService – Edge Cases

Goal: Ensure robustness of core orchestration

- `GoBackAsync()` on empty stack → no effect
- failed ViewModel creation → no state change
- exception in `OnNavigatedToAsync()` → defined behavior
- `AskUser` flow:
  - `ContinueAsync` invoked exactly once
  - `DialogResult` propagated correctly
- navigation to identical target is prevented
- combination:
  - `ClearBackStack` + `AddToBackStack`

---

### 5. NavigationParameters / NavigationOptions

Goal: Stabilize parameter handling

- `GetValueOrDefault<T>` with explicit `null`
- `TryGetValue<T>` with base types / interfaces
- define and test key case-sensitivity
- stability of parameter normalization (`ToString()` implications)
- custom `NavigationKey` correctly applied

---

### 6. Platform Integration Tests (Current Gap)

Goal: Validate platform-specific behavior

- MAUI:
  - `RegisterView<TVm,TView>()` works correctly
  - `RegisterDialog<TVm,TView>()` works correctly
  - `ViewLocator` resolves views as expected

- WPF:
  - ResourceDictionary loaded correctly
  - view resolution via DataTemplates

- General:
  - no crashes on repeated initialization
  - basic smoke tests per platform

---

## Prioritization

### High
- NavigationService edge cases
- DI / registration
- public API guards

### Medium
- base classes
- NavigationParameters

### Low (but important long-term)
- platform integration tests

---

## Summary

- Core logic is already well covered
- Main gaps are at the boundaries:
  - DI setup
  - platform integration
  - failure paths

👉 Guiding principle:  
**Stabilize orchestration first, then validate integration.**
------------------------------------------------------------------------
