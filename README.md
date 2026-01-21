# Wolfgang.Extensions.ICollection

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.ICollection.svg)](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

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
<PackageReference Include="Wolfgang.Extensions.ICollection" Version="0.1.0" />
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

### AddRange Method

```csharp
public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
```

Adds all items from the specified enumerable to the source collection.

**Parameters:**
- `source` - The collection to add items to (must not be null)
- `items` - The items to add (must not be null)

**Type Parameters:**
- `T` - The type of items in the collection

**Exceptions:**
- `ArgumentNullException` - Thrown if `source` or `items` is null
- `NotSupportedException` - Thrown if the collection is read-only

**Behavior Notes:**
- Empty enumerables are handled gracefully (no exception, no items added)
- Duplicate handling depends on the collection implementation (e.g., HashSet ignores duplicates)
- Not thread-safe; external synchronization required for concurrent access
- Preserves the collection's native constraints and behaviors

## 🔧 Requirements

- **.NET Framework 4.6.2** or higher
- **.NET Standard 2.0** or higher
- **.NET 8.0** or higher
- **.NET 10.0** or higher

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


