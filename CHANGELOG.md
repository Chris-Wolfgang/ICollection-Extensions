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

## [0.3.1] - 2026-06-01

Polish release. The public API and runtime behavior are unchanged from
v0.3.0; this round delivers documentation, examples, tests, benchmarks,
and canonical workflow updates.

### Added

- `docs/DOCFX-VERSION-PICKER.md` — the canonical 210-line reference
  for the docs-site version picker (companion to the
  `docfx_project/public/version-picker.js` /
  `docfx_project/versions.json` /
  `.github/version-picker-template.html` triplet that was already
  shipping).
- `examples/Wolfgang.Extensions.ICollection.DotNet80.Example1/` and
  `examples/Wolfgang.Extensions.ICollection.DotNet10.Example1/` —
  runnable demonstration apps exercising every public extension
  method against `List<T>`, `HashSet<T>`, `LinkedList<T>`, and
  `Collection<T>`. The .NET 10 example uses C# 12 collection
  expressions; the .NET 8 example uses traditional `new[]` syntax.
- `RemoveWhere_when_source_is_HashSet_uses_native_RemoveWhere` test —
  closes the previous Stryker `NoCoverage` gap on the HashSet
  fast-path block.
- Twelve fast/slow-path-paired benchmarks covering the six public
  methods that were previously unbenchmarked: `RemoveRange`,
  `AddRangeIf`, `RemoveWhere`, `ReplaceAll`,
  `AddIfNotContains<T>(T)`, `AddIfNotContains<T>(IEnumerable<T>)`.
  Brings benchmark coverage to 9 / 9 public methods. The
  `_itemsToAdd` test array is now populated with distinct values
  (`Enumerable.Range(0, Count).ToArray()`) so the HashSet fast-path
  benchmarks measure a real `Count`-element set, not a single-element
  deduplication.

### Changed

- XML documentation expanded across `AddRange` / `AddRangeIf` /
  `RemoveRange` / `ReplaceAll` to describe the self-aliasing
  `ReferenceEquals` snapshot guard (passing the same collection as
  both `source` and `items` is now documented as safe).
- `IsNotEmpty` remarks block — the phrase "self-documenting emptiness
  check" was inaccurate (the method returns true when `Count > 0`);
  corrected to "non-emptiness check".
- README — analyzer count clarified to "8 analyzer rule sets (7
  explicit `PackageReference`s plus the SDK's built-in
  `NetAnalyzers`)" with each named.
- `.github/copilot-instructions.md` — method descriptions resynced
  with the v0.3.0 fast-path behavior.
- `CHANGELOG` — the v0.3.0 release date corrected from `2026-05-30`
  to `2026-05-31` (matches the actual `v0.3.0` tag timestamp).
- `docfx_project/` — landing page, introduction, getting-started all
  rewritten for v0.3.0+ content.
- `.github/workflows/stryker.yaml` — switched to a Windows runner,
  dropped the per-repo opt-in gate (`stryker-config.json` presence
  is now the universal signal), comprehensive SDK install list
  (`3.1.x` through `10.0.x`).
- `.github/workflows/codeql.yaml` — bumped `github/codeql-action/init`
  and `analyze` from `@v3` to `@v4` (Node.js 20 → 24 deprecation).
- `src` and test csprojs — dropped redundant `<Copyright>` (test)
  and `<Title>$(AssemblyName)</Title>` (src); both inherit from
  `Directory.Build.props` / MSBuild defaults respectively.
- `assets/icon.ico` — moved out of the src project directory; the
  binary is retained as a fleet asset rather than a per-project
  resource.

### Fixed

- Three Stryker mutation survivors on `ICollectionExtensions.cs`
  reclassified as intentional via `// Stryker disable all : <reason>`
  comments: the five redundant defensive null-check blocks
  (`AddRangeIf` predicate / `RemoveWhere` source+predicate /
  `ReplaceAll` items / `AddIfNotContains(IEnumerable<T>)` source) and
  the two redundant optimization fast paths (`RemoveWhere`'s
  `HashSet<T>` block, `AddIfNotContains<T>(T)`'s `ISet<T>` block).
  Each annotation documents why the mutated form is observationally
  indistinguishable from the original (the BCL / slow path throws
  the same `ArgumentNullException` or produces the same final
  state). Brings mutation score on `ICollectionExtensions.cs` to
  100 % on the testable surface.
- Mis-indented `EmptyStateCases` comment block in the test file —
  re-aligned to the surrounding 4-space member indentation.
- `AddRange_when_items_is_large_collection_adds_all_items` —
  extracted the literal `10_000` into a named
  `const int LargeBatchCount` so the value lives in one place and
  the three assertions self-document.

## [0.3.0] - 2026-05-31

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

[Unreleased]: https://github.com/Chris-Wolfgang/ICollection-Extensions/compare/v0.3.1...HEAD
[0.3.1]: https://github.com/Chris-Wolfgang/ICollection-Extensions/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/Chris-Wolfgang/ICollection-Extensions/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Chris-Wolfgang/ICollection-Extensions/releases/tag/v0.2.0
