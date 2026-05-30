using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Wolfgang.Extensions.ICollection;

namespace Wolfgang.Extensions.ICollection.Benchmarks;

/// <summary>
/// Microbenchmarks for the public extension methods. <c>AddRange</c> has
/// two interesting shapes — the fast path where the target is
/// <c>List&lt;T&gt;</c> and the appended sequence exposes
/// <c>ICollection&lt;T&gt;.Count</c> (capacity can be pre-allocated), and
/// the slow path where the target is some other <c>ICollection&lt;T&gt;</c>
/// (one-by-one append, no pre-allocation). Every receiver is typed as
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
}

