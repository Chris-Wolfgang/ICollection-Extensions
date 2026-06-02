window.BENCHMARK_DATA = {
  "lastUpdate": 1780366055573,
  "repoUrl": "https://github.com/Chris-Wolfgang/ICollection-Extensions",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "id": "dd7837b454ad6dae8593cf79b6e33b0cae538200",
          "message": "benchmarks.yaml: fix workflow validation — move hashFiles() out of job-level if\n\nGitHub's expression evaluator rejects hashFiles() in a job-level\n'if:' condition with 'Unrecognized function: hashFiles'. The\nfunction is only valid in step-level 'if:' and 'with:' expressions.\nEvery push to main since the workflow shipped failed at parse time\n(0 jobs run, no logs, 'workflow file issue' in the UI).\n\nReplaced the job-level guard with:\n1. A 'Detect benchmark project' step that uses plain bash + [ -f \"…\" ]\n   to set 'exists=true|false' as a step output.\n2. Step-level 'if: steps.detect.outputs.exists == \"true\"' on every\n   subsequent step.\n\nWhen the benchmark project isn't present the job still completes\ngreen (all subsequent steps skipped, ::notice:: emitted).\n\nAlso added a comment block in the workflow explaining the\nhashFiles-at-job-level pitfall so a future contributor doesn't\nreintroduce the same parse failure.",
          "timestamp": "2026-05-31T23:45:50Z",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/dd7837b454ad6dae8593cf79b6e33b0cae538200"
        },
        "date": 1780271287041,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_fast_path_List_target",
            "value": 2492.3515605926514,
            "unit": "ns",
            "range": "± 8.130913682992775"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_slow_path_LinkedList_target",
            "value": 11096.889892578125,
            "unit": "ns",
            "range": "± 156.74464017822723"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_empty_collection",
            "value": 0.0003931062916914622,
            "unit": "ns",
            "range": "± 0.00036129480490832035"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_nonempty_collection",
            "value": 0.0016666166484355927,
            "unit": "ns",
            "range": "± 0.0006475377203428368"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_empty_collection",
            "value": 0.00036523987849553424,
            "unit": "ns",
            "range": "± 0.0002086406955926646"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_nonempty_collection",
            "value": 0.0027259389559427896,
            "unit": "ns",
            "range": "± 0.00016817221319307465"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "c96f3a276202f9fa27200e585afd211c7c94aa8a",
          "message": "Merge pull request #145 from Chris-Wolfgang/fix/benchmarks-hashfiles-job-level\n\nfix(ci): benchmarks.yaml — hashFiles() at job-level if breaks workflow parse",
          "timestamp": "2026-05-31T19:50:15-04:00",
          "tree_id": "0b7b642ce3fc1ea34f8ee71b6a36f190fdd159f2",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/c96f3a276202f9fa27200e585afd211c7c94aa8a"
        },
        "date": 1780271557823,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_fast_path_List_target",
            "value": 1155.6133778889973,
            "unit": "ns",
            "range": "± 4.44670194326183"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_slow_path_LinkedList_target",
            "value": 13311.983184814453,
            "unit": "ns",
            "range": "± 244.90213138201108"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_empty_collection",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_nonempty_collection",
            "value": 0.3353978556891282,
            "unit": "ns",
            "range": "± 0.0018828185061436783"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_empty_collection",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_nonempty_collection",
            "value": 0.3495696981747945,
            "unit": "ns",
            "range": "± 0.0017592185307739297"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "4858c0cecb9280d4e46930a86eb9501f5ce53dc0",
          "message": "Merge pull request #148 from Chris-Wolfgang/fix/build-hygiene\n\nRollup: build hygiene + test cleanup + XML docs + examples + canonical workflow tweaks",
          "timestamp": "2026-06-01T21:12:50-04:00",
          "tree_id": "d6e8c23933686079899e7be08b56100e9f9a23b9",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/4858c0cecb9280d4e46930a86eb9501f5ce53dc0"
        },
        "date": 1780362936869,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_fast_path_List_target",
            "value": 1188.8000882466633,
            "unit": "ns",
            "range": "± 6.463009679706386"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_slow_path_LinkedList_target",
            "value": 13458.017669677734,
            "unit": "ns",
            "range": "± 76.10448791900393"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_empty_collection",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_nonempty_collection",
            "value": 0.33591917902231216,
            "unit": "ns",
            "range": "± 0.0010359936303535589"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_empty_collection",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_nonempty_collection",
            "value": 0.35076095908880234,
            "unit": "ns",
            "range": "± 0.001444864997181056"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d8cdf03c21faed7d790973fc6d6483ce537e3f55",
          "message": "Merge pull request #157 from Chris-Wolfgang/stack/benchmark-coverage\n\nbenchmarks: cover the 6 missing public methods",
          "timestamp": "2026-06-01T21:29:33-04:00",
          "tree_id": "056cfc1b8bee3418b7f997648a58b0e65050dec4",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/d8cdf03c21faed7d790973fc6d6483ce537e3f55"
        },
        "date": 1780363991454,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_fast_path_List_target",
            "value": 2581.645595550537,
            "unit": "ns",
            "range": "± 4.554548038595373"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_slow_path_LinkedList_target",
            "value": 13138.30800374349,
            "unit": "ns",
            "range": "± 115.10855158363023"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_empty_collection",
            "value": 0.0003443285822868347,
            "unit": "ns",
            "range": "± 0.00030829312113697356"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_nonempty_collection",
            "value": 0.006571220854918162,
            "unit": "ns",
            "range": "± 0.00788155148576164"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_empty_collection",
            "value": 0.0000144404669602712,
            "unit": "ns",
            "range": "± 0.00002501162246020942"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_nonempty_collection",
            "value": 0.004632617036501567,
            "unit": "ns",
            "range": "± 0.006226291877450644"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveRange_List_target",
            "value": 44839.90928141276,
            "unit": "ns",
            "range": "± 59.4112460919562"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveRange_LinkedList_target",
            "value": 19199.86829630534,
            "unit": "ns",
            "range": "± 162.66838209619806"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRangeIf_all_match",
            "value": 3638.549067179362,
            "unit": "ns",
            "range": "± 13.432111562737981"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRangeIf_none_match",
            "value": 2029.6561101277669,
            "unit": "ns",
            "range": "± 42.95552395651018"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveWhere_fast_path_HashSet_target",
            "value": 9702.036885579428,
            "unit": "ns",
            "range": "± 22.471177647473496"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveWhere_slow_path_List_target",
            "value": 28370.751047770184,
            "unit": "ns",
            "range": "± 31.553236286012826"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.ReplaceAll_List_target",
            "value": 2707.5427843729653,
            "unit": "ns",
            "range": "± 4.926887602947151"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.ReplaceAll_LinkedList_target",
            "value": 30879.451090494793,
            "unit": "ns",
            "range": "± 307.18155114409217"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_single_fast_path_HashSet_target",
            "value": 12752.000508626303,
            "unit": "ns",
            "range": "± 41.51292833872464"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_single_slow_path_List_target",
            "value": 24714.898468017578,
            "unit": "ns",
            "range": "± 38.27995162883487"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_many_HashSet_target",
            "value": 15207.835866292318,
            "unit": "ns",
            "range": "± 251.40876092598046"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_many_List_target",
            "value": 27019.372884114582,
            "unit": "ns",
            "range": "± 60.29836998521603"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "9b1b3d0d951b1abc8dfa5444ec96c7e4b7b98865",
          "message": "Address Copilot on #158: fix DOCFX-VERSION-PICKER deploy flow + icon.ico CHANGELOG entry\n\nTwo findings:\n\n1. docs/DOCFX-VERSION-PICKER.md 'How it gets to gh-pages' diagram\n   claimed 'push to main / release tag' triggers docfx.yaml. In\n   reality docfx.yaml only has workflow_call (invoked by release.yaml\n   after a GitHub Release is published) and workflow_dispatch\n   (manual). Reworked the diagram to show the actual chain\n   (Release published → release.yaml → docfx.yaml) and added an\n   explicit note that a plain push to main does NOT redeploy docs.\n\n2. CHANGELOG [0.3.1] said 'assets/icon.ico — moved out of the src\n   project directory'. The icon is actually still at\n   src/.../icon.ico (referenced by <ApplicationIcon> and packed via\n   <Content Include='icon.ico'/>) — the assets/icon.ico is an\n   additional fleet-asset copy, not a relocation. Reworded the\n   bullet to describe the actual change accurately.",
          "timestamp": "2026-06-01T22:03:35-04:00",
          "tree_id": "ce98c522da9f7d755574b60f1e7abacb622d47da",
          "url": "https://github.com/Chris-Wolfgang/ICollection-Extensions/commit/9b1b3d0d951b1abc8dfa5444ec96c7e4b7b98865"
        },
        "date": 1780366053826,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_fast_path_List_target",
            "value": 1137.3834438323975,
            "unit": "ns",
            "range": "± 4.753526819663082"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRange_slow_path_LinkedList_target",
            "value": 13061.87538655599,
            "unit": "ns",
            "range": "± 105.48619534550512"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_empty_collection",
            "value": 0.11459797248244286,
            "unit": "ns",
            "range": "± 0.0015375347061583843"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsEmpty_on_nonempty_collection",
            "value": 0.33570606634020805,
            "unit": "ns",
            "range": "± 0.0027583827731353233"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_empty_collection",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.IsNotEmpty_on_nonempty_collection",
            "value": 0.35298336669802666,
            "unit": "ns",
            "range": "± 0.0014725378495719535"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveRange_List_target",
            "value": 49283.12422688802,
            "unit": "ns",
            "range": "± 103.81450397036946"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveRange_LinkedList_target",
            "value": 18666.505696614582,
            "unit": "ns",
            "range": "± 53.14806166334312"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRangeIf_all_match",
            "value": 3300.9564170837402,
            "unit": "ns",
            "range": "± 8.014784161832667"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddRangeIf_none_match",
            "value": 995.8470331827799,
            "unit": "ns",
            "range": "± 2.4150101479894004"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveWhere_fast_path_HashSet_target",
            "value": 10597.036692301432,
            "unit": "ns",
            "range": "± 33.506728722014294"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.RemoveWhere_slow_path_List_target",
            "value": 34235.933766682945,
            "unit": "ns",
            "range": "± 139.39928002042654"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.ReplaceAll_List_target",
            "value": 1247.0608558654785,
            "unit": "ns",
            "range": "± 20.610115796928596"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.ReplaceAll_LinkedList_target",
            "value": 28630.81478881836,
            "unit": "ns",
            "range": "± 171.08137406748375"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_single_fast_path_HashSet_target",
            "value": 9863.388061523438,
            "unit": "ns",
            "range": "± 74.66774753033384"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_single_slow_path_List_target",
            "value": 34432.931884765625,
            "unit": "ns",
            "range": "± 103.58621304445742"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_many_HashSet_target",
            "value": 12610.656168619791,
            "unit": "ns",
            "range": "± 111.11265159711196"
          },
          {
            "name": "Wolfgang.Extensions.ICollection.Benchmarks.ICollectionExtensionsBenchmarks.AddIfNotContains_many_List_target",
            "value": 37013.573486328125,
            "unit": "ns",
            "range": "± 139.98686506801135"
          }
        ]
      }
    ]
  }
}