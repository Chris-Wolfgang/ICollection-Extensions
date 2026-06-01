# Getting Started

This guide will help you install and start using Wolfgang.Extensions.ICollection in your .NET projects.

## Installation

### Using NuGet Package Manager (Visual Studio)

1. Open your project in Visual Studio
2. Right-click on your project in Solution Explorer
3. Select "Manage NuGet Packages..."
4. Search for "Wolfgang.Extensions.ICollection"
5. Click "Install"

### Using Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.ICollection
```

### Using .NET CLI

```bash
dotnet add package Wolfgang.Extensions.ICollection
```

### Manual Package Reference

Add this to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Wolfgang.Extensions.ICollection" Version="x.x.x" />
</ItemGroup>
```

## Basic Usage

### 1. Add the Using Directive

First, add the namespace to your C# file:

```csharp
using Wolfgang.Extensions.ICollection;
```

### 2. Bulk Additions

`AddRange` works on any `ICollection<T>` — `List<T>`, `HashSet<T>`, `LinkedList<T>`, `Collection<T>`, `ObservableCollection<T>`, and any custom implementation:

```csharp
ICollection<string> names = new List<string>();
names.AddRange(new[] { "Alice", "Bob", "Charlie" });

ICollection<int> numbers = new HashSet<int>();
numbers.AddRange(new[] { 1, 2, 3, 4, 5 });    // HashSet dedups → unique items only

ICollection<double> values = new LinkedList<double>();
values.AddRange(new[] { 1.5, 2.5, 3.5 });
```

For predicate-gated bulk add, use `AddRangeIf`:

```csharp
ICollection<int> evens = new List<int>();
evens.AddRangeIf(Enumerable.Range(1, 20), n => n % 2 == 0);   // → 2, 4, …, 20
```

For "add only if not already present", use `AddIfNotContains`:

```csharp
ICollection<int> seen = new List<int> { 1, 2, 3 };
bool addedThree = seen.AddIfNotContains(3);   // → false (already present)
bool addedFour  = seen.AddIfNotContains(4);   // → true; collection now { 1, 2, 3, 4 }
int  addedMany  = seen.AddIfNotContains(new[] { 4, 5, 6 });   // → 2 (only 5 and 6 were new)
```

`HashSet<T>` and `SortedSet<T>` get a single-lookup fast path that uses `ISet<T>.Add` directly (and honours the set's own `IEqualityComparer<T>`).

### 3. Bulk Removals

```csharp
ICollection<int> nums = new List<int> { 1, 2, 3, 4, 5, 6 };
nums.RemoveRange(new[] { 1, 6 });               // → 2, 3, 4, 5
int removed = nums.RemoveWhere(n => n > 3);     // → 2 (collection now { 2, 3 })
nums.ReplaceAll(new[] { 10, 20, 30 });          // → 10, 20, 30
```

`RemoveWhere` uses `HashSet<T>.RemoveWhere` as a fast path for `HashSet<T>` consumers; other implementations get a safe materialise-then-mutate pattern that never trips the BCL's mutate-during-enumerate contract.

### 4. Presence Checks

Use `IsEmpty()` and `IsNotEmpty()` for cleaner, more readable emptiness checks:

```csharp
ICollection<string> names = new List<string>();

// Instead of: if (names.Count == 0)
if (names.IsEmpty())
{
    Console.WriteLine("No names found.");
}

names.Add("Alice");

// Instead of: if (names.Count > 0)
if (names.IsNotEmpty())
{
    Console.WriteLine($"Found {names.Count} name(s).");
}
```

> **Readability note:** `IsEmpty()` and `IsNotEmpty()` use `ICollection<T>.Count` directly (typically `O(1)` for standard `ICollection<T>` implementations), providing a clearer, self-documenting alternative to `!Enumerable.Any()`.

## Common Scenarios

### Scenario 1: Adding Items from Multiple Sources

```csharp
using Wolfgang.Extensions.ICollection;

public class DataAggregator
{
    public ICollection<Product> GetAllProducts()
    {
        ICollection<Product> allProducts = new List<Product>();

        // Add products from different sources
        allProducts.AddRange(GetDatabaseProducts());
        allProducts.AddRange(GetCachedProducts());
        allProducts.AddRange(GetApiProducts());

        return allProducts;
    }

    private IEnumerable<Product> GetDatabaseProducts() { /* ... */ }
    private IEnumerable<Product> GetCachedProducts() { /* ... */ }
    private IEnumerable<Product> GetApiProducts() { /* ... */ }
}
```

### Scenario 2: Working with LINQ Results

```csharp
using Wolfgang.Extensions.ICollection;
using System.Linq;

public class NumberProcessor
{
    public ICollection<int> GetEvenNumbers(int max)
    {
        ICollection<int> result = new List<int>();
        result.AddRangeIf(Enumerable.Range(1, max), n => n % 2 == 0);
        return result;
    }
}
```

### Scenario 3: Building Collections Incrementally

```csharp
using Wolfgang.Extensions.ICollection;

public class ReportBuilder
{
    private ICollection<string> _reportLines = new List<string>();

    public void AddHeader()
    {
        _reportLines.AddRange(new[]
        {
            "===================",
            "  Monthly Report   ",
            "==================="
        });
    }

    public void AddSection(string title, IEnumerable<string> data)
    {
        _reportLines.AddRange(new[] { "", $"## {title}", "" });
        _reportLines.AddRange(data);
    }

    public IEnumerable<string> GetReport() => _reportLines;
}
```

### Scenario 4: Set-Aware Deduplication

```csharp
using Wolfgang.Extensions.ICollection;

public class TagAggregator
{
    public ICollection<string> CollectUniqueTags(IEnumerable<IEnumerable<string>> tagLists)
    {
        // HashSet<T> typed as ICollection<T> so the extension binds AND
        // the ISet<T>.Add fast path kicks in for AddIfNotContains.
        ICollection<string> unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tags in tagLists)
        {
            unique.AddIfNotContains(tags);
        }
        return unique;
    }
}
```

### Scenario 5: Replacing the Contents of an Observable Collection

```csharp
using System.Collections.ObjectModel;
using Wolfgang.Extensions.ICollection;

public class ViewModel
{
    public ObservableCollection<string> Items { get; } = new();

    public void Refresh(IEnumerable<string> newItems)
    {
        // ReplaceAll: clear + re-populate. The UI binding sees the full
        // turnover (one Reset + one Add-per-item, or one Add range
        // depending on the implementation).
        ICollection<string> bindable = Items;
        bindable.ReplaceAll(newItems);
    }
}
```

## Error Handling

All methods perform argument validation and surface the target collection's own contract:

```csharp
using Wolfgang.Extensions.ICollection;
using System.Collections.ObjectModel;

// ArgumentNullException if source is null
ICollection<string>? nullCollection = null;
nullCollection!.AddRange(new[] { "item" });   // Throws ArgumentNullException("source")

// ArgumentNullException if items is null
ICollection<string> collection = new List<string>();
collection.AddRange(items: null!);             // Throws ArgumentNullException("items")

// NotSupportedException if collection is read-only (ReadOnlyCollection<T>, T[], etc.)
ICollection<int> readOnly = new ReadOnlyCollection<int>(new[] { 1, 2, 3 });
readOnly.AddRange(new[] { 4, 5 });             // Throws NotSupportedException

ICollection<int> array = new int[5];
array.RemoveRange(new[] { 1 });                // Same — arrays are fixed-size

// ArgumentNullException if predicate is null (AddRangeIf / RemoveWhere)
collection.RemoveWhere(predicate: null!);      // Throws ArgumentNullException("predicate")
```

## Important Considerations

### Thread Safety

The extension methods are **not thread-safe**. If you need to access collections from multiple threads:

```csharp
using Wolfgang.Extensions.ICollection;

ICollection<int> collection = new List<int>();
var lockObject = new object();

lock (lockObject)
{
    collection.AddRange(itemsFromThreadA);
}

lock (lockObject)
{
    collection.AddRange(itemsFromThreadB);
}
```

### Self-Aliasing

Passing the same collection as both `source` and `items` (e.g. `list.AddRange(list)`) is safe for every mutating method that takes an `IEnumerable<T>` — each method snapshots `items` via `ReferenceEquals` before mutating, so the call doesn't trip the BCL's mutate-during-enumerate contract:

- `list.AddRange(list)` — appends a copy of the current contents
- `list.AddRangeIf(list, pred)` — appends the items in the current contents that satisfy the predicate
- `list.RemoveRange(list)` — empties the collection
- `list.ReplaceAll(list)` — no-op (snapshot is restored after `Clear`)

### Performance

The methods include fast paths where it matters:

- **`AddRange`** pre-allocates `List<T>.Capacity` when the source exposes `Count`, avoiding the amortised-doubling growth path
- **`RemoveWhere`** delegates to `HashSet<T>.RemoveWhere(Predicate<T>)` when the receiver is a `HashSet<T>` (skips the temp-list allocation)
- **`AddIfNotContains(T)`** delegates to `ISet<T>.Add` when the receiver is any `ISet<T>` (single lookup instead of `Contains` + `Add`)

Otherwise the cost is the obvious shape: O(n) over the input, with each per-item cost matching the target collection's own `Add` / `Remove` / `Contains`.

### Empty Collections

Adding an empty enumerable is safe and does nothing:

```csharp
ICollection<string> collection = new List<string> { "existing" };
collection.AddRange(Enumerable.Empty<string>());   // No exception, collection unchanged
```

## Next Steps

- **[API Documentation](../api/index.html)** — Explore detailed API documentation
- **[Introduction](introduction.md)** — Learn more about the library's design
- **[GitHub Repository](https://github.com/Chris-Wolfgang/ICollection-Extensions)** — View source code and examples

## Getting Help

If you encounter issues or have questions:

1. Check the [GitHub Issues](https://github.com/Chris-Wolfgang/ICollection-Extensions/issues) for existing discussions
2. Review the [API Documentation](../api/index.html) for detailed method information
3. Open a new issue if you've found a bug or have a feature request
