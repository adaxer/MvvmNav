# ADaxer.MvvmNav – Chat Context

Repository  
https://github.com/adaxer/MvvmNav

## Goal

ADaxer.MvvmNav is a cross-platform MVVM navigation framework for .NET.

Target UI platforms:

- WPF
- Avalonia
- .NET MAUI
- possibly Uno

The framework focuses on ViewModel-driven navigation with minimal UI coupling.

---

# Solution Structure

ADaxer.MvvmNav.Abstractions  
Navigation contracts and shared interfaces.

ADaxer.MvvmNav.Core  
Platform-independent navigation implementation.

ADaxer.MvvmNav.Wpf  
WPF integration and bootstrapping.

ADaxer.MvvmNav.Avalonia  
Avalonia integration.

Samples:

Sample.Common  
ViewModels used by sample apps.

Sample.Wpf  
WPF sample application.

---

# Main Interfaces

Navigation system:

INavigationService  
INavigationHost  
IDialogHost  

Shell infrastructure:

IShellView  
IShellViewModel  

Dialog integration:

IDialogService  

Supporting types:

NavigationContext  
NavigationOptions  
DialogResult  

---

# Core Components

NavigationService  
ShellNavigationHost  
DialogHost  

These are platform-independent and live in `MvvmNav.Core`.

---

# WPF Integration

MvvmNav.Wpf provides:

AddMvvmNavWpf()

and a generic bootstrapper:

MvvmNavWpfBootstrapper

Main methods:

Build<TShellView, TShellViewModel>()  
Start<TShellView, TShellViewModel>()  
BuildAndStart<TShellView, TShellViewModel>()

The bootstrapper:

- registers MvvmNav
- registers shell view + viewmodel
- sets DataContext
- shows the shell

---

# Framework Dependencies

CommunityToolkit.Mvvm  
Microsoft.Extensions.DependencyInjection

---

# Sample Shell Layout (WPF)

ShellWindow uses:

DockPanel

Left side  
ItemsControl for navigation

Center  
ContentControl bound to:

CurrentModule

ShellViewModel contains:

CurrentModule  
Title  
IsBusy  
NavigationItems

---

# Current Development Focus

Currently working on:

- WPF sample app
- navigation testing
- dialog infrastructure