---
_layout: landing
---

# Wolfgang.Extensions.ICollection

## Essential Extension Methods for ICollection&lt;T&gt;

Wolfgang.Extensions.ICollection is a lightweight .NET library that extends the functionality of `ICollection<T>` with convenient bulk operation methods. The library brings the convenience of `List<T>.AddRange()` to all collection types that implement `ICollection<T>`.

### Why Use This Library?

Many collection types in .NET implement `ICollection<T>` but lack convenient bulk operation methods. This library provides these missing operations while:

- **Working with any ICollection&lt;T&gt;** - List, HashSet, LinkedList, ObservableCollection, and more
- **Maintaining simplicity** - Small, focused API surface with zero dependencies
- **Preserving type safety** - Generic methods that work with your specific types
- **Respecting constraints** - Honors the behavior of the underlying collection implementation

### Quick Example

```csharp
using Wolfgang.Extensions.ICollection;

ICollection<string> names = new HashSet<string>();
names.AddRange(new[] { "Alice", "Bob", "Charlie" });
// Works with any ICollection<T> implementation!
```

### Get Started

- [Introduction](docs/introduction.md) - Learn about the library and its purpose
- [Getting Started](docs/getting-started.md) - Installation and first steps
- [API Documentation](api/index.html) - Complete API reference

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

- ✅ **AddRange** - Add multiple items to any ICollection&lt;T&gt; in one call
- ✅ **Multi-framework support** - .NET Framework 4.6.2, .NET Standard 2.0, .NET 8.0
- ✅ **Zero dependencies** - No external dependencies
- ✅ **Well documented** - Comprehensive XML documentation and examples
- ✅ **Tested** - Full unit test coverage

### License

MIT License - see [LICENSE](https://github.com/Chris-Wolfgang/ICollection-Extensions/blob/main/LICENSE) for details.
