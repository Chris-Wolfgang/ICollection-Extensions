# Introduction

## Welcome to Wolfgang.Extensions.ICollection

Wolfgang.Extensions.ICollection is a focused .NET library that provides extension methods for the `ICollection<T>` interface. The library aims to fill gaps in the .NET BCL by providing convenient bulk operation methods, presence checks, and conditional add/remove primitives that are missing from the base `ICollection<T>` interface.

## The Problem

While `List<T>` provides helpful methods like `AddRange()`, these methods are not available on the `ICollection<T>` interface that many collection types implement. This forces developers to either:

1. Write repetitive `foreach` loops to add, remove, filter, or test multiple items
2. Cast collections to specific types (losing abstraction)
3. Create their own extension methods in every project

## The Solution

This library provides well-tested, documented extension methods that work with any `ICollection<T>` implementation, maintaining the abstraction level while providing the convenience of bulk operations.

## Core Philosophy

- **Simplicity First** — Small, focused API that does the things the BCL leaves out
- **Zero Dependencies** — No external runtime dependencies
- **Type Safety** — Fully generic methods that preserve type information
- **Behavioral Consistency** — Respects the underlying collection's behavior and constraints; uses native fast paths (`HashSet<T>.RemoveWhere`, `ISet<T>.Add`, `List<T>` capacity pre-allocation) when they exist

## Supported Collections

The extension methods work with any type implementing `ICollection<T>`, including:

- **`List<T>`** — Standard generic list (and the target of the `AddRange` capacity pre-allocation fast path)
- **`HashSet<T>`** — Unordered set of unique elements (used by the `RemoveWhere` and `AddIfNotContains` fast paths)
- **`SortedSet<T>`** — Sorted set (also covered by the `ISet<T>` fast path for `AddIfNotContains`)
- **`LinkedList<T>`** — Doubly-linked list
- **`Collection<T>`** — Base class for generic collections
- **`ObservableCollection<T>`** — Collection with change notifications
- **Arrays** — `T[]` implements `ICollection<T>` (read-only / fixed-size); query methods (`IsEmpty`, `IsNotEmpty`) work; mutating methods throw `NotSupportedException` cleanly
- **Custom implementations** — Any class implementing `ICollection<T>`

## Target Frameworks

Wolfgang.Extensions.ICollection supports a broad set of modern .NET TFMs:

- **`.NET Standard 2.0`** — covers `.NET Framework 4.6.1+`, `.NET Core 2.0+`, Mono, Xamarin
- **`.NET Standard 2.1`** — picks up the BCL improvements available since 2019
- **`.NET 8.0`** — current LTS
- **`.NET 9.0`** — latest released runtime
- **`.NET 10.0`** — active LTS branch

## What's Included

### Bulk additions

- **`AddRange(this ICollection<T>, IEnumerable<T>)`** — appends every item; pre-allocates `List<T>` capacity when the source exposes `Count`.
- **`AddRangeIf(this ICollection<T>, IEnumerable<T>, Func<T, bool>)`** — appends items that satisfy a predicate.
- **`AddIfNotContains(this ICollection<T>, T) -> bool`** — adds a single item only if it's not already present; returns whether the add happened. Uses `ISet<T>.Add` as a single-lookup fast path for set consumers.
- **`AddIfNotContains(this ICollection<T>, IEnumerable<T>) -> int`** — bulk overload; returns the count actually added.

### Bulk removals

- **`RemoveRange(this ICollection<T>, IEnumerable<T>)`** — removes one occurrence of each listed item.
- **`RemoveWhere(this ICollection<T>, Func<T, bool>) -> int`** — removes every item matching the predicate; returns the count removed. Uses `HashSet<T>.RemoveWhere` as a fast path for `HashSet<T>` consumers; safe for every other `ICollection<T>`.
- **`ReplaceAll(this ICollection<T>, IEnumerable<T>)`** — clears the collection then appends every item from the new sequence.

### Presence checks

- **`IsEmpty(this ICollection<T>) -> bool`** — `true` when `Count == 0`.
- **`IsNotEmpty(this ICollection<T>) -> bool`** — `true` when `Count > 0`.

**Features common to every method:**

- Null-safe (throws `ArgumentNullException` for null parameters)
- Self-aliasing safe (`list.AddRange(list)` and friends are guarded against the BCL's mutate-during-enumerate contract)
- Read-only-aware (throws `NotSupportedException` for read-only or fixed-size collections, surfacing the underlying type's own contract)
- Fully documented with XML comments and examples

## Next Steps

- **[Getting Started](getting-started.md)** — Install the package and write your first code
- **[API Documentation](../api/index.html)** — Explore the complete API reference
- **[GitHub Repository](https://github.com/Chris-Wolfgang/ICollection-Extensions)** — View source code and contribute

## License

This project is licensed under the MIT License, making it free to use in both open-source and commercial projects.
