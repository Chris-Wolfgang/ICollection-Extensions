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
- `RemoveWhere<T>(this ICollection<T>, Func<T, bool>) -> int` — removes every item matching the predicate; returns the count removed. Materialises matches into a temp list before mutating to keep enumeration safe.
- `ReplaceAll<T>(this ICollection<T>, IEnumerable<T>)` — clears the collection then appends every item from the new sequence. Not atomic — see remarks.
- `AddIfNotContains<T>(this ICollection<T>, T) -> bool` — generalises `HashSet<T>.Add`'s Boolean return to every `ICollection<T>`.
- `AddIfNotContains<T>(this ICollection<T>, IEnumerable<T>) -> int` — bulk overload; returns the count actually added.
- `IsEmpty<T>(this ICollection<T>)` — `true` if `Count == 0`. O(1) for every standard `ICollection<T>` implementation.
- `IsNotEmpty<T>(this ICollection<T>)` — `true` if `Count > 0`.

## Important Notes
- All nine methods throw `ArgumentNullException` for `null` arguments and pass through the target's own `NotSupportedException` for read-only collections.
- The pre-allocation branch in `AddRange` requires **both** conditions to hold: target is `List<T>` (the only `ICollection<T>` with a settable `Capacity`) **and** the sequence exposes `Count` via `ICollection<T>`. Without both, the loop falls back to the target's own growth policy.
- `HashSet<T>` semantics flow through naturally — duplicates added via `AddRange` are silently ignored.
- Self-aliasing (`list.AddRange(list)`, `list.ReplaceAll(list)`, etc.) is guarded via a `ReferenceEquals` snapshot on every mutating method that enumerates `items` — without that guard most BCL enumerators throw `InvalidOperationException` from mutate-during-enumerate.
- Public API is tracked by `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` under `src/Wolfgang.Extensions.ICollection/`. Additions surface as RS0016 at compile time; the analyzer is suppressed on test + benchmark projects via `NoWarn`.

## Code Style
- Allman brace style
- 3 blank lines between members
- File-scoped namespaces
- Warnings as errors in Release builds
- Test names follow `MethodUnderTest_when_condition_expected_result`

## Test Conventions
- xunit 2.9.3 across every TFM. `xunit.runner.visualstudio` is pinned **per TFM**: `[2.4.5]` for `netcoreapp3.1` and `net5.0` (the last versions of the runner compatible with those older runtimes), `[2.8.2]` for every other TFM (`net462`/`net47`/`net471`/`net472`/`net48`/`net481` and `net6.0` through `net10.0`). NEVER bump to xunit v3 — incompatible with our xunit v2 surface.
- One assertion per logical case; prefer `[Theory]` + `MemberData` when the same shape repeats across collection types

## Target Frameworks
- **src:** netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0
- **tests:** the full multi-TFM matrix from net462 through net10.0

## Related repos
- Sibling extension libraries in the Wolfgang.Extensions.* family: IEnumerable-Extensions, IAsyncEnumerable-Extensions, IComparable-Extensions, IEquatable-Extensions, DateTime-Extensions, String-Extensions
- Each follows the same csproj + test + benchmark + docfx pattern from `Chris-Wolfgang/repo-template`
