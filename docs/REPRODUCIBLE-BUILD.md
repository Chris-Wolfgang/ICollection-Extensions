# Verifying the Build

This page explains how to check a published `Wolfgang.Extensions.ICollection` package against the
source at its release tag, where each release's expected hashes are published, and how to record an
independent verification. It records what has been measured and where the limits are; it does not
promise a bit-identical `.nupkg` or assembly.

## What you can and cannot expect

- **The `.nupkg` container is only comparable when it was packed with `SOURCE_DATE_EPOCH`.** A
  package is a zip, and NuGet stamps every entry with a timestamp: the pack time by default, or
  `SOURCE_DATE_EPOCH` when that environment variable is set (NuGet maps it to
  `DeterministicTimestamp`). Releases that publish a `reproducible-build-manifest.json` (below) were
  packed with `SOURCE_DATE_EPOCH` set to the tagged commit's time, so a rebuild packed with the same
  value can match the container byte for byte. NuGet appears to write that timestamp in the
  machine's **local time zone** (observed on one Windows machine): the release runs in UTC, so pack
  with `TZ=UTC` (Linux/macOS) or on a machine set to UTC. For older releases, compare the *entries*
  (name, order, content), not the container.
- **Compare the container against the GitHub Release asset, not the nuget.org download.** NuGet.org
  adds a repository signature (`.signature.p7s`) to the copy it serves, so that file never matches a
  rebuild. The `.nupkg` attached to the GitHub Release is the unsigned original.
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

## Expected hashes: `reproducible-build-manifest.json`

Every release built by the current `release.yaml` attaches `reproducible-build-manifest.json` to its
GitHub Release (next to the `.nupkg`, `.snupkg` and SBOM). It records:

| Field | Meaning |
|---|---|
| `tag`, `commit` | The release tag and the exact commit that was built. |
| `sourceDateEpoch` | The `SOURCE_DATE_EPOCH` the packages were packed with (the commit's time). |
| `dotnetSdk` | The exact .NET SDK version that built the release. Use the same one. |
| `runnerOs`, `timeZone` | Where it was built (`windows-latest`, UTC). |
| `environment`, `commands` | The environment variables and the build and pack commands. |
| `artifacts` | `path` and `sha256` of every shipped `.dll` and `.pdb` (per target framework) and of every `.nupkg` / `.snupkg`, relative to the repository root. |

After the procedure below, from the root of your worktree:

```bash
jq -r '.artifacts[] | "\(.sha256)  \(.path)"' reproducible-build-manifest.json | sha256sum -c
```

Every line should say `OK`. Releases cut before the manifest step existed have no manifest; use
the entry comparison in step 4 for those.

## Procedure

1. Install the .NET SDK version named in the manifest's `dotnetSdk` (for example with
   `dotnet-install --version <dotnetSdk>`); without a manifest, the .NET 10 SDK. That is enough to
   build the `src/` project; the older SDKs in `release.yaml` are only needed to run the
   multi-target tests.
2. Check out the release tag in a **git worktree**, not a copied folder. A copy has no `.git`, so
   commit and SourceLink data are missing and the hashes differ for an unrelated reason.

   ```bash
   git clone https://github.com/Chris-Wolfgang/ICollection-Extensions.git
   cd ICollection-Extensions
   git worktree add ../verify v<version>
   cd ../verify
   ```

3. Build and pack, passing the release year taken from the published assembly's copyright, and
   (for a release with a manifest) the manifest's `sourceDateEpoch`, in UTC:

   ```bash
   export CI=true
   export TZ=UTC
   export SOURCE_DATE_EPOCH=<sourceDateEpoch from the manifest>
   dotnet restore src/Wolfgang.Extensions.ICollection
   dotnet build src/Wolfgang.Extensions.ICollection --no-restore --configuration Release -p:Copyright="Copyright (c) <year> Chris Wolfgang"
   dotnet pack src/Wolfgang.Extensions.ICollection --no-build --configuration Release --output ./nuget-packages
   ```

   With a manifest, download it from the GitHub Release into the worktree root and run the
   `sha256sum -c` check above.

   If restore fails on a vulnerability advisory published after the release (`NU1902` and similar),
   add `-p:NuGetAudit=false`. The audit depends on today's advisories, not on the tag.

4. Download the published package from NuGet.org and compare **entries** with the one you built.
   NuGet.org adds a `.signature.p7s` entry after publishing, and the `.psmdcp` entry's file name
   differs; ignore those two.

   ```bash
   mkdir built published
   unzip -q nuget-packages/*.nupkg -d built
   unzip -q Wolfgang.Extensions.ICollection.<version>.nupkg -d published
   (cd built && find lib -type f -exec sha256sum {} +) | sort -k2 > built.sha256
   (cd published && find lib -type f -exec sha256sum {} +) | sort -k2 > published.sha256
   diff built.sha256 published.sha256 && echo "assemblies match"
   ```

5. Independently of the rebuild, confirm provenance with the attestation described in
   [SECURITY.md](../SECURITY.md#verifying-a-release). Releases cut after #431 merged also attest
   `reproducible-build-manifest.json` itself, so `gh attestation verify reproducible-build-manifest.json`
   proves the hashes you compared against came from the release workflow.

## If something differs

Open an issue (not a security advisory, unless you believe the package was tampered with) with: the
tag, your OS and `dotnet --info` output, the `diff` or `sha256sum -c` output, and the entry or file
names that differ.

## Publishing a third-party verification attestation

Anyone who rebuilds a release can record the result so other consumers can rely on it. This follows
the [Reproducible Builds](https://reproducible-builds.org/docs/) convention of independent rebuilders
publishing signed statements about what they rebuilt and what they got.

1. **Write the statement.** Save a `verification.json` next to your build:

   ```json
   {
     "subject": "Wolfgang.Extensions.ICollection",
     "tag": "v<version>",
     "commit": "<commit from the manifest>",
     "manifestSha256": "<sha256 of the reproducible-build-manifest.json you checked>",
     "result": "reproduced",
     "mismatches": [],
     "environment": "<OS, dotnet --version, TZ>",
     "verifiedBy": "<your name or GitHub handle>",
     "date": "<YYYY-MM-DD>"
   }
   ```

   Use `"result": "mismatch"` and list the differing paths in `mismatches` if any check failed.
2. **Sign it** with a key that others can tie to you, for example
   `ssh-keygen -Y sign -f ~/.ssh/id_ed25519 -n file verification.json` (an SSH key on your GitHub
   profile), `gpg --armor --detach-sign verification.json`, or
   `cosign sign-blob --bundle verification.json.bundle verification.json` (keyless, tied to your
   OIDC identity).
3. **Upload it.** Open an issue titled `Rebuild verification: v<version>` and attach
   `verification.json`, the signature file, the full `sha256sum -c` output and your `dotnet --info`.
   Say which key or identity signed it. The maintainer links verified attestations from the release
   notes; a `mismatch` is investigated as described above.

A verification service that publishes rebuild attestations for NuGet packages can be used the same
way: link its attestation in the issue instead of attaching your own files.
