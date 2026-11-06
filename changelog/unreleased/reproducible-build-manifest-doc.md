type: docs

Document the per-release `reproducible-build-manifest.json` (expected SHA-256 of every shipped assembly, symbol file and package), packing with `SOURCE_DATE_EPOCH` in UTC so a rebuilt `.nupkg` can match byte for byte, and how a third party publishes a signed rebuild-verification attestation (`docs/REPRODUCIBLE-BUILD.md`).
