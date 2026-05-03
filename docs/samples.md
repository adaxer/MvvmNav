# Sample Application Guide

This document explains the structure and purpose of the **MvvmNav sample application**.

The sample is designed to demonstrate real-world usage of navigation, dialogs and guards in a simple, understandable way.

---

## Goals of the Sample

The sample application is not just a demo.

It is used to:

- validate architectural decisions
- demonstrate real usage patterns
- provide copy/paste-ready examples
- keep the framework grounded in practical scenarios

---

## Application Structure

A typical sample setup looks like this:

```text
App
 └─ registers services

ShellView
 └─ displays CurrentModule and CurrentDialog

ShellViewModel
 └─ navigation entry point

Modules
 ├─ Home
 ├─ Settings
 ├─ Details
 └─ About (Dialog)
```

---

## Shell

The Shell is the main container of the application.

It provides:

- navigation UI (e.g. sidebar)
- content area
- dialog overlay

### Responsibilities

- display CurrentModule
- display CurrentDialog
- host navigation commands

---

## Modules Overview

### Home

Purpose:

- entry screen
- introduction to the app
- simple navigation

Demonstrates:

- basic navigation

---

### Details

Purpose:

- show parameter-based navigation

Demonstrates:

- NavigationParameters
- INavigationAware

Example:

```csharp
await navigation.NavigateAsync<DetailsViewModel>(
    ("Id", 42));
```

---

### Settings

Purpose:

- demonstrate navigation guards

Demonstrates:

- ICanNavigateFrom
- AskUser flow
- save / discard / cancel logic

---

### About (Dialog)

Purpose:

- simple dialog example

Demonstrates:

- ShowDialogAsync
- DialogResult handling

---

## Navigation Flow Example

```text
User clicks menu
        ↓
ShellViewModel calls NavigateAsync
        ↓
NavigationService resolves ViewModel
        ↓
Shell updates CurrentModule
        ↓
View is rendered
```

---

## Guard Flow Example

```text
User tries to navigate away
        ↓
ICanNavigateFrom is invoked
        ↓
AskUser dialog shown
        ↓
User decision returned
        ↓
Navigation continues or stops
```

---

## Dialog Flow Example

```text
ViewModel requests dialog
        ↓
DialogService creates ViewModel
        ↓
Shell shows dialog
        ↓
User interacts
        ↓
DialogResult returned
```

---

## Markdown-Based Help (Optional)

The sample may include help pages rendered from Markdown.

This allows:

- inline documentation
- feature explanations
- cross-platform consistency

---

## Cross-Platform Approach

The same ViewModels are reused across:

- WPF
- Avalonia
- MAUI

Only platform-specific setup differs.

This demonstrates the core idea:

> Navigation logic is platform-independent.

---

## Key Takeaways

- The sample is a **design driver**, not just a demo
- It demonstrates real navigation scenarios
- It shows how ViewModels remain UI-independent
- It provides working patterns for:
  - navigation
  - dialogs
  - guards

---

## Next Steps

- Explore the sample project in the repository
- Modify modules to test behavior
- Use it as a starting point for your own application
