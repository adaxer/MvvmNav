## Generic Factory

MvvmNav includes a generic `IFactory<T>` abstraction.

### Purpose

- Create instances with full DI support
  - For example to create a new Customer
- Avoid direct dependency on `IServiceProvider`

### Benefits

- Cleaner architecture
- Better testability
- Explicit dependencies