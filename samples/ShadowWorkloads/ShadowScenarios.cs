using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Wolfgang.Extensions.ICollection;

namespace ShadowWorkloads;

/// <summary>
/// Realistic consumer workloads for shadow testing (#166). Unlike the curated microbenchmarks in
/// <c>benchmarks/</c>, each scenario strings several extension methods together the way an
/// application would, so the numbers describe a workload rather than one call. They double as
/// "real usage" documentation. <c>shadow.yaml</c> runs them nightly and compares them with
/// <c>docs/shadow-baseline.json</c>: allocation is gated (more than 50 % regression fails),
/// latency is advisory.
/// </summary>
[MemoryDiagnoser]
public class ShadowScenarios
{
    private const int Seed = 20261022;

    private int[][] _incomingIdBatches = [];

    private OrderLine[] _cartLines = [];

    private string[] _remoteSnapshot = [];

    private List<string> _localCache = [];

    private ICollection<int>[] _queues = [];



    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);

        // 50 batches of 200 ids drawn from 0..4,999: heavy overlap between batches, like
        // at-least-once message delivery.
        _incomingIdBatches = Enumerable
            .Range(0, 50)
            .Select(_ => Enumerable.Range(0, 200).Select(_ => random.Next(5_000)).ToArray())
            .ToArray();

        _cartLines = Enumerable
            .Range(0, 500)
            .Select(i => new OrderLine(i, random.Next(1, 5), Expired: random.Next(10) == 0))
            .ToArray();

        _remoteSnapshot = Enumerable.Range(0, 2_000).Select(i => "key-" + (i * 3)).ToArray();
        _localCache = Enumerable.Range(0, 2_000).Select(i => "key-" + (i * 2)).ToList();

        _queues = Enumerable
            .Range(0, 1_000)
            .Select(i => i % 3 == 0 ? (ICollection<int>)Array.Empty<int>() : new List<int> { i })
            .ToArray();
    }



    /// <summary>
    /// Idempotent ingest: de-duplicate incoming id batches into a HashSet with the bulk
    /// <c>AddIfNotContains</c> (ISet fast path), counting how many were new.
    /// </summary>
    [Benchmark]
    public int Scenario_DedupeMessageIdsIntoHashSet()
    {
        ICollection<int> seen = new HashSet<int>();
        var newIds = 0;
        foreach (var batch in _incomingIdBatches)
        {
            newIds += seen.AddIfNotContains(batch);
        }

        return newIds;
    }



    /// <summary>
    /// Shopping cart: load lines with <c>AddRange</c>, prune expired ones with
    /// <c>RemoveWhere</c>, then re-price by replacing the contents with <c>ReplaceAll</c>.
    /// </summary>
    [Benchmark]
    public int Scenario_CartLoadPruneAndReprice()
    {
        ICollection<OrderLine> cart = new List<OrderLine>();
        cart.AddRange(_cartLines);
        var pruned = cart.RemoveWhere(line => line.Expired);
        cart.ReplaceAll(cart.Select(line => line with { Quantity = line.Quantity * 2 }).ToList());
        return pruned + cart.Count;
    }



    /// <summary>
    /// Cache sync against a remote snapshot: drop keys the remote no longer has with
    /// <c>RemoveRange</c>, then add the remote's new keys with <c>AddRangeIf</c>.
    /// </summary>
    [Benchmark]
    public int Scenario_SyncLocalCacheWithRemoteSnapshot()
    {
        ICollection<string> cache = new List<string>(_localCache);
        var remote = new HashSet<string>(_remoteSnapshot, StringComparer.Ordinal);
        var local = new HashSet<string>(cache, StringComparer.Ordinal);

        cache.RemoveRange(local.Where(key => !remote.Contains(key)).ToList());
        cache.AddRangeIf(_remoteSnapshot, key => !local.Contains(key));
        return cache.Count;
    }



    /// <summary>
    /// Scheduler poll: scan many work queues (arrays and lists) and count the non-empty ones
    /// with <c>IsEmpty</c> / <c>IsNotEmpty</c>. This should not allocate at all.
    /// </summary>
    [Benchmark]
    public int Scenario_PollQueuesForWork()
    {
        var ready = 0;
        var idle = 0;
        foreach (var queue in _queues)
        {
            ready += queue.IsNotEmpty() ? 1 : 0;
            idle += queue.IsEmpty() ? 1 : 0;
        }

        return ready - idle;
    }
}

