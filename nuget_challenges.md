# NuGet & Cross-Platform Challenges (ADaxer.MvvmNav)

This document summarizes the key challenges encountered while building and publishing the ADaxer.MvvmNav framework.

---

## 1. NuGet Publishing Complexity

Publishing a NuGet package is more than just building and uploading.

### Topics:
- Versioning (no overwrite, only new versions)
- Unlisting / deprecating instead of deleting
- Dependency hygiene
- Transitive dependencies
- SourceLink and PrivateAssets
- README and metadata quality

👉 **Key Insight:**
NuGet publishing is a form of packaging engineering.

---

## 2. Caching & Restore Behavior

NuGet caching can lead to confusing and inconsistent results.

### Topics:
- global-packages cache
- HTTP cache
- restore vs build vs pack behavior
- Visual Studio vs CLI differences
- same version ≠ same binaries

👉 **Key Insight:**
If behavior is unexplained, caching is usually the cause.

---

## 3. Target Framework Strategy

Target frameworks define compatibility and reach.

### Strategy:
- Abstractions → netstandard2.0
- Core → netstandard2.0
- WPF → net8.0-windows
- Avalonia → net8.0 + net10.0
- MAUI → net10.0-*

👉 **Key Insight:**
Target frameworks are an architectural decision, not just a technical one.

---

## 4. Separation of Concerns (Dependencies)

Libraries should not enforce runtime behavior.

### Examples:
- Logging:
  - avoid Debug provider in libraries
  - use abstractions only
- DI:
  - prefer abstractions
- Toolkit dependencies:
  - optional vs required

👉 **Key Insight:**
Libraries define capabilities, applications define behavior.

---

## 5. Platform Constraints (MAUI / Avalonia)

Modern UI frameworks impose strong constraints.

### Observations:
- MAUI requires current SDK/workloads
- Platform-specific TFMs are mandatory
- Avalonia requires multi-targeting for full support

👉 **Key Insight:**
Cross-platform development reduces control over runtime choices.

---

## 6. Tooling & Build Behavior

Tooling influences the final result more than expected.

### Topics:
- SourceLink behavior
- Debug vs Release packages
- symbol loading
- MSBuild server and file locks
- CLI vs IDE differences

👉 **Key Insight:**
Tooling is part of the system, not just a helper.

---

## 7. Library vs Application Mindset

A conceptual shift is required.

### Principles:
- no logging providers in libraries
- no global behavior enforcement
- no hidden defaults

👉 **Key Insight:**
Framework code must remain neutral.

---

## 8. NuGet Package UX

User experience is critical.

### Topics:
- correct README per package
- clear descriptions
- consistent naming
- easy onboarding

👉 **Key Insight:**
Users understand your package through NuGet, not your code.

---

## Summary (for Talk)

1. NuGet publishing is its own discipline  
2. Cross-platform development enforces architectural decisions  
3. Libraries must remain neutral and not control application behavior  

