# Code Formatting Guidelines

This document describes the code formatting rules enforced in this repository via `.editorconfig` and Roslyn analyzers.

---

## General Rules (All Files)

| Setting | Value |
|--------|-------|
| Charset | UTF-8 |
| Line endings | LF (Unix-style) |
| Indentation | Spaces |
| Indent size | 4 |
| Insert final newline | Yes |
| Trim trailing whitespace | Yes |

### File-Type Overrides

| File Type | Indent Size | Line Endings |
|-----------|------------|--------------|
| `*.csproj`, `*.props`, `*.targets`, etc. | 2 | LF |
| `*.json` | 2 | LF |
| `*.yml`, `*.yaml` | 2 | LF |
| `*.ps1` | 4 | CRLF (UTF-8 BOM) |

---

## C# Formatting Rules

### Indentation and Spacing

- **Indent size:** 4 spaces
- **Braces:** Allman style (new line before opening brace)
- **Space before colon in base or interface list:** Yes
- **Space after colon in base or interface list:** Yes
- **Space around binary operators:** Yes

### New Line Preferences

```
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_new_line_before_members_in_object_initializers = true
csharp_new_line_before_members_in_anonymous_types = true
```

### Namespaces

- **File-scoped namespaces required** in C# 10+:

```csharp
// Correct
namespace MyNamespace;

public class MyClass { }

// Incorrect
namespace MyNamespace
{
    public class MyClass { }
}
```

### `var` Usage

- Use `var` for built-in types and when the type is apparent from the right-hand side.
- Use explicit types when the type is not obvious.

```csharp
// Preferred
var count = 0;
var items = new List<string>();

// Preferred when type is not obvious
IEnumerable<string> results = GetResults();
```

### Null Checks

Prefer pattern matching over equality comparisons:

```csharp
// Correct
if (value is null) { }
if (value is not null) { }

// Avoid
if (value == null) { }
if (value != null) { }
```

### Naming Conventions

| Symbol | Convention | Example |
|--------|-----------|---------|
| Public types and members | PascalCase | `PublicMethod` |
| Parameters and local variables | camelCase | `localVariable` |
| Private fields | `_camelCase` | `_privateField` |
| Constants | PascalCase | `MaxRetries` |
| Interfaces | `I` prefix + PascalCase | `IMyInterface` |
| Type parameters | `T` prefix + PascalCase | `TResult` |

---

## Running the Formatter

```bash
# Format all files
dotnet format

# Check formatting without changes (CI mode)
dotnet format --verify-no-changes

# PowerShell formatting script
pwsh ./scripts/format.ps1
```

---

## Analyzer Severity Levels

This project uses several analyzer packages. Key rules:

| Category | Example Rule | Default Severity |
|----------|-------------|-----------------|
| AsyncFixer | `AsyncFixer01` – Unnecessary async/await | Error |
| AsyncFixer | `AsyncFixer02` – Blocking calls in async methods | Error |
| CA (Code Analysis) | `CA1849` – Call async methods when in async method | Warning |
| VSTHRD | `VSTHRD100` – Avoid async void methods | Warning |
| Roslynator | `RCS1138` – Add summary to documentation comment | Warning |

See the [.editorconfig](../.editorconfig) file for the complete set of rules and their configured severities.
