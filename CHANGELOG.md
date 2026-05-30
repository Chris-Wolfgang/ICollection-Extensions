# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `RemoveRange<T>(this ICollection<T>, IEnumerable<T>)` — removes one
  occurrence of each listed item from the source collection.
- `AddRangeIf<T>(this ICollection<T>, IEnumerable<T>, Func<T, bool>)` —
  adds items that satisfy the predicate.
- `RemoveWhere<T>(this ICollection<T>, Func<T, bool>) -> int` — removes
  every item matching the predicate; returns the count removed. Safe
  for any `ICollection<T>` (materialises matches into a temp list
  before mutating).
- `ReplaceAll<T>(this ICollection<T>, IEnumerable<T>)` — clears the
  collection then appends every item from the new sequence.
- `AddIfNotContains<T>(this ICollection<T>, T) -> bool` — adds a
  single item only if it is not already present; returns whether the
  add happened. Generalises `HashSet<T>.Add`'s Boolean return to every
  `ICollection<T>`.
- `AddIfNotContains<T>(this ICollection<T>, IEnumerable<T>) -> int` —
  bulk overload; returns the count actually added.

### Changed

### Deprecated

### Removed

### Fixed

### Security

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

[Unreleased]: https://github.com/Chris-Wolfgang/ICollection-Extensions/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/Chris-Wolfgang/ICollection-Extensions/releases/tag/v0.2.0
