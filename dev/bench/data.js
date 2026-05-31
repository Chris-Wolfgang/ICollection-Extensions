window.BENCHMARK_DATA = {
  "lastUpdate": 1780271558696,
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
      }
    ]
  }
}