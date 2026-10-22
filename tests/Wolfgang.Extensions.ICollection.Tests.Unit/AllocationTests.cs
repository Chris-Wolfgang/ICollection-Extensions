#if NETCOREAPP3_0_OR_GREATER
using System;
using System.Collections.Generic;
using Xunit;

namespace Wolfgang.Extensions.ICollection.Tests.Unit;

/// <summary>
/// Pins the call sites that are expected to allocate nothing, measured with
/// <see cref="GC.GetAllocatedBytesForCurrentThread"/> (available from .NET Core 3.0, so the
/// class compiles only on those targets).
/// </summary>
/// <remarks>
/// Zero-allocation contract (every other method allocates by design: an enumerator over
/// <c>items</c>, a self-aliasing snapshot, or the temporary match list in <c>RemoveWhere</c>):
/// <list type="bullet">
/// <item><description><c>IsEmpty</c> and <c>IsNotEmpty</c> on any receiver: a single <c>Count</c> read.</description></item>
/// <item><description><c>AddIfNotContains(T)</c> on an <see cref="ISet{T}"/>: one <c>ISet.Add</c> call.</description></item>
/// <item><description><c>AddIfNotContains(T)</c> on a <see cref="List{T}"/> with spare capacity, or when the item is already present.</description></item>
/// </list>
/// Each test warms the call up first so JIT work is not counted, then measures
/// <see cref="Iterations"/> calls. The budget is less than one byte per call: a one-off runtime
/// allocation (a type-check cache or comparer initialised on first use; seen as 40 bytes once on
/// netcoreapp3.1) stays under it, while a regression of even one object per call (at least 24
/// bytes, so 24,000 here) fails.
/// </remarks>
public class AllocationTests
{
    private const int Iterations = 1_000;
    private const int WarmUpCalls = 10;



    [Fact]
    public void IsEmpty_when_called_repeatedly_allocates_nothing()
    {
        ICollection<int> source = new List<int> { 1, 2, 3 };

        var allocated = MeasureAllocatedBytes(() => source.IsEmpty());

        AssertNoPerCallAllocation(allocated);
    }



    [Fact]
    public void IsNotEmpty_when_called_repeatedly_allocates_nothing()
    {
        ICollection<int> source = new HashSet<int>();

        var allocated = MeasureAllocatedBytes(() => source.IsNotEmpty());

        AssertNoPerCallAllocation(allocated);
    }



    [Fact]
    public void AddIfNotContains_single_when_source_is_ISet_and_item_is_present_allocates_nothing()
    {
        ICollection<int> source = new HashSet<int> { 1, 2, 3 };

        var allocated = MeasureAllocatedBytes(() => source.AddIfNotContains(2));

        AssertNoPerCallAllocation(allocated);
    }



    [Fact]
    public void AddIfNotContains_single_when_source_is_List_and_item_is_present_allocates_nothing()
    {
        ICollection<int> source = new List<int> { 1, 2, 3 };

        var allocated = MeasureAllocatedBytes(() => source.AddIfNotContains(3));

        AssertNoPerCallAllocation(allocated);
    }



    [Fact]
    public void AddIfNotContains_single_when_List_has_spare_capacity_allocates_nothing()
    {
        var list = new List<int>(Iterations * 2);
        ICollection<int> source = list;
        var next = 0;

        var allocated = MeasureAllocatedBytes(() => source.AddIfNotContains(next++));

        AssertNoPerCallAllocation(allocated);
        Assert.Equal(WarmUpCalls + Iterations, list.Count);
    }



    private static long MeasureAllocatedBytes(Func<bool> call)
    {
        // Warm-up: JIT-compile the call path (and the lambda) before measuring.
        for (var i = 0; i < WarmUpCalls; i++)
        {
            call();
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < Iterations; i++)
        {
            call();
        }

        return GC.GetAllocatedBytesForCurrentThread() - before;
    }



    private static void AssertNoPerCallAllocation(long allocatedBytes)
    {
        Assert.InRange(allocatedBytes, 0L, Iterations - 1L);
    }
}
#endif
