window.BENCHMARK_DATA = {
  "lastUpdate": 1791699956578,
  "repoUrl": "https://github.com/Chris-Wolfgang/ICollection-Extensions",
  "entries": {
    "Mutation score": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "771490fc6a35119011cbc360d6dd3f12e638a56d",
          "message": "docs(security): take repo-template's security policy, with the disclosure process and timelines (#271)\n\nScorecard scores this repository 4/10 on Security-Policy: \"security policy file\ndetected: Warn: no linked content found\". The file exists - all twelve affected\nrepositories carry the same 735-byte stub - but Scorecard also grades its\nCONTENT, and the stub has no links, no addresses and no concrete timelines, so\nit reads as a placeholder rather than a policy someone could act on.\n\nrepo-template's version is the one that scores: it names the private advisory\nURL for this repository, links GitHub's own guidance on private reporting, and\nreplaces \"we will acknowledge within 48 hours\" with a table that commits to\nassessment and fix windows by severity - explicitly scoped to what a\nsingle-maintainer project can actually meet. It also documents the disclosure\nprocess end to end (report, triage, private fork, release, publish + CVE) and\nsays reporters are credited.\n\nThe {{GITHUB_REPO_URL}} placeholder is substituted for this repository, so the\nadvisory link is real rather than pointing at the template.\n\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-23T12:27:10Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/771490fc6a35119011cbc360d6dd3f12e638a56d"
        },
        "date": 1790490379451,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "b862d0e8889adc103b565c6e94a3ca328e8c00cd",
          "message": "chore: explicit usings on every TFM (no implicit/global usings in multi-TFM projects) (#276)\n\nRemove ImplicitUsings (src net8+/net9/net10 slices, tests all TFMs) and the\n<Using Include=\"Xunit\" /> global using from the test project, and declare the\nusings each file needs explicitly, so the directives are required on every TFM\nand single-slice InspectCode analysis stops flagging them as redundant.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-28T14:02:26Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/b862d0e8889adc103b565c6e94a3ca328e8c00cd"
        },
        "date": 1791101480151,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "dependabot[bot]",
            "username": "dependabot[bot]",
            "email": "49699333+dependabot[bot]@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "349d23ada419276f39256a29ab6277fa743817d7",
          "message": "build(deps): bump urllib3 from 2.7.0 to 2.8.0 in /.github/requirements (#287)\n\nBumps [urllib3](https://github.com/urllib3/urllib3) from 2.7.0 to 2.8.0.\n- [Release notes](https://github.com/urllib3/urllib3/releases)\n- [Changelog](https://github.com/urllib3/urllib3/blob/main/CHANGES.rst)\n- [Commits](https://github.com/urllib3/urllib3/compare/2.7.0...2.8.0)\n\n---\nupdated-dependencies:\n- dependency-name: urllib3\n  dependency-version: 2.8.0\n  dependency-type: indirect\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-11T02:40:40Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/349d23ada419276f39256a29ab6277fa743817d7"
        },
        "date": 1791699953408,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 100,
            "unit": "%"
          }
        ]
      }
    ]
  }
}