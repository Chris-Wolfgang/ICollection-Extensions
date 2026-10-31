using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CsCheck;
using Xunit;

namespace Wolfgang.Extensions.ICollection.Tests.Fuzz;

/// <summary>
/// Property-based fuzzing of every mutating method against a reference implementation (#165).
/// </summary>
/// <remarks>
/// <para>
/// Each property runs <see cref="FuzzIterations"/> random cases: 1,000 by default so a local
/// <c>dotnet test</c> and the PR stages stay fast, and 100,000 (with a time budget) on the weekly
/// <c>fuzz.yaml</c> run, through the <c>FUZZ_ITERATIONS</c> / <c>FUZZ_TIME_SECONDS</c> variables.
/// CsCheck shrinks a failure to a minimal case and prints a seed; set <c>CsCheck_Seed</c> to
/// replay it deterministically.
/// </para>
/// <para>
/// Before its random cases, every property replays the seeds recorded for it in
/// <c>replay/seeds.txt</c> (see <see cref="FuzzReplay"/>), so a failure found by the weekly run
/// becomes a deterministic regression case once its seed is committed there.
/// </para>
/// <para>
/// Every property returns a single expression, so a passing run executes every line (the test
/// assemblies are held to 100 % line coverage).
/// </para>
/// </remarks>
public sealed class FuzzProperties
{
    // Small value range so duplicates and "already present" cases are common.
    private static readonly Gen<List<int>> Items = Gen.Int[-20, 20].List[0, 60];

    private static readonly Gen<(List<int> Source, List<int> Items)> Pair = Gen.Select(Items, Items);

    private static int FuzzIterations => GetEnvInt("FUZZ_ITERATIONS", 1_000);

    // CsCheck's `time` parameter is int seconds; -1 means "no time budget".
    private static int FuzzTimeSeconds => GetEnvInt("FUZZ_TIME_SECONDS", -1);



    [Fact]
    public async Task AddRange_appends_items_in_order()
    {
        await CheckAsync
        (
            Pair,
            t =>
            {
                ICollection<int> source = new List<int>(t.Source);
                source.AddRange(t.Items);
                return source.SequenceEqual(t.Source.Concat(t.Items));
            }
        );
    }



    [Fact]
    public async Task AddRangeIf_appends_exactly_the_matching_items()
    {
        await CheckAsync
        (
            Pair.Select(Gen.Int[-20, 20]),
            t =>
            {
                var ((sourceItems, items), threshold) = t;
                ICollection<int> source = new List<int>(sourceItems);
                source.AddRangeIf(items, n => n > threshold);
                return source.SequenceEqual(sourceItems.Concat(items.Where(n => n > threshold)));
            }
        );
    }



    [Fact]
    public async Task RemoveRange_matches_removing_each_item_once()
    {
        await CheckAsync
        (
            Pair,
            t =>
            {
                ICollection<int> source = new List<int>(t.Source);
                var expected = new List<int>(t.Source);
                t.Items.ForEach(item => expected.Remove(item));
                source.RemoveRange(t.Items);
                return source.SequenceEqual(expected);
            }
        );
    }



    [Fact]
    public async Task RemoveWhere_matches_List_RemoveAll_on_both_paths()
    {
        await CheckAsync
        (
            Items.Select(Gen.Int[-20, 20]),
            t =>
            {
                var (sourceItems, threshold) = t;
                ICollection<int> list = new List<int>(sourceItems);
                ICollection<int> set = new HashSet<int>(sourceItems);
                var expectedList = new List<int>(sourceItems);
                var expectedSet = new HashSet<int>(sourceItems);
                var expectedListCount = expectedList.RemoveAll(n => n < threshold);
                var expectedSetCount = expectedSet.RemoveWhere(n => n < threshold);

                return list.RemoveWhere(n => n < threshold) == expectedListCount
                    && list.SequenceEqual(expectedList)
                    && set.RemoveWhere(n => n < threshold) == expectedSetCount
                    && expectedSet.SetEquals(set);
            }
        );
    }



    [Fact]
    public async Task ReplaceAll_leaves_exactly_the_new_items()
    {
        await CheckAsync
        (
            Pair,
            t =>
            {
                ICollection<int> source = new List<int>(t.Source);
                source.ReplaceAll(t.Items);
                return source.SequenceEqual(t.Items);
            }
        );
    }



    [Fact]
    public async Task AddIfNotContains_adds_each_new_value_once_and_counts_it()
    {
        await CheckAsync
        (
            Pair,
            t =>
            {
                ICollection<int> list = new List<int>(t.Source);
                ICollection<int> set = new HashSet<int>(t.Source);
                var expectedAdded = t.Items.Distinct().Where(item => !t.Source.Contains(item)).ToList();

                return list.AddIfNotContains(t.Items) == expectedAdded.Count
                    && list.SequenceEqual(t.Source.Concat(expectedAdded))
                    && set.AddIfNotContains(t.Items) == expectedAdded.Count
                    && new HashSet<int>(t.Source.Concat(expectedAdded)).SetEquals(set);
            }
        );
    }



    [Fact]
    public async Task IsEmpty_and_IsNotEmpty_agree_with_Count()
    {
        await CheckAsync
        (
            Items,
            items =>
            {
                ICollection<int> source = new List<int>(items);
                return source.IsEmpty() == (items.Count == 0)
                    && source.IsNotEmpty() == (items.Count > 0);
            }
        );
    }



    private static async Task CheckAsync<T>
    (
        Gen<T> gen,
        Func<T, bool> property,
        [CallerMemberName] string name = ""
    )
    {
        // Replay the committed corpus first (one deterministic case per seed), so a recorded
        // regression fails with its own seed before any random case runs.
        foreach (var seed in await FuzzReplay.SeedsForAsync(name))
        {
            gen.Sample(property, seed: seed, iter: 1);
        }

        gen.Sample(property, iter: FuzzIterations, time: FuzzTimeSeconds);
    }



    private static int GetEnvInt(string name, int fallback)
    {
        var raw = Environment.GetEnvironmentVariable(name);
        return int.TryParse(raw, out var parsed) && parsed > 0 ? parsed : fallback;
    }
}
