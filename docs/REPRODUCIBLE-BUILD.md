# Verifying the Build

This page explains how to check a published `Wolfgang.Extensions.ICollection` package against the
source at its release tag. It records what has been measured and where the limits are; it does not
promise a bit-identical `.nupkg` or assembly.

## What you can and cannot expect

- **The `.nupkg` file itself will never hash the same twice.** A package is a zip, and NuGet stamps
  every entry with its modification time. Compare the *entries* (name, order, content), not the
  container.
- **Assemblies are built deterministically, but a rebuild is not guaranteed to be byte-identical.**
  The SDK compiles deterministically by default, and `Directory.Build.props` sets
  `ContinuousIntegrationBuild` when the `CI` environment variable is `true`, which normalises source
  paths. Set `CI=true` for your rebuild. Differences in SDK version, operating system, and the
  copyright year can still change bytes without changing IL.
- **A measured example.** On 2026-10-20, v0.3.1 was rebuilt on Windows with SDK 10.0.400 and
  `CI=true`. The XML documentation files matched the published package byte for byte. The five
  assemblies did **not**: same file sizes, but 791 bytes differed in the `net10.0` one. The cause was
  not isolated, so do not read a mismatch as evidence of tampering by itself; compare the IL (for
  example with `ilspycmd` or `monodis`) before drawing a conclusion.
- **The release is built on `windows-latest`** (`pack-and-validate` in `release.yaml`), so a Windows
  rebuild is the closest comparison. Cross-OS byte identity is not provided: this repository does not
  normalise the SDK's generated source files, which are written with the OS newline.
- **The copyright year is part of the assembly.** `Directory.Build.props` builds the `Copyright`
  string from the current UTC year, so a rebuild in a later year differs. Pass the release year
  explicitly (below).

## Procedure

1. Install the .NET 10 SDK (enough to build the `src/` project; the older SDKs in `release.yaml` are
   only needed to run the multi-target tests).
2. Check out the release tag in a **git worktree**, not a copied folder. A copy has no `.git`, so
   commit and SourceLink data are missing and the hashes differ for an unrelated reason.

   ```bash
   git clone https://github.com/Chris-Wolfgang/ICollection-Extensions.git
   cd ICollection-Extensions
   git worktree add ../verify v<version>
   cd ../verify
   ```

3. Build and pack, passing the release year taken from the published assembly's copyright:

   ```bash
   export CI=true
   dotnet restore src/Wolfgang.Extensions.ICollection
   dotnet build src/Wolfgang.Extensions.ICollection --no-restore --configuration Release -p:Copyright="Copyright (c) <year> Chris Wolfgang"
   dotnet pack src/Wolfgang.Extensions.ICollection --no-build --configuration Release --output ./out
   ```

   If restore fails on a vulnerability advisory published after the release (`NU1902` and similar),
   add `-p:NuGetAudit=false`. The audit depends on today's advisories, not on the tag.

4. Download the published package from NuGet.org and compare **entries** with the one you built.
   NuGet.org adds a `.signature.p7s` entry after publishing, and the `.psmdcp` entry's file name
   differs; ignore those two.

   ```bash
   mkdir built published
   unzip -q out/*.nupkg -d built
   unzip -q Wolfgang.Extensions.ICollection.<version>.nupkg -d published
   (cd built && find lib -type f -exec sha256sum {} +) | sort -k2 > built.sha256
   (cd published && find lib -type f -exec sha256sum {} +) | sort -k2 > published.sha256
   diff built.sha256 published.sha256 && echo "assemblies match"
   ```

5. Independently of the rebuild, confirm provenance with the attestation described in
   [SECURITY.md](../SECURITY.md#verifying-a-release).

## If something differs

Open an issue (not a security advisory, unless you believe the package was tampered with) with: the
tag, your OS and `dotnet --info` output, the `diff` output, and the entry names that differ.

## Not yet provided

Two items from the original request are not done here: a per-release `reproducible-build-manifest.json`
published by `release.yaml` (a workflow change, which needs its own protected-file PR), and a
procedure for a third party to upload a verification attestation.
