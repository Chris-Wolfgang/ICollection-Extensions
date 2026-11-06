using System;
using System.Collections.Generic;
using System.Linq;
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
    public void AddRange_appends_items_in_order()
    {
        Pair.Sample
        (
            t =>
            {
                ICollection<int> source = new List<int>(t.Source);
                source.AddRange(t.Items);
                return source.SequenceEqual(t.Source.Concat(t.Items));
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void AddRangeIf_appends_exactly_the_matching_items()
    {
        Pair.Select(Gen.Int[-20, 20]).Sample
        (
            t =>
            {
                var ((sourceItems, items), threshold) = t;
                ICollection<int> source = new List<int>(sourceItems);
                source.AddRangeIf(items, n => n > threshold);
                return source.SequenceEqual(sourceItems.Concat(items.Where(n => n > threshold)));
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void RemoveRange_matches_removing_each_item_once()
    {
        Pair.Sample
        (
            t =>
            {
                ICollection<int> source = new List<int>(t.Source);
                var expected = new List<int>(t.Source);
                t.Items.ForEach(item => expected.Remove(item));
                source.RemoveRange(t.Items);
                return source.SequenceEqual(expected);
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void RemoveWhere_matches_List_RemoveAll_on_both_paths()
    {
        Items.Select(Gen.Int[-20, 20]).Sample
        (
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
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void ReplaceAll_leaves_exactly_the_new_items()
    {
        Pair.Sample
        (
            t =>
            {
                ICollection<int> source = new List<int>(t.Source);
                source.ReplaceAll(t.Items);
                return source.SequenceEqual(t.Items);
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void AddIfNotContains_adds_each_new_value_once_and_counts_it()
    {
        Pair.Sample
        (
            t =>
            {
                ICollection<int> list = new List<int>(t.Source);
                ICollection<int> set = new HashSet<int>(t.Source);
                var expectedAdded = t.Items.Distinct().Where(item => !t.Source.Contains(item)).ToList();

                return list.AddIfNotContains(t.Items) == expectedAdded.Count
                    && list.SequenceEqual(t.Source.Concat(expectedAdded))
                    && set.AddIfNotContains(t.Items) == expectedAdded.Count
                    && new HashSet<int>(t.Source.Concat(expectedAdded)).SetEquals(set);
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    [Fact]
    public void IsEmpty_and_IsNotEmpty_agree_with_Count()
    {
        Items.Sample
        (
            items =>
            {
                ICollection<int> source = new List<int>(items);
                return source.IsEmpty() == (items.Count == 0)
                    && source.IsNotEmpty() == (items.Count > 0);
            },
            iter: FuzzIterations,
            time: FuzzTimeSeconds
        );
    }



    private static int GetEnvInt(string name, int fallback)
    {
        var raw = Environment.GetEnvironmentVariable(name);
        return int.TryParse(raw, out var parsed) && parsed > 0 ? parsed : fallback;
    }
}
