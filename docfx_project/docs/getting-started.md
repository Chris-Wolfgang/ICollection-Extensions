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

### 2. Use AddRange on Any ICollection&lt;T&gt;

Now you can use `AddRange()` on any collection implementing `ICollection<T>`:

```csharp
// With List<T>
ICollection<string> names = new List<string>();
names.AddRange(new[] { "Alice", "Bob", "Charlie" });

// With HashSet<T>
ICollection<int> numbers = new HashSet<int>();
numbers.AddRange(new[] { 1, 2, 3, 4, 5 });

// With LinkedList<T>
ICollection<double> values = new LinkedList<double>();
values.AddRange(new[] { 1.5, 2.5, 3.5 });
```

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
        var evenNumbers = Enumerable.Range(1, max)
                                   .Where(n => n % 2 == 0);
        
        ICollection<int> result = new List<int>();
        result.AddRange(evenNumbers);
        
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

### Scenario 4: Using with Different Collection Types

```csharp
using Wolfgang.Extensions.ICollection;
using System.Collections.ObjectModel;

public class CollectionExamples
{
    public void DemonstrateWithHashSet()
    {
        // HashSet automatically handles duplicates
        ICollection<int> uniqueNumbers = new HashSet<int> { 1, 2, 3 };
        uniqueNumbers.AddRange(new[] { 3, 4, 5, 5 }); // Duplicates ignored
        // Result: 1, 2, 3, 4, 5
    }
    
    public void DemonstrateWithObservableCollection()
    {
        // ObservableCollection raises change notifications
        var observable = new ObservableCollection<string>();
        observable.CollectionChanged += (s, e) => 
            Console.WriteLine($"Items added: {e.NewItems?.Count ?? 0}");
        
        ICollection<string> collection = observable;
        collection.AddRange(new[] { "A", "B", "C" });
        // Notifications are raised for each item
    }
}
```

## Error Handling

The `AddRange` method performs validation and throws exceptions in these cases:

```csharp
using Wolfgang.Extensions.ICollection;

// ArgumentNullException if source is null
ICollection<string> nullCollection = null;
nullCollection.AddRange(new[] { "item" }); // Throws ArgumentNullException

// ArgumentNullException if items is null
ICollection<string> collection = new List<string>();
collection.AddRange(null); // Throws ArgumentNullException

// NotSupportedException if collection is read-only
ICollection<int> readOnly = new ReadOnlyCollection<int>(new[] { 1, 2, 3 });
readOnly.AddRange(new[] { 4, 5 }); // Throws NotSupportedException
```

## Important Considerations

### Thread Safety

The `AddRange` method is **not thread-safe**. If you need to add items from multiple threads:

```csharp
using Wolfgang.Extensions.ICollection;

ICollection<int> collection = new List<int>();
var lockObject = new object();

// Thread-safe usage
lock (lockObject)
{
    collection.AddRange(itemsFromThreadA);
}

lock (lockObject)
{
    collection.AddRange(itemsFromThreadB);
}
```

### Performance

For very large collections, consider the performance characteristics:

```csharp
using Wolfgang.Extensions.ICollection;

// For List<T>, this is efficient
ICollection<int> list = new List<int>(capacity: 10000);
list.AddRange(Enumerable.Range(1, 10000)); // O(n)

// For HashSet<T>, duplicate checking occurs for each item
ICollection<int> hashSet = new HashSet<int>();
hashSet.AddRange(Enumerable.Range(1, 10000)); // O(n) with hash checks
```

### Empty Collections

Adding an empty enumerable is safe and does nothing:

```csharp
using Wolfgang.Extensions.ICollection;

ICollection<string> collection = new List<string> { "existing" };
collection.AddRange(Enumerable.Empty<string>()); // No exception, collection unchanged
```

## Next Steps

- **[API Documentation](../api/index.html)** - Explore detailed API documentation
- **[Introduction](introduction.md)** - Learn more about the library's design
- **[GitHub Repository](https://github.com/Chris-Wolfgang/ICollection-Extensions)** - View source code and examples

## Getting Help

If you encounter issues or have questions:

1. Check the [GitHub Issues](https://github.com/Chris-Wolfgang/ICollection-Extensions/issues) for existing discussions
2. Review the [API Documentation](../api/index.html) for detailed method information
3. Open a new issue if you've found a bug or have a feature request
