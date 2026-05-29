using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Wolfgang.Extensions.ICollection;

namespace Wolfgang.Extensions.ICollection.Benchmarks;

/// <summary>
/// Microbenchmarks for the public extension methods. <c>AddRange</c> has
/// two interesting shapes — the fast path where the source is also an
/// <c>IList&lt;T&gt;</c> and the appended sequence exposes
/// <c>ICollection&lt;T&gt;.Count</c> (capacity can be pre-allocated), and
/// the slow path where neither holds (one-by-one append). The
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
    public List<int> AddRange_to_List_from_IList()
    {
        // Fast path: both sides expose Count + IList semantics.
        var target = new List<int>();
        target.AddRange((IEnumerable<int>)_itemsToAdd);
        return target;
    }



    [Benchmark]
    public LinkedList<int> AddRange_to_LinkedList_from_IList()
    {
        // Slow path: LinkedList is ICollection<T> but not IList<T>, so the
        // capacity pre-allocation branch does not apply.
        var target = new LinkedList<int>();
        target.AddRange((IEnumerable<int>)_itemsToAdd);
        return target;
    }



    [Benchmark]
    public bool IsEmpty_on_empty_List() => new List<int>().IsEmpty();



    [Benchmark]
    public bool IsEmpty_on_nonempty_List() => _itemsToAdd.AsCollection().IsEmpty();



    [Benchmark]
    public bool IsNotEmpty_on_empty_List() => new List<int>().IsNotEmpty();



    [Benchmark]
    public bool IsNotEmpty_on_nonempty_List() => _itemsToAdd.AsCollection().IsNotEmpty();
}



/// <summary>
/// Small helper to coerce an array into the <see cref="ICollection{T}"/>
/// shape the extension methods take without allocating a new list.
/// </summary>
internal static class CollectionShim
{
    public static ICollection<T> AsCollection<T>(this T[] source) => source;
}
