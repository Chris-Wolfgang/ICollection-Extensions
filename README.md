# Wolfgang.Extensions.ICollection

A collection of extension methods for types that implement `ICollection<T>` in .Net

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.ICollection.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection)
[![NuGet downloads](https://img.shields.io/nuget/dt/Wolfgang.Extensions.ICollection.svg?logo=nuget&label=downloads)](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/ICollection-Extensions/pr.yaml?event=pull_request_target&label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/ICollection-Extensions/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/ICollection-Extensions/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/ICollection-Extensions/actions/workflows/release.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/ICollection-Extensions)

---

## 📦 Installation

```bash
dotnet add package Wolfgang.Extensions.ICollection
```

**NuGet Package:** [Wolfgang.Extensions.ICollection](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection)

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/ICollection-Extensions](https://github.com/Chris-Wolfgang/ICollection-Extensions)
- **API Documentation:** https://Chris-Wolfgang.github.io/ICollection-Extensions/
- **CHANGELOG:** [CHANGELOG.md](CHANGELOG.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **Formatting Guide:** [docs/README-FORMATTING.md](docs/README-FORMATTING.md)
- **Release Workflow Setup:** [docs/RELEASE-WORKFLOW-SETUP.md](docs/RELEASE-WORKFLOW-SETUP.md)
- **Workflow Security:** [docs/WORKFLOW_SECURITY.md](docs/WORKFLOW_SECURITY.md)

---

## 🚀 Quick Start

```csharp
using System.Collections.Generic;       // ICollection<T>, List<T>, HashSet<T>
using Wolfgang.Extensions.ICollection;

ICollection<int> numbers = new List<int> { 1, 2, 3 };

numbers.AddRange(new[] { 4, 5, 6 });                  // → 1, 2, 3, 4, 5, 6
numbers.AddRangeIf(new[] { 7, 8, 9 }, n => n % 2 == 1); // → +7, +9 (8 skipped)
numbers.RemoveRange(new[] { 1, 9 });                  // → 2, 3, 4, 5, 6, 7
numbers.RemoveWhere(n => n > 5);                       // → 2 removed
numbers.ReplaceAll(new[] { 10, 20, 30 });              // → 10, 20, 30

numbers.AddIfNotContains(10);                          // → false (already present)
numbers.AddIfNotContains(40);                          // → true; appended
numbers.AddIfNotContains(new[] { 40, 50 });            // → 1 (only 50 was new)

numbers.IsEmpty();                                     // → false
numbers.IsNotEmpty();                                  // → true
```

Every method works against the `ICollection<T>` contract, so it transparently supports `List<T>`, `HashSet<T>`, `LinkedList<T>`, `Collection<T>`, `ObservableCollection<T>`, and any custom implementation. Self-aliasing (`list.AddRange(list)`, `list.ReplaceAll(list)`, etc.) is guarded so it never trips the BCL's mutate-during-enumerate contract.

---

## ✨ Features

The table below is a snapshot of the public API at the time of writing. For the
authoritative list (kept in sync with source on every release), see the
[API documentation](https://Chris-Wolfgang.github.io/ICollection-Extensions/api/Wolfgang.Extensions.ICollection.ICollectionExtensions.html).

### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `AddRange<T>(this ICollection<T>, IEnumerable<T>)` | `void` | Appends every item from the sequence to the collection. Pre-allocates capacity when the target is `List<T>` and the appended sequence exposes `ICollection<T>.Count`. |
| `AddRangeIf<T>(this ICollection<T>, IEnumerable<T>, Func<T, bool>)` | `void` | Appends only the items for which the predicate returns `true`. |
| `RemoveRange<T>(this ICollection<T>, IEnumerable<T>)` | `void` | Removes one occurrence of each listed item. |
| `RemoveWhere<T>(this ICollection<T>, Func<T, bool>)` | `int` | Removes every item matching the predicate; returns the count removed. Uses `HashSet<T>.RemoveWhere` as a fast path for `HashSet<T>` consumers; safe for every other `ICollection<T>`. |
| `ReplaceAll<T>(this ICollection<T>, IEnumerable<T>)` | `void` | Clears the collection then appends every item from the new sequence. |
| `AddIfNotContains<T>(this ICollection<T>, T)` | `bool` | Adds the item only if it isn't already present; returns whether the add happened. Uses `ISet<T>.Add` as a fast path for set consumers (single lookup). |
| `AddIfNotContains<T>(this ICollection<T>, IEnumerable<T>)` | `int` | Bulk overload; returns the count actually added. |
| `IsEmpty<T>(this ICollection<T>)` | `bool` | `true` when `Count == 0`. Typically `O(1)` for standard `ICollection<T>` implementations. |
| `IsNotEmpty<T>(this ICollection<T>)` | `bool` | `true` when `Count > 0`. |

### Examples

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Wolfgang.Extensions.ICollection;

// Works on any ICollection<T>, not just List<T>
ICollection<string> set = new HashSet<string>(StringComparer.Ordinal) { "Alice" };
set.AddRange(new[] { "Bob", "Alice", "Carol" });   // HashSet dedups → 3 items

ICollection<int> linked = new LinkedList<int>();
linked.AddRange(Enumerable.Range(1, 5));           // → 1..5

// Predicate-gated bulk operations
ICollection<int> evens = new List<int>();
evens.AddRangeIf(Enumerable.Range(1, 20), n => n % 2 == 0); // → 2, 4, …, 20
int removed = evens.RemoveWhere(n => n > 10);                // → 5

// Set-aware AddIfNotContains
int added = set.AddIfNotContains(new[] { "Bob", "Dave", "Erin" }); // → 2

// Array as a source: int[] implements ICollection<int>
int[] buffer = { 7, 8, 9 };
buffer.IsEmpty();   // → false
// buffer.AddRange(...) would throw NotSupportedException — arrays are fixed-size.
```

Self-aliasing is safe by construction: `list.AddRange(list)` snapshots the source before mutating; `list.ReplaceAll(list)` is a no-op; `list.RemoveRange(list)` empties the collection cleanly.

---

## 🎯 Supported Frameworks

This library targets:

- **.NET Standard:** 2.0

See the [NuGet package page](https://www.nuget.org/packages/Wolfgang.Extensions.ICollection/) for the authoritative per-TFM compatibility matrix.

## 🔍 Code Quality & Static Analysis

This project enforces **strict code quality standards** through **8 analyzer rule sets** (7 explicit `PackageReference`s plus the .NET SDK's built-in `Microsoft.CodeAnalysis.NetAnalyzers`), an `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` Release gate, and custom async-first rules:

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** — Built-in .NET analyzers for correctness and performance
2. **Roslynator.Analyzers** — Advanced refactoring and code quality rules
3. **AsyncFixer** — Async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** — Thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** — Prevents usage of banned synchronous APIs (see `BannedSymbols.txt`)
6. **Meziantou.Analyzer** — Comprehensive code quality rules
7. **SonarAnalyzer.CSharp** — Industry-standard code analysis
8. **Microsoft.CodeAnalysis.PublicApiAnalyzers** — Tracks the public API surface via `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt`; surfaces additions/removals at compile time as a breaking-change review gate


---

## 🛠️ Building from Source

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) — required for the modern build / test / pack flow used by CI
- Optional: [PowerShell Core](https://github.com/PowerShell/PowerShell) for formatting scripts

### Build Steps

```bash
# Clone the repository
git clone https://github.com/Chris-Wolfgang/ICollection-Extensions.git
cd ICollection-Extensions

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Run code formatting (PowerShell Core)
pwsh ./scripts/format.ps1
```

### Code Formatting

This project uses `.editorconfig` and `dotnet format`:

```bash
# Format code
dotnet format

# Verify formatting (as CI does)
dotnet format --verify-no-changes
```

See [docs/README-FORMATTING.md](docs/README-FORMATTING.md) for detailed formatting guidelines.

### Building Documentation

This project uses [DocFX](https://dotnet.github.io/docfx/) to generate API documentation:

```bash
# Install DocFX (one-time setup)
dotnet tool install -g docfx

# Generate API metadata and build documentation
cd docfx_project
docfx metadata  # Extract API metadata from source code
docfx build     # Build HTML documentation

# Documentation is generated in the docs/ folder at the repository root
```

The documentation is automatically built and deployed to GitHub Pages when changes are pushed to the `main` branch.

**Local Preview:**
```bash
# Serve documentation locally (with live reload)
cd docfx_project
docfx build --serve

# Open http://localhost:8080 in your browser
```

**Documentation Structure:**
- `docfx_project/` - DocFX configuration and source files
- `docs/` - Generated HTML documentation (published to GitHub Pages)
- `docfx_project/index.md` - Main landing page content
- `docfx_project/docs/` - Additional documentation articles
- `docfx_project/api/` - Auto-generated API reference YAML files

---


## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code quality standards
- Build and test instructions
- Pull request guidelines
- Analyzer configuration details
