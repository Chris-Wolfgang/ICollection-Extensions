# ADR-0005: `BannedSymbols.txt` enforces the fleet's async-first rules

## Status

Accepted

## Context

This library is synchronous: every method is a short, CPU-bound operation on an in-memory
collection. It still carries the fleet-wide `BannedSymbols.txt` (blocking task waits, `.Result`,
`Thread.Sleep`, synchronous file and stream I/O, `Parallel.For`/`ForEach`/`Invoke`, ...),
enforced by `Microsoft.CodeAnalysis.BannedApiAnalyzers`, because the same `Directory.Build.props`,
analyzers and CI run across every Wolfgang.* repository and test, benchmark and sample code
lives here too.

The alternative was a repository-specific, shorter list. That would drift from the rest of the
fleet and let a synchronous-first habit into test or tool code that later moves to an async
library.

## Decision

Keep the fleet's `BannedSymbols.txt` unchanged in this repository. Each entry carries its own
reason and async alternative, which the analyzer prints in the diagnostic. Concurrency code
in the tests (`Tests.Concurrency`, added in #332) uses `Task.Run` / `Task.WhenAll`, not `Parallel.*`.

## Consequences

- A banned call anywhere in the repository fails the Release build with the replacement named
  inline.
- Some entries never apply to the library's own code (it has no I/O and no tasks). They cost
  nothing and keep the policy uniform.
- `BannedSymbols.txt` is a protected file: a change goes in its own PR and a maintainer reviews
  it on its own (`docs/WORKFLOW_SECURITY.md`).
- The list governs this repository's code only, not what a consumer of the package does.
