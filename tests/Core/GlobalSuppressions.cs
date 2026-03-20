// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Suppress async suffix warnings for test methods
[assembly: SuppressMessage("Naming", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods should describe behavior, not implementation details")]
[assembly: SuppressMessage("Naming", "RCS1046:Asynchronous method name should end with 'Async'", Justification = "Test methods should describe behavior, not implementation details")]
[assembly: SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage", Justification = "Test methods should describe behavior, not implementation details")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Test methods should describe behavior, not implementation details")]
