# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [0.3.0] - 2026-05-30

### Added

- `RemoveRange<T>(this ICollection<T>, IEnumerable<T>)` — removes one
  occurrence of each listed item from the source collection.
- `AddRangeIf<T>(this ICollection<T>, IEnumerable<T>, Func<T, bool>)` —
  adds items that satisfy the predicate.
- `RemoveWhere<T>(this ICollection<T>, Func<T, bool>) -> int` — removes
  every item matching the predicate; returns the count removed. Uses
  `HashSet<T>.RemoveWhere` as a fast path for `HashSet<T>` consumers
  and falls back to a snapshot-then-remove pattern for every other
  `ICollection<T>`.
- `ReplaceAll<T>(this ICollection<T>, IEnumerable<T>)` — clears the
  collection then appends every item from the new sequence.
- `AddIfNotContains<T>(this ICollection<T>, T) -> bool` — adds a
  single item only if it is not already present; returns whether the
  add happened. Uses `ISet<T>.Add` as a fast path for set consumers
  so the call is a single lookup; otherwise generalises the
  `HashSet<T>.Add` Boolean return to every `ICollection<T>`.
- `AddIfNotContains<T>(this ICollection<T>, IEnumerable<T>) -> int` —
  bulk overload; returns the count actually added.
- `PublicAPI.Shipped.txt` baseline now tracks the full public surface
  via the `Microsoft.CodeAnalysis.PublicApiAnalyzers` package. Future
  surface changes will surface as RS0016 / RS0017 at build time.
- `CHANGELOG.md`, populated for both `0.2.0` and `0.3.0`.
- `benchmarks/Wolfgang.Extensions.ICollection.Benchmarks` —
  BenchmarkDotNet baseline covering `AddRange` (fast-path vs slow-path)
  and `IsEmpty` / `IsNotEmpty`. Published to gh-pages via the new
  `benchmarks.yaml` workflow.

### Changed

- Self-aliasing on `AddRange`, `AddRangeIf`, `RemoveRange`, and
  `ReplaceAll` (passing the same collection as both `source` and
  `items`) is now safe: each method snapshots `items` via
  `ReferenceEquals` before mutating, so the call no longer trips
  `InvalidOperationException` from a mutate-during-enumerate
  enumerator. `AddRange(list, list)` now appends a copy of the
  current contents; `ReplaceAll(list, list)` is a no-op;
  `RemoveRange(list, list)` empties the collection.
- README now documents the full nine-method surface in a single
  reference table; multi-target framework list matches the actual
  shipped TFMs (`netstandard2.0`, `netstandard2.1`, `net8.0`,
  `net9.0`, `net10.0`).

### Fixed

- `<AssemblyVersion>` is now explicitly pinned to `1.0.0.0` in the
  csproj (paired with `<FileVersion>$(Version).0</FileVersion>`).
  Without this, the canonical CI3 metadata sweep would have let the
  SDK derive `AssemblyVersion` from `<Version>` on every minor bump,
  changing the assembly's binding identity and forcing .NET Framework
  consumers onto binding redirects.

## [0.2.0] - 2026-05-01

Initial public NuGet release.

### Added

- `AddRange<T>(this ICollection<T>, IEnumerable<T>)` extension method.
  Pre-allocates capacity when the source is `List<T>` and the
  appended sequence exposes `ICollection<T>.Count`, eliminating
  intermediate resize allocations in the common batch-append path.
- `IsEmpty<T>(this ICollection<T>)` and `IsNotEmpty<T>(this ICollection<T>)`
  extension methods returning a Boolean for the source's empty state.
- Multi-framework targeting: `netstandard2.0`, `netstandard2.1`,
  `net8.0`, `net9.0`, `net10.0`.
- README content packaged into the NuGet `.nupkg` so it renders on the
  package's nuget.org page.

[Unreleased]: https://github.com/Chris-Wolfgang/ICollection-Extensions/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/Chris-Wolfgang/ICollection-Extensions/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Chris-Wolfgang/ICollection-Extensions/releases/tag/v0.2.0
