using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Extensions.ICollection.Tests.Concurrency;

/// <summary>
/// Concurrency stress tests (#170).
/// </summary>
/// <remarks>
/// <para>
/// Thread-safety contract: every method is stateless (no static field, no cache), and none is
/// thread-safe with respect to its <em>receiver</em>. Like the BCL collections it extends,
/// concurrent mutation of one collection needs external synchronisation (each method's XML
/// docs say so). These tests check both halves on the real thread pool:
/// <list type="bullet">
/// <item><description>many workers, each on its own collections, never interfere (no hidden shared state);</description></item>
/// <item><description>workers sharing one collection under a caller-held lock get exactly the sequential result (the documented "external synchronisation" contract is enough).</description></item>
/// </list>
/// </para>
/// <para>
/// <c>STRESS_ITERATIONS</c> scales the rounds: 100 by default (fast enough for every
/// <c>dotnet test</c>), more on the weekly <c>concurrency.yaml</c> run. Coyote was considered
/// and not used. These methods are synchronous and hold no locks or shared state, so systematic
/// interleaving exploration has nothing to explore beyond what the receiver collection does;
/// the thread-pool stress below is the proportionate check.
/// </para>
/// </remarks>
public sealed class ConcurrencyStressTests
{
    private static readonly int StressIterations = GetStressIterations();

    private static readonly int Workers = Math.Clamp(Environment.ProcessorCount * 2, 4, 16);



    [Fact]
    public async Task All_methods_when_workers_use_independent_collections_do_not_interfere()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await Task.WhenAll
            (
                Enumerable
                    .Range(0, Workers)
                    .Select(worker => Task.Run(() => RunIndependentWorkload(worker)))
            );

            for (var worker = 0; worker < Workers; worker++)
            {
                Assert.Equal(ExpectedIndependentResult(worker), results[worker]);
            }
        }
    }



    [Fact]
    public async Task AddIfNotContains_when_workers_share_a_locked_set_adds_each_value_exactly_once()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var shared = new HashSet<int>();
            var gate = new object();

            var addedPerWorker = await Task.WhenAll
            (
                Enumerable
                    .Range(0, Workers)
                    .Select(worker => Task.Run(() =>
                    {
                        var added = 0;
                        foreach (var value in Enumerable.Range(worker * 10, 50))
                        {
                            lock (gate)
                            {
                                added += shared.AddIfNotContains(value) ? 1 : 0;
                            }
                        }

                        return added;
                    }))
            );

            var expectedDistinct = Enumerable.Range(0, Workers).SelectMany(w => Enumerable.Range(w * 10, 50)).Distinct().Count();
            Assert.Equal(expectedDistinct, shared.Count);
            Assert.Equal(expectedDistinct, addedPerWorker.Sum());
        }
    }



    [Fact]
    public async Task AddRange_and_RemoveWhere_when_workers_share_a_locked_list_match_the_sequential_result()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var shared = new List<int>();
            var gate = new object();

            await Task.WhenAll
            (
                Enumerable
                    .Range(0, Workers)
                    .Select(worker => Task.Run(() =>
                    {
                        lock (gate)
                        {
                            shared.AddRange(Enumerable.Range(worker * 100, 100));
                        }

                        lock (gate)
                        {
                            shared.RemoveWhere(n => n >= worker * 100 && n < (worker * 100) + 50);
                        }
                    }))
            );

            var expected = Enumerable.Range(0, Workers).SelectMany(w => Enumerable.Range((w * 100) + 50, 50)).OrderBy(n => n);
            Assert.Equal(expected, shared.OrderBy(n => n));
        }
    }



    private static int GetStressIterations()
    {
        var raw = Environment.GetEnvironmentVariable("STRESS_ITERATIONS");
        return int.TryParse(raw, out var parsed) && parsed > 0 ? parsed : 100;
    }



    private static string RunIndependentWorkload(int worker)
    {
        ICollection<int> list = new List<int>();
        list.AddRange(Enumerable.Range(worker, 20));
        list.AddRangeIf(Enumerable.Range(100, 10), n => n % 2 == 0);
        list.RemoveRange(new[] { worker, worker + 1 });
        var removed = list.RemoveWhere(n => n % 3 == 0);
        var added = list.AddIfNotContains(Enumerable.Range(worker, 5));

        ICollection<int> set = new HashSet<int>(list);
        var setRemoved = set.RemoveWhere(n => n > 100);
        set.ReplaceAll(set.Where(n => n % 2 == 1).ToList());

        return string.Join(",", list) + "|" + removed + "|" + added + "|" + setRemoved + "|"
            + string.Join(",", set.OrderBy(n => n)) + "|" + set.IsEmpty() + list.IsNotEmpty();
    }



    private static string ExpectedIndependentResult(int worker)
    {
        // The same workload on a fresh thread with nothing else running: the reference the
        // concurrent run must reproduce exactly.
        return RunIndependentWorkload(worker);
    }
}
