window.BENCHMARK_DATA = {
  "lastUpdate": 1794410007038,
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
          "id": "7d4579976929e1bcf86205d7d6ec1b6ae3e2aba0",
          "message": "fix(scripts): security-alerts updates the open summary issue instead of filing a new one (#292)\n\nGet-TrackedIssues keyed each marker to the LAST issue in the listing. The REST\nissues listing (state=all) is newest first, so with a closed old summary and an\nopen newer one the map kept the CLOSED issue, and every weekly run filed a\nduplicate summary issue. Prefer the OPEN issue for a marker, and between two of\nthe same state the higher number. Port of Chris-Wolfgang/repo-template#719.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-10-15T18:05:31Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/7d4579976929e1bcf86205d7d6ec1b6ae3e2aba0"
        },
        "date": 1792304684642,
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
          "id": "cb61a04adcd046135501c5dc2f0cf21614fa67f9",
          "message": "ci(shadow): compare against the latest release in the same job; re-check, then file an issue (#166) (#363)\n\nshadow.yaml now resolves the baseline release (latest stable on nuget.org,\nor a workflow_dispatch input), runs samples/ShadowWorkloads against it\n(ShadowLibraryVersion env var) and against this commit, checks from the\n\"Library under test\" line that each side ran the intended build, and\ncompares the two with compare.py. Latency (20 %) and allocation (50 %)\nare both gated, thresholds configurable per metric via dispatch inputs /\nSHADOW_* env. A regression must reproduce on a second fresh pair of runs;\nthen the run fails and opens (or comments on) a \"Shadow workloads\nregressed vs <version>\" issue with the markdown report.\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-07T03:27:27Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/cb61a04adcd046135501c5dc2f0cf21614fa67f9"
        },
        "date": 1794103454290,
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
          "id": "d5103978f50177be3bcbdfad7c25f091d746bcac",
          "message": "fix(ci): write normalized test results with LF on Windows runners (#410)\n\nPython text mode writes CRLF on Windows, so every line of the Windows result file differed\nfrom the Linux baseline in the Cross-Platform Differential workflow (114 of 114 lines, 114 CR\nbytes against 0) although the two result sets are identical once the CRs are stripped.\n\nCo-authored-by: Claude Sonnet 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-08T02:26:04Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/d5103978f50177be3bcbdfad7c25f091d746bcac"
        },
        "date": 1794142637503,
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
          "id": "3c75183c7cda42515d712ef804d33b30b95d3bff",
          "message": "Bump the dotnet-dependencies group with 2 updates (#425)\n\nBumps dotnet-reportgenerator-globaltool from 5.5.12 to 5.6.0\nBumps Meziantou.Analyzer from 3.0.322 to 3.0.324\n\n---\nupdated-dependencies:\n- dependency-name: dotnet-reportgenerator-globaltool\n  dependency-version: 5.6.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: Meziantou.Analyzer\n  dependency-version: 3.0.324\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-11-11T13:54:01Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/3c75183c7cda42515d712ef804d33b30b95d3bff"
        },
        "date": 1794410004035,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score (tests/Wolfgang.Extensions.ICollection.Tests.Unit)",
            "value": 100,
            "unit": "%"
          }
        ]
      }
    ]
  }
}