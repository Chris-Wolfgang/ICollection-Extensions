# Copilot Instructions for Wolfgang.Extensions.ICollection

## Project Overview
- **Package:** Wolfgang.Extensions.ICollection
- **Namespace:** Wolfgang.Extensions.ICollection
- **Purpose:** Extension methods for `System.Collections.Generic.ICollection<T>` — bulk add and emptiness checks that work across every concrete collection type, not just `List<T>`.

## Key Types
- `ICollectionExtensions` — static class with extension methods on `ICollection<T>`

## Extension Methods
- `AddRange<T>(this ICollection<T>, IEnumerable<T>)` — appends every item from the sequence to the collection. Pre-allocates capacity when the target is `List<T>` and the appended sequence exposes `ICollection<T>.Count` (the common batch-append fast path).
- `IsEmpty<T>(this ICollection<T>)` — `true` if `Count == 0`. O(1) for every standard `ICollection<T>` implementation.
- `IsNotEmpty<T>(this ICollection<T>)` — `true` if `Count > 0`.

## Important Notes
- All three methods throw `ArgumentNullException` for `null` arguments and pass through the target's own `NotSupportedException` for read-only collections.
- The pre-allocation branch in `AddRange` requires **both** conditions to hold: target is `List<T>` (the only `ICollection<T>` with a settable `Capacity`) **and** the sequence exposes `Count` via `ICollection<T>`. Without both, the loop falls back to the target's own growth policy.
- `HashSet<T>` semantics flow through naturally — duplicates added via `AddRange` are silently ignored.
- Public API will be tracked by `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` once the A1 baseline is generated; additions will surface as RS0016 at compile time.

## Code Style
- Allman brace style
- 3 blank lines between members
- File-scoped namespaces
- Warnings as errors in Release builds
- Test names follow `MethodUnderTest_when_condition_expected_result`

## Test Conventions
- xunit 2.9.3 across every TFM. `xunit.runner.visualstudio` is pinned **per TFM**: `[2.4.5]` for net462 and net47 (last version compatible with those frameworks), `[2.8.2]` for net471/net472/net48/net481/netcoreapp3.1/net5.0–net10.0. NEVER bump to xunit v3 — incompatible with our xunit v2 surface.
- One assertion per logical case; prefer `[Theory]` + `MemberData` when the same shape repeats across collection types

## Target Frameworks
- **src:** netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0
- **tests:** the full multi-TFM matrix from net462 through net10.0

## Related repos
- Sibling extension libraries in the Wolfgang.Extensions.* family: IEnumerable-Extensions, IAsyncEnumerable-Extensions, IComparable-Extensions, IEquatable-Extensions, DateTime-Extensions, String-Extensions
- Each follows the same csproj + test + benchmark + docfx pattern from `Chris-Wolfgang/repo-template`
