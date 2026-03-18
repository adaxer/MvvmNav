## Dependency Injection

MvvmNav is built on `Microsoft.Extensions.DependencyInjection`.

### How it works

- ViewModels are resolved via DI
- Services are injected via constructors
- Framework components are registered automatically
- No service locator required
- Generic Factory (`IFactory<T>`) is included for creating instances correctly

### Benefits

- Familiar .NET patterns
- Easy integration into existing apps