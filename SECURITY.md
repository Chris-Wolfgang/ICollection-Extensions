# Security Policy

## Supported Versions

Security fixes are released for the **latest published version** only. If you are on an older
version, upgrade to the latest release to receive the fix. Pre-1.0 releases (0.x) follow the same
rule: the newest 0.x release is the supported one.

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not** create a public issue on this repository.
2. Open the private report form: https://github.com/Chris-Wolfgang/ICollection-Extensions/security/advisories/new
   (or, from the repository's **Security** tab, click **Report a vulnerability**).
   GitHub's guide to this process: https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing-information-about-vulnerabilities/privately-reporting-a-security-vulnerability
3. Fill out the provided form with:
   - A description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)

## Response Timeline

This is a single-maintainer project; the commitments below are realistic for that, not aspirational.

| Stage | Target |
|-------|--------|
| Acknowledgement of the report | Within 48 hours |
| Initial assessment (confirmed / not a vulnerability / need more information) and severity | Within 7 days |
| Fix for **Critical** severity | Within 30 days |
| Fix for **High** or **Medium** severity | Within 90 days |
| Fix for **Low** severity | Next scheduled release |

If a target is going to slip, you will hear that from the maintainer in the advisory thread before
the deadline passes, not after.

## Disclosure Process

1. **Report** — you open a private advisory (above). It is not public, but it is not a
   two-person conversation either. GitHub grants access to the repository owner, to organization
   owners, to security managers, and to anyone with the repository's admin role, plus any
   collaborator explicitly added to the advisory. Repository write access alone does not grant it.
   Nobody outside that set sees the report until it is published.
2. **Triage** — the maintainer confirms the issue and assigns a severity, discussed with you in the
   advisory thread.
3. **Fix** — the fix is developed in a temporary private fork attached to the advisory, so nothing
   about the vulnerability is visible in public pull requests until the fix is released.
4. **Release** — a new version ships with the fix. The release notes say a security issue was fixed
   without giving details that would help exploit unpatched versions.
5. **Publish** — the advisory is published, which notifies dependents through Dependabot. A CVE is
   requested from GitHub for confirmed vulnerabilities; assignment is GitHub's decision, so the
   advisory may publish without one. Publication happens at release time, or after 90 days from the
   initial report if no fix is possible, whichever comes first.

Please keep the details private until the advisory is published.

## Credit

Reporters are credited in the published advisory and in the release notes, unless you ask not to be.
Tell us in the report how you would like to be named.

## Thank You

Your help is greatly appreciated!
Responsible disclosure of security vulnerabilities helps protect our entire community.

## OSSF Scorecard

[`scorecard.yaml`](.github/workflows/scorecard.yaml) runs the
[OSSF Scorecard](https://github.com/ossf/scorecard) weekly (Mondays 06:31 UTC) and on every push
to `main`, scoring this repo's security posture (branch protection, pinned dependencies,
dangerous-workflow patterns, vulnerability response time, etc.). Results publish to the
[Scorecard viewer](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/ICollection-Extensions)
and the badge in `README.md`, and upload as SARIF to this repo's Security tab alongside CodeQL alerts.

**Score floor: 7.5** (the target). If a run drops the score below the floor, or below the baseline
while the baseline is still under it, note it in `CHANGELOG.md` under `### Security` and open a
maintenance issue for the regressed check; don't let it sit unaddressed.

**Baseline: 7 / 10**, the first published score (run of 2026-11-12 on `main` @ `abf9f9a`, read from
`api.scorecard.dev` on 2026-11-14). It is below the 7.5 target; the floor stays at 7.5 rather than
being lowered to match, and the gap is the checks below. Every other check scored 10.

| Check | Score | Status |
|---|---|---|
| Branch-Protection | 4 | Accepted for now: `main` does not require approvers or code-owner review (ruleset settings; alert tracked in #248, `blocked`). |
| Pinned-Dependencies | 7 | Open: actions are SHA-pinned; the findings are `dotnet restore` / NuGet steps in workflows, because the projects use neither Central Package Management nor lock files. |
| Fuzzing | 0 | Accepted: Scorecard reports "no fuzzer integrations found"; it does not detect the CsCheck property suite in `tests/Wolfgang.Extensions.ICollection.Tests.Fuzz` or `fuzz.yaml` (#246). |
| Signed-Releases | 0 | Expected to rise: the scored releases (v0.2.0-v0.3.1) predate the SLSA provenance step in `release.yaml`; package signing waits on a code-signing certificate (#216, `blocked`). |
| Code-Review | 0 | Accepted: single-maintainer repository; 0 of the last 26 changesets had an approving review. |
| Contributors | 0 | Accepted: single-maintainer repository (no contributing organizations). |
| CII-Best-Practices | 0 | Open: no OpenSSF Best Practices badge has been applied for. |

## Release path & compromise scope

Facts a maintainer would need at 2am if the release identity is compromised. Generic incident-response steps (rotating credentials, revoking OAuth apps, publishing advisories, unlisting NuGet packages) are not duplicated here — GitHub's and NuGet's own docs update faster than a checked-in runbook.

- **Release path**: OIDC / NuGet Trusted Publishing via `NuGet/login` in `.github/workflows/release.yaml`. The workflow mints an ephemeral push token per run via OIDC — the release path does not depend on a long-lived API key stored in GitHub secrets or on the NuGet account. During an incident, check the NuGet account for any long-lived API keys anyway (they can be created outside of CI) and delete anything you don't recognize.
- **Fallback**: none. If Trusted Publishing is compromised, the incident is at the GitHub-account level (the OIDC identity is `Chris-Wolfgang/ICollection-Extensions`).
- **Owner**: @Chris-Wolfgang.
- **Downstream consumers**: no dependents are known within the Wolfgang.* packages; unknown external consumers may exist on nuget.org.
- **Package coordinates for unlisting**: `Wolfgang.Extensions.ICollection` — https://www.nuget.org/packages/Wolfgang.Extensions.ICollection/.

## Verifying a release

Each release publishes two supply-chain artifacts that let a consumer check what they downloaded.

- **Build provenance (SLSA attestation).** `release.yaml` attests every `.nupkg` before it is pushed to nuget.org, binding the package's SHA-256 digest to this repository, the commit and the workflow run. Download the package from nuget.org (or the GitHub Release) and verify it with the GitHub CLI:

  ```bash
  gh attestation verify Wolfgang.Extensions.ICollection.<version>.nupkg     --repo Chris-Wolfgang/ICollection-Extensions     --signer-workflow Chris-Wolfgang/ICollection-Extensions/.github/workflows/release.yaml
  ```

  A non-zero exit means the file was not produced by that workflow in this repository.
  Only releases cut after the attestation step was added carry one: `gh attestation verify` finds no attestation for 0.3.1 (checked 2026-10-20), which predates it.
- **SBOM.** A CycloneDX software bill of materials, `Wolfgang.Extensions.ICollection.bom.json`, is attached to each GitHub Release next to the `.nupkg`. It lists the package's dependency graph. It is published alongside the package but is not itself covered by the attestation, so treat it as an inventory, not as proof.

Package *signing* (`nuget verify -Signatures`) is not available yet: it needs a code-signing certificate and is tracked in #216. Until then the attestation above is the way to verify provenance.
