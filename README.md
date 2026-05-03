# ADaxer.MvvmNav

[![Downloads](https://img.shields.io/nuget/dt/ADaxer.MvvmNav.Core?label=downloads&color=green)](https://www.nuget.org/packages/ADaxer.MvvmNav.Core)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue)](LICENSE)

A lightweight, ViewModel-first navigation framework for .NET UI applications.

Run the same navigation concepts across:
**Windows · Linux · macOS · Android · iOS**

Supports WPF, Avalonia and MAUI with a consistent mental model and minimal setup.

---

## ✨ Why MvvmNav?

- ViewModel-first navigation
- Clean separation of concerns
- Minimal infrastructure (no heavy frameworks)
- Cross-platform core
- Fully DI and logging compatible
- Works with existing applications

👉 Want to understand how it works internally?  
See [Architecture & Concepts](docs/architecture.md)

---

## 🚀 Quick Start

```csharp
services
    .AddMvvmNav()
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>();
```

Inject `INavigationService` into your ViewModels and start navigating.

👉 More detailed setup and concepts:  
[Getting Started & Concepts](docs/concepts.md)

---

## 🧩 The Shell Concept (Core Idea)

MvvmNav uses a central Shell ViewModel that hosts:

- `CurrentModule`
- `CurrentDialog`

This is the composition root of your UI.

---

## 🖥️ WPF

```csharp
WpfNavigationHostBuilder
    .Default()
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>()
    .Build();

await host.StartAsync();
```

---

## 🧩 Avalonia

```csharp
services.AddMvvmNav()
    .WithShell<ShellWindow, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>();
```

---

## 📱 MAUI

```csharp
builder.Services
    .AddMvvmNav()
    .WithShell<ShellPage, ShellViewModel>()
    .WithStartupNavigation<HomeViewModel>();
```

---

## 🔄 Navigation

```csharp
await navigation.NavigateAsync<HomeViewModel>();
```
👉 Deep dive into navigation, parameters and back stack:  
[Navigation Guide](docs/navigation.md)

---

## 🛑 Navigation Guards

Prevent navigation when needed (e.g. unsaved changes).

👉 Full guard flows and advanced scenarios:  
[Navigation Guards & Flow](docs/navigation.md)

---

## 📸 Sample Application

Cross-platform sample included in the repository.

- Navigation & back stack
- Dialogs 
- Guards
- Markdown help pages

👉 Walkthrough of the sample app and architecture decisions:  
[Sample App Guide](docs/samples.md)

---

## 📚 Documentation

- [Architecture & Concepts](docs/architecture.md)
- [Navigation Guide](docs/navigation.md)
- [Dialogs](docs/dialogs.md)
- [Sample App](docs/samples.md)

---

## 🛠️ Status

Actively developed – API stabilizing toward v1.

---

## 🎯 Roadmap

### Current
- ViewModel-first navigation
- Cross-platform core
- Navigation parameters
- Dialog integration

### Planned
- Authorization / redirects
- State persistence
- Typed parameters

---

## ❤️ Support

https://github.com/sponsors/adaxer

---

## 📄 License

Apache License 2.0
