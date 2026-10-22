# ADR-0001: Pin AssemblyVersion to a fixed 1.0.0.0 baseline

## Status

Accepted (v0.3.1; see the `AssemblyVersion` entry in `CHANGELOG.md`)

## Context

.NET distinguishes `AssemblyVersion` (the identity the CLR binds against at load time) from
`FileVersion` / `InformationalVersion` (the human-facing release version, e.g. the NuGet
`<Version>`). If `AssemblyVersion` follows the NuGet version, every minor or patch release
changes the assembly's binding identity. A .NET Framework consumer that references the
library from more than one place then needs a binding redirect, or a recompile, on every
bump, even though no public API broke.

This package ships small, additive releases (new extension methods, fixes), and the canonical
CI metadata sweep would otherwise have let the SDK derive `AssemblyVersion` from `<Version>`.

## Decision

`<AssemblyVersion>` is pinned at `1.0.0.0` in
`src/Wolfgang.Extensions.ICollection/Wolfgang.Extensions.ICollection.csproj` and is bumped only
on a deliberate breaking API change. `<FileVersion>` (`$(Version).0`) and
`InformationalVersion` keep tracking the real `<Version>`, so tools that inspect the DLL still
show the exact release.

## Consequences

- Consumers never need a binding redirect for a release that does not break the API.
- Two package versions (e.g. 0.3.0 and 0.3.1) produce assemblies with the same
  `AssemblyVersion`. Anything that needs to tell them apart must read `FileVersion` or
  `InformationalVersion`.
- The bump is a manual, reviewed step tied to a breaking release. Forgetting it on a real break
  would bring back the binding conflicts this ADR exists to avoid; package validation
  (`EnablePackageValidation`, added in #323) is what flags the break itself.
