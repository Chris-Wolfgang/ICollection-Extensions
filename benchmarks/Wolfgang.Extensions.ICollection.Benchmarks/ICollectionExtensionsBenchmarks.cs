using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Wolfgang.Extensions.ICollection;

namespace Wolfgang.Extensions.ICollection.Benchmarks;

/// <summary>
/// Microbenchmarks for the public extension methods. The harness pairs
/// fast-path and slow-path scenarios per method so the gh-pages chart
/// surfaces any future regression in the targeted optimizations
/// (e.g. <c>RemoveWhere</c>'s <c>HashSet&lt;T&gt;</c> branch,
/// <c>AddIfNotContains</c>'s <c>ISet&lt;T&gt;</c> branch,
/// <c>AddRange</c>'s pre-allocation branch). Every receiver is typed as
/// <c>ICollection&lt;T&gt;</c> at the call site so the extension method
/// wins overload resolution against any concrete-type instance method
/// (e.g. <c>List&lt;T&gt;.AddRange</c> would otherwise bind first). The
/// MemoryDiagnoser is enabled so any future refactor that introduces
/// allocation surfaces in the gh-pages benchmark chart immediately.
/// </summary>
[MemoryDiagnoser]
public class ICollectionExtensionsBenchmarks
{
    // 1024 items is enough to exercise the pre-allocation path's win over
    // repeated resize on the slow path while staying well under 1 ms per
    // op so the BDN ShortRun finishes quickly.
    private const int Count = 1024;

    private readonly int[] _itemsToAdd = new int[Count];



    [Benchmark]
    public ICollection<int> AddRange_fast_path_List_target()
    {
        // Fast path: target is List<T> (the only ICollection<T> with
        // settable Capacity) AND _itemsToAdd is an int[], so the
        // 'items is ICollection<T>' check succeeds and the pre-allocation
        // branch fires. The receiver is typed as ICollection<T> so the
        // extension wins overload resolution — without this typing,
        // 'target.AddRange(...)' would bind to List<T>.AddRange instead.
        ICollection<int> target = new List<int>();
        target.AddRange(_itemsToAdd);
        return target;
    }



    [Benchmark]
    public ICollection<int> AddRange_slow_path_LinkedList_target()
    {
        // Slow path: LinkedList<T> is ICollection<T> but not List<T>, so
        // the capacity pre-allocation branch does not fire. Iteration
        // falls through to one-by-one Add().
        ICollection<int> target = new LinkedList<int>();
        target.AddRange(_itemsToAdd);
        return target;
    }



    [Benchmark]
    public bool IsEmpty_on_empty_collection()
    {
        ICollection<int> source = new List<int>();
        return source.IsEmpty();
    }



    [Benchmark]
    // int[] implements ICollection<int>, so the IsEmpty extension binds
    // directly — no cast/shim required (unlike AddRange, where the
    // List<T>.AddRange instance method wins resolution and we have to
    // type the receiver as ICollection<int>).
    public bool IsEmpty_on_nonempty_collection() => _itemsToAdd.IsEmpty();



    [Benchmark]
    public bool IsNotEmpty_on_empty_collection()
    {
        ICollection<int> source = new List<int>();
        return source.IsNotEmpty();
    }



    [Benchmark]
    public bool IsNotEmpty_on_nonempty_collection() => _itemsToAdd.IsNotEmpty();



    // -- RemoveRange ------------------------------------------------------

    [Benchmark]
    public ICollection<int> RemoveRange_List_target()
    {
        // Parity with AddRange's List/LinkedList split. List<T>.Remove
        // is O(n) (LastIndexOf + shift), so RemoveRange across n items
        // becomes O(n²). Pairing with the LinkedList variant below makes
        // a future O(1)-set-membership shortcut for RemoveRange visible
        // in the chart.
        ICollection<int> target = new List<int>(_itemsToAdd);
        target.RemoveRange(_itemsToAdd);
        return target;
    }



    [Benchmark]
    public ICollection<int> RemoveRange_LinkedList_target()
    {
        ICollection<int> target = new LinkedList<int>(_itemsToAdd);
        target.RemoveRange(_itemsToAdd);
        return target;
    }



    // -- AddRangeIf -------------------------------------------------------

    [Benchmark]
    public ICollection<int> AddRangeIf_all_match()
    {
        // Pays the predicate cost per item, then takes the same path as
        // AddRange. Predicate-always-true measures the upper bound of
        // the filtered-append cost.
        ICollection<int> target = new List<int>();
        target.AddRangeIf(_itemsToAdd, _ => true);
        return target;
    }



    [Benchmark]
    public ICollection<int> AddRangeIf_none_match()
    {
        // Predicate-always-false measures the per-item predicate cost
        // without any Add() — a regression in predicate dispatch would
        // appear here without being masked by allocation noise.
        ICollection<int> target = new List<int>();
        target.AddRangeIf(_itemsToAdd, _ => false);
        return target;
    }



    // -- RemoveWhere ------------------------------------------------------

    [Benchmark]
    public int RemoveWhere_fast_path_HashSet_target()
    {
        // Fast path: HashSet<T>.RemoveWhere is the native single-pass
        // implementation. The extension's 'source is HashSet<T>' check
        // routes here and avoids the temp-list allocation + double pass
        // the slow path takes.
        var set = new HashSet<int>(_itemsToAdd);
        ICollection<int> source = set;
        return source.RemoveWhere(n => (n & 1) == 0);
    }



    [Benchmark]
    public int RemoveWhere_slow_path_List_target()
    {
        // Slow path: List<T> is ICollection<T> but not HashSet<T>, so
        // the extension materialises matches into a temp list and then
        // calls source.Remove() per item (List.Remove is O(n)).
        ICollection<int> source = new List<int>(_itemsToAdd);
        return source.RemoveWhere(n => (n & 1) == 0);
    }



    // -- ReplaceAll -------------------------------------------------------

    [Benchmark]
    public ICollection<int> ReplaceAll_List_target()
    {
        ICollection<int> target = new List<int>(_itemsToAdd);
        target.ReplaceAll(_itemsToAdd);
        return target;
    }



    [Benchmark]
    public ICollection<int> ReplaceAll_LinkedList_target()
    {
        ICollection<int> target = new LinkedList<int>(_itemsToAdd);
        target.ReplaceAll(_itemsToAdd);
        return target;
    }



    // -- AddIfNotContains(T) ---------------------------------------------

    [Benchmark]
    public int AddIfNotContains_single_fast_path_HashSet_target()
    {
        // Fast path: ISet<T>.Add is the native single-lookup contract.
        // The extension routes HashSet<T> through here, skipping the
        // Contains+Add double-lookup the slow path takes.
        var set = new HashSet<int>();
        ICollection<int> target = set;
        var added = 0;
        for (var i = 0; i < Count; i++)
        {
            if (target.AddIfNotContains(i))
            {
                added++;
            }
        }
        return added;
    }



    [Benchmark]
    public int AddIfNotContains_single_slow_path_List_target()
    {
        // Slow path: List<T>.Contains is O(n), so this is O(n²) over
        // the loop. The contrast with the HashSet path above makes the
        // optimization's value visible in the chart.
        ICollection<int> target = new List<int>();
        var added = 0;
        for (var i = 0; i < Count; i++)
        {
            if (target.AddIfNotContains(i))
            {
                added++;
            }
        }
        return added;
    }



    // -- AddIfNotContains(IEnumerable<T>) --------------------------------

    [Benchmark]
    public int AddIfNotContains_many_HashSet_target()
    {
        // Many-arg path: delegates to single-arg per item, so HashSet
        // target inherits the same ISet fast path through Count().
        ICollection<int> target = new HashSet<int>();
        return target.AddIfNotContains(_itemsToAdd);
    }



    [Benchmark]
    public int AddIfNotContains_many_List_target()
    {
        ICollection<int> target = new List<int>();
        return target.AddIfNotContains(_itemsToAdd);
    }
}
