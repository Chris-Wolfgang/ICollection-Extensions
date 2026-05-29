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
