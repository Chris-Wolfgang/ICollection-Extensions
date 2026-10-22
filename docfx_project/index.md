---
_layout: landing
---

# Wolfgang.Extensions.ICollection

## Essential Extension Methods for ICollection&lt;T&gt;

Wolfgang.Extensions.ICollection is a focused .NET library that extends the functionality of `ICollection<T>` with the bulk operations, presence checks, and conditional add/remove primitives the BCL leaves out. Every method works against the `ICollection<T>` interface, so the same call site works for `List<T>`, `HashSet<T>`, `LinkedList<T>`, `Collection<T>`, `ObservableCollection<T>`, and any custom implementation.

### Why Use This Library?

Many collection types implement `ICollection<T>` but lack convenient bulk operation methods. This library provides them while:

- **Working with any `ICollection<T>`** — `List<T>`, `HashSet<T>`, `LinkedList<T>`, `Collection<T>`, `ObservableCollection<T>`, custom implementations
- **Maintaining simplicity** — Small, focused API surface with zero runtime dependencies
- **Preserving type safety** — Generic methods that work with your specific types
- **Respecting the collection's contract** — Read-only / fixed-size collections throw `NotSupportedException` cleanly; set semantics flow through; self-aliasing is guarded against the BCL's mutate-during-enumerate contract

### Quick Example

```csharp
using System.Collections.Generic;
using Wolfgang.Extensions.ICollection;

ICollection<int> numbers = new List<int> { 1, 2, 3 };

numbers.AddRange(new[] { 4, 5, 6 });                       // → 1, 2, 3, 4, 5, 6
numbers.AddRangeIf(new[] { 7, 8, 9 }, n => n % 2 == 1);    // → +7, +9 (8 skipped)
numbers.RemoveRange(new[] { 1, 9 });                       // → 2, 3, 4, 5, 6, 7
int removed = numbers.RemoveWhere(n => n > 5);             // → 2
numbers.ReplaceAll(new[] { 10, 20, 30 });                  // → 10, 20, 30
numbers.AddIfNotContains(40);                              // → true
int added = numbers.AddIfNotContains(new[] { 40, 50 });    // → 1
numbers.IsEmpty();                                         // → false
numbers.IsNotEmpty();                                      // → true
```

### Get Started

- [Introduction](docs/introduction.md) — Learn about the library and its purpose
- [Getting Started](docs/getting-started.md) — Installation and first steps
- [API Documentation](api/index.html) — Complete API reference

### Installation

Install via NuGet Package Manager:

```bash
Install-Package Wolfgang.Extensions.ICollection
```

Or using .NET CLI:

```bash
dotnet add package Wolfgang.Extensions.ICollection
```

### Key Features

- ✅ **9 extension methods** — `AddRange`, `AddRangeIf`, `RemoveRange`, `RemoveWhere`, `ReplaceAll`, `AddIfNotContains` (single + bulk), `IsEmpty`, `IsNotEmpty`
- ✅ **Fast paths where it matters** — `List<T>` capacity pre-allocation for `AddRange`; `HashSet<T>.RemoveWhere` for `RemoveWhere`; `ISet<T>.Add` for `AddIfNotContains` (single lookup vs. `Contains` + `Add`)
- ✅ **Self-aliasing is safe** — `list.AddRange(list)`, `list.ReplaceAll(list)`, `list.RemoveRange(list)`, and `list.AddRangeIf(list, predicate)` all guard against the mutate-during-enumerate contract
- ✅ **Multi-framework support** — `netstandard2.0`, `netstandard2.1`, `net8.0`, `net9.0`, `net10.0`
- ✅ **Zero runtime dependencies** — Just the .NET BCL
- ✅ **Well documented** — Comprehensive XML documentation, examples, and CHANGELOG
- ✅ **Unit tested** — the suite runs on 13 test TFMs (.NET Framework 4.6.2 through .NET 10); CI enforces 90% line coverage

### License

MIT License — see [LICENSE](https://github.com/Chris-Wolfgang/ICollection-Extensions/blob/main/LICENSE) for details.
