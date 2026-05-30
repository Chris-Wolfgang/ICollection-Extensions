# Wolfgang.Extensions.ICollection

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.ICollection.svg?logo=nuget)](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection/)
[![Downloads](https://img.shields.io/nuget/dt/Wolfgang.Extensions.ICollection.svg?logo=nuget)](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection/)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/ICollection-Extensions/pr.yaml?label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/ICollection-Extensions/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/ICollection-Extensions/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/ICollection-Extensions/actions/workflows/release.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-blueviolet?logo=dotnet)](#-target-frameworks)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/ICollection-Extensions)

A lightweight .NET library providing essential extension methods for types implementing the `ICollection<T>` interface. This library brings the convenience of `List<T>.AddRange()` to all collection types.

## 🎯 Purpose

Many collection types in .NET implement `ICollection<T>` but lack convenient bulk operation methods like `AddRange`. This library fills that gap by providing extension methods that work with any `ICollection<T>` implementation, including:

- `List<T>`
- `HashSet<T>`
- `LinkedList<T>`
- `Collection<T>`
- `ObservableCollection<T>`
- Custom collection implementations

## 📦 Installation

### NuGet Package Manager

```bash
Install-Package Wolfgang.Extensions.ICollection
```

### .NET CLI

```bash
dotnet add package Wolfgang.Extensions.ICollection
```

### Package Reference

```xml
<PackageReference Include="Wolfgang.Extensions.ICollection" Version="x.x.x" />
```

## 🚀 Usage

### Basic Example

```csharp
using Wolfgang.Extensions.ICollection;

// Add multiple items to any ICollection<T>
ICollection<string> names = new List<string> { "Alice" };
names.AddRange(new[] { "Bob", "Charlie", "David" });
// names now contains: "Alice", "Bob", "Charlie", "David"
```

### Working with Different Collection Types

```csharp
using Wolfgang.Extensions.ICollection;

// Works with HashSet
ICollection<int> uniqueNumbers = new HashSet<int> { 1, 2, 3 };
uniqueNumbers.AddRange(new[] { 4, 5, 6 });
// uniqueNumbers now contains: 1, 2, 3, 4, 5, 6

// Works with LinkedList
ICollection<string> linkedList = new LinkedList<string>();
linkedList.AddRange(new[] { "First", "Second", "Third" });

// Works with ObservableCollection
ICollection<int> observable = new ObservableCollection<int>();
observable.AddRange(Enumerable.Range(1, 5));
```

### Adding Items from LINQ Queries

```csharp
using Wolfgang.Extensions.ICollection;

// Add filtered results from LINQ
var evenNumbers = Enumerable.Range(1, 20).Where(n => n % 2 == 0);
ICollection<int> collection = new List<int>();
collection.AddRange(evenNumbers);
// collection now contains: 2, 4, 6, 8, 10, 12, 14, 16, 18, 20
```

### Practical Scenarios

```csharp
using Wolfgang.Extensions.ICollection;

// Combining multiple sources
ICollection<string> allItems = new List<string>();
allItems.AddRange(GetItemsFromDatabase());
allItems.AddRange(GetItemsFromCache());
allItems.AddRange(GetItemsFromApi());

// Building collections incrementally
ICollection<Product> products = new HashSet<Product>();
products.AddRange(featuredProducts);
products.AddRange(recommendedProducts);
products.AddRange(newProducts);
```

## 📚 API Reference

| Method | Returns | Description |
|---|---|---|
| `AddRange<T>(this ICollection<T>, IEnumerable<T>)` | `void` | Appends every item in the sequence to the collection. Pre-allocates capacity when the target is `List<T>` and the appended sequence exposes `ICollection<T>.Count`. |
| `AddRangeIf<T>(this ICollection<T>, IEnumerable<T>, Func<T, bool>)` | `void` | Appends items for which the predicate returns `true`. |
| `RemoveRange<T>(this ICollection<T>, IEnumerable<T>)` | `void` | Removes one occurrence of each listed item from the collection. |
| `RemoveWhere<T>(this ICollection<T>, Func<T, bool>)` | `int` | Removes every item matching the predicate; returns the count removed. Safe for any `ICollection<T>`. |
| `ReplaceAll<T>(this ICollection<T>, IEnumerable<T>)` | `void` | Clears the collection then appends every item from the new sequence. |
| `AddIfNotContains<T>(this ICollection<T>, T)` | `bool` | Adds the item only if it is not already present; returns whether the add happened. |
| `AddIfNotContains<T>(this ICollection<T>, IEnumerable<T>)` | `int` | Bulk overload; returns the count actually added. |
| `IsEmpty<T>(this ICollection<T>)` | `bool` | `true` if the collection has zero items, otherwise `false`. |
| `IsNotEmpty<T>(this ICollection<T>)` | `bool` | `true` if the collection has at least one item, otherwise `false`. |

### `AddRange<T>`

```csharp
public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
```

Adds every item from `items` to `source`.

**Throws:**
- `ArgumentNullException` if `source` or `items` is `null`
- `NotSupportedException` if `source` is read-only

**Behavior:**
- Empty `items` is a no-op (no exception)
- Duplicate handling follows the target collection's semantics (e.g.,
  `HashSet<T>` silently ignores duplicates)
- Pre-allocates capacity when `source` is `List<T>` and `items`
  exposes `ICollection<T>.Count` (the common batch-append fast path)
- Not thread-safe — external synchronization required for concurrent
  access

### `IsEmpty<T>` / `IsNotEmpty<T>`

```csharp
public static bool IsEmpty<T>(this ICollection<T> source)
public static bool IsNotEmpty<T>(this ICollection<T> source)
```

Reads `source.Count` and compares to zero. Typically `O(1)` for standard
`ICollection<T>` implementations.

**Throws:**
- `ArgumentNullException` if `source` is `null`

## 🔧 Target Frameworks

| TFM | Use case |
|---|---|
| `netstandard2.0` | Broad consumer compatibility (covers .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+, Mono, Xamarin) |
| `netstandard2.1` | Picks up the BCL improvements available since 2019 |
| `net8.0` | Current LTS |
| `net9.0` | Latest released runtime |
| `net10.0` | Active LTS branch |

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Chris Wolfgang**

- GitHub: [@Chris-Wolfgang](https://github.com/Chris-Wolfgang)

## 🔗 Links

- [Documentation](https://chris-wolfgang.github.io/ICollection-Extensions/)
- [NuGet Package](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection/)
- [Source Code](https://github.com/Chris-Wolfgang/ICollection-Extensions)
- [Report Issues](https://github.com/Chris-Wolfgang/ICollection-Extensions/issues)
