# Migration guides

One guide per major version, written during the release prep for that version (not after it
ships) and linked from its GitHub Release notes.

| Guide | From | To | Status |
|-------|------|----|--------|
| _none yet_ | | | The package is pre-1.0 (0.3.1); the first guide is due with the first release that breaks the public API |

## Writing one

1. Copy [TEMPLATE-major-version-migration.md](TEMPLATE-major-version-migration.md) to
   `docs/migrations/vX-to-vY.md`.
2. Fill the breaking-change inventory from the `PublicAPI.Shipped.txt` diff between the last
   release of the old version and the new one, plus any `breaking` changelog fragments
   (`changelog/unreleased/`). Package validation (`EnablePackageValidation`) fails the pack on
   an unsuppressed break, and every suppression in `CompatibilitySuppressions.xml` belongs in
   the inventory too.
3. Add the guide to the table above and link it from the release notes.

Pre-1.0 note: while the package is 0.x, a breaking change ships as a minor bump
(see `changelog/unreleased/README.md`). Write a guide for such a release too when the break
needs more than a one-line changelog entry to migrate.
