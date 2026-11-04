# Copilot Instructions for Wolfgang.Extensions.ICollection

## Project Overview
- **Package:** Wolfgang.Extensions.ICollection
- **Namespace:** Wolfgang.Extensions.ICollection
- **Purpose:** Extension methods for `System.Collections.Generic.ICollection<T>` — bulk add and emptiness checks that work across every concrete collection type, not just `List<T>`.

## Key Types
- `ICollectionExtensions` — static class with extension methods on `ICollection<T>`

## Extension Methods
- `AddRange<T>(this ICollection<T>, IEnumerable<T>)` — appends every item from the sequence to the collection. Pre-allocates capacity when the target is `List<T>` and the appended sequence exposes `ICollection<T>.Count` (the common batch-append fast path).
- `AddRangeIf<T>(this ICollection<T>, IEnumerable<T>, Func<T, bool>)` — appends items for which the predicate returns `true`.
- `RemoveRange<T>(this ICollection<T>, IEnumerable<T>)` — removes one occurrence of each listed item.
- `RemoveWhere<T>(this ICollection<T>, Func<T, bool>) -> int` — removes every item matching the predicate; returns the count removed. `HashSet<T>` fast path delegates to the native `HashSet<T>.RemoveWhere(Predicate<T>)` (no temp-list allocation); other `ICollection<T>` implementations materialise matches into a temp list before mutating to keep enumeration safe.
- `ReplaceAll<T>(this ICollection<T>, IEnumerable<T>)` — clears the collection then appends every item from the new sequence. Not atomic — see remarks.
- `AddIfNotContains<T>(this ICollection<T>, T) -> bool` — `ISet<T>` fast path delegates to `ISet<T>.Add` (single lookup, honours the set's own `IEqualityComparer<T>`); other `ICollection<T>` implementations use `Contains` + `Add`. Generalises `HashSet<T>.Add`'s Boolean return to every `ICollection<T>`.
- `AddIfNotContains<T>(this ICollection<T>, IEnumerable<T>) -> int` — bulk overload; returns the count actually added.
- `IsEmpty<T>(this ICollection<T>)` — `true` if `Count == 0`. Typically O(1) for standard `ICollection<T>` implementations; the `ICollection<T>.Count` contract does not formally guarantee constant time.
- `IsNotEmpty<T>(this ICollection<T>)` — `true` if `Count > 0`.

## Important Notes
- All nine methods throw `ArgumentNullException` for `null` arguments and pass through the target's own `NotSupportedException` for read-only collections.
- The pre-allocation branch in `AddRange` requires **both** conditions to hold: target is `List<T>` (the only `ICollection<T>` with a settable `Capacity`) **and** the sequence exposes `Count` via `ICollection<T>`. Without both, the loop falls back to the target's own growth policy.
- `HashSet<T>` semantics flow through naturally — duplicates added via `AddRange` are silently ignored.
- Self-aliasing (`list.AddRange(list)`, `list.ReplaceAll(list)`, etc.) is guarded via a `ReferenceEquals` snapshot on the methods that would mutate `source` while enumerating `items`: `AddRange`, `AddRangeIf`, `RemoveRange`, and `ReplaceAll`. `AddIfNotContains(IEnumerable<T>)` doesn't need the snapshot — every item is already present so the inner `AddIfNotContains(T)` returns `false` without mutating, making the enumeration safe by construction.
- Public API is tracked by `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` under `src/Wolfgang.Extensions.ICollection/`. Additions surface as RS0016 at compile time; the analyzer is suppressed on test + benchmark projects via `NoWarn`.

## Code Style
- Allman brace style
- 3 blank lines between members
- File-scoped namespaces
- Warnings as errors in Release builds
- Test names follow `MethodUnderTest_when_condition_expected_result`

## Test Conventions
- xunit 2.9.3 across every TFM. NEVER bump the `xunit` package to v3 — incompatible with our xunit v2 surface. That rule is about `xunit` itself, not the VS test adapter: `xunit.runner.visualstudio` is split by TFM slot — `3.0.1` (uncapped) on `net8.0`/`net9.0`/`net10.0` (runner 3.x runs xunit v2 tests), `[2.8.2]` on `net462`–`net481`, `net6.0` and `net7.0`, and `[2.4.5]` on `netcoreapp3.1` and `net5.0` (2.8.2 has no discovery adapter there). The exact pins on the older slots stop Dependabot moving them to 3.x, which drops those frameworks.
- One assertion per logical case; prefer `[Theory]` + `MemberData` when the same shape repeats across collection types

## Target Frameworks
- **src:** netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0
- **tests:** the full multi-TFM matrix from net462 through net10.0

## Related repos
- Sibling extension libraries in the Wolfgang.Extensions.* family: IEnumerable-Extensions, IAsyncEnumerable-Extensions, IComparable-Extensions, IEquatable-Extensions, DateTime-Extensions, String-Extensions
- Each follows the same csproj + test + benchmark + docfx pattern from `Chris-Wolfgang/repo-template`
