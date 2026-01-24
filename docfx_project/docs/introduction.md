# Introduction

## Welcome to Wolfgang.Extensions.ICollection

Wolfgang.Extensions.ICollection is a focused .NET library that provides extension methods for the `ICollection<T>` interface. The library aims to fill gaps in the .NET BCL by providing convenient bulk operation methods that are missing from the base `ICollection<T>` interface.

## The Problem

While `List<T>` provides helpful methods like `AddRange()`, these methods are not available on the `ICollection<T>` interface that many collection types implement. This forces developers to either:

1. Write repetitive foreach loops to add multiple items
2. Cast collections to specific types (losing abstraction)
3. Create their own extension methods in every project

## The Solution

This library provides well-tested, documented extension methods that work with any `ICollection<T>` implementation, maintaining the abstraction level while providing the convenience of bulk operations.

## Core Philosophy

- **Simplicity First** - Small, focused API that does one thing well
- **Zero Dependencies** - No external dependencies to worry about
- **Type Safety** - Fully generic methods that preserve type information
- **Behavioral Consistency** - Respects the underlying collection's behavior and constraints

## Supported Collections

The extension methods work with any type implementing `ICollection<T>`, including:

- **List&lt;T&gt;** - Standard generic list
- **HashSet&lt;T&gt;** - Unordered set of unique elements
- **LinkedList&lt;T&gt;** - Doubly-linked list
- **Collection&lt;T&gt;** - Base class for generic collections
- **ObservableCollection&lt;T&gt;** - Collection with change notifications
- **Custom implementations** - Any class implementing ICollection&lt;T&gt;

## Target Frameworks

Wolfgang.Extensions.ICollection supports multiple .NET platforms:

- **.NET Framework 4.6.2**
- **.NET Standard 2.0** (provides compatibility with .NET Core 2.0+, .NET 5+, and newer .NET Framework versions)
- **.NET 8.0**
- **.NET 10.0**

This broad compatibility ensures the library can be used in virtually any .NET project, from legacy applications to cutting-edge solutions.

## What's Included

### AddRange Method

The primary extension method provided by this library is `AddRange<T>()`, which allows adding multiple items to any `ICollection<T>` in a single operation.

**Features:**
- Null-safe (throws ArgumentNullException for null parameters)
- Works with any IEnumerable&lt;T&gt; source
- Preserves collection-specific behaviors (e.g., HashSet deduplication)
- Handles empty enumerables gracefully
- Fully documented with XML comments and examples

## Next Steps

- **[Getting Started](getting-started.md)** - Install the package and write your first code
- **[API Documentation](../api/index.html)** - Explore the complete API reference
- **[GitHub Repository](https://github.com/Chris-Wolfgang/ICollection-Extensions)** - View source code and contribute

## License

This project is licensed under the MIT License, making it free to use in both open-source and commercial projects.
