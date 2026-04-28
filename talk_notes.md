# 🎤 Talk Notes -- Real-World Learnings from Building a Cross-Platform MVVM Navigation Framework

## 🧨 1. Directory.Build.props -- The Global Saboteur

A single line can break your entire multi-target setup:

``` xml
<TargetFramework>net10.0</TargetFramework>
```

**Problem:** - Overrides MAUI / WPF / multi-target projects - Leads to
confusing errors like: - project.assets.json missing target - app won't
start

**Key Insight:** \> Props are imported before the project → and they
win.

------------------------------------------------------------------------

## 🧠 2. MAUI is Never Truly Platform-Neutral

Even if your code is:

-   just XAML
-   just ContentView
-   no platform APIs

MAUI still forces:

-   platform-specific builds
-   multiple target frameworks
-   handler infrastructure

**Key Insight:** \> You can write platform-neutral code -- but MAUI will
still build it platform-specific.

------------------------------------------------------------------------

## 🧩 3. Visual Studio vs CLI -- Two Different Worlds

**Observation:**

-   Visual Studio → works
-   dotnet build → fails

**Reason:**

  Visual Studio            CLI
  ------------------------ ---------------------
  Uses ProjectReferences   Uses NuGet Packages
  Uses cache               Real restore

**Key Insight:** \> If it only works in Visual Studio, it doesn't really
work.

------------------------------------------------------------------------

## 🧹 4. The Hidden Enemy: IDE Cache

Symptoms:

-   everything looks correct
-   still broken
-   weird startup errors

**Fix:**

``` powershell
dotnet clean
Remove-Item -Recurse -Force bin,obj
Remove-Item -Recurse -Force .vs
```

**Key Insight:** \> When nothing makes sense anymore, delete everything
first.

------------------------------------------------------------------------

## 📦 5. Local NuGet Feeds & Versioning Trap

Problem:

-   Package updated
-   Version unchanged
-   NuGet still uses old version

Symptoms:

-   types missing
-   XAML errors
-   "but I just added that class!"

**Key Insight:** \> NuGet doesn't cache files -- it caches trust in your
version number.

------------------------------------------------------------------------

## 🔍 6. SourceLink ≠ Debug Build

Expectation:

> "I have SourceLink, so I can debug the library."

Reality:

-   code is still Release
-   optimizations remain
-   debugging is limited

**Key Insight:** \> SourceLink lets you read the code -- not debug it
perfectly.

------------------------------------------------------------------------

## ⚙️ 7. The 3 Debug Switches (Huge Impact)

For debugging NuGet packages:

-   Just My Code → OFF
-   Suppress JIT optimization → ON
-   SourceLink → ON

**Result:**

-   breakpoints work
-   source loads automatically
-   stepping is usable

------------------------------------------------------------------------

## 🤯 8. Packaging is Not Just Zipping

Common misconception:

> "dotnet pack just packages my DLL"

Reality:

-   pack orchestrates build outputs
-   multi-target projects behave differently
-   missing outputs → broken packages

**Key Insight:** \> Packaging is a build process, not a file operation.

------------------------------------------------------------------------

## 📱 9. Reality Check: Hardware Can Block You

Example:

-   Xiaomi devices require SIM for USB install

**Key Insight:** \> Not all problems are in your code -- sometimes the
device fights back.

------------------------------------------------------------------------

## 🎯 Core Message of the Talk

> Building a clean cross-platform library reveals hidden complexity in:
>
> -   build systems
> -   tooling
> -   packaging
> -   debugging
>
> Most problems are not in your code -- but in the layers around it.

------------------------------------------------------------------------

## 🚀 Closing Thought

> The real challenge is not writing cross-platform code.
>
> It's understanding everything that happens *around* it.
