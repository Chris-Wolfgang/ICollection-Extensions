using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wolfgang.Extensions.ICollection;

// Native-AOT / trim smoke test (#175). Calls every public method on ICollectionExtensions
// from a PublishAot + PublishTrimmed consumer and checks real results, so a trimmed member or
// an AOT-incompatible code path shows up as a non-zero exit instead of a silent no-op. The
// library uses no reflection or dynamic code; this proves it rather than assuming it.

var failures = new List<string>();

Check("AddRange", () =>
{
    ICollection<int> list = new List<int> { 1 };
    list.AddRange(new[] { 2, 3 });
    Assert(list.SequenceEqual(new[] { 1, 2, 3 }), "AddRange did not append in order");
});

Check("AddRangeIf", () =>
{
    ICollection<int> list = new List<int>();
    list.AddRangeIf(Enumerable.Range(1, 6), n => n % 2 == 0);
    Assert(list.SequenceEqual(new[] { 2, 4, 6 }), "AddRangeIf added the wrong items");
});

Check("RemoveRange", () =>
{
    ICollection<string> list = new List<string> { "a", "b", "c" };
    list.RemoveRange(new[] { "b", "x" });
    Assert(list.SequenceEqual(new[] { "a", "c" }), "RemoveRange removed the wrong items");
});

Check("RemoveWhere (generic path)", () =>
{
    ICollection<int> list = new List<int> { 1, 2, 3, 4 };
    Assert(list.RemoveWhere(n => n > 2) == 2, "RemoveWhere returned the wrong count");
    Assert(list.SequenceEqual(new[] { 1, 2 }), "RemoveWhere left the wrong items");
});

Check("RemoveWhere (HashSet fast path)", () =>
{
    ICollection<int> set = new HashSet<int> { 1, 2, 3, 4 };
    Assert(set.RemoveWhere(n => n % 2 == 0) == 2, "HashSet RemoveWhere returned the wrong count");
    Assert(set.Count == 2 && set.Contains(1) && set.Contains(3), "HashSet RemoveWhere left the wrong items");
});

Check("ReplaceAll", () =>
{
    ICollection<int> list = new List<int> { 9, 9 };
    list.ReplaceAll(new[] { 1, 2 });
    Assert(list.SequenceEqual(new[] { 1, 2 }), "ReplaceAll did not replace the contents");
});

Check("AddIfNotContains(T)", () =>
{
    ICollection<string> list = new List<string> { "a" };
    ICollection<string> set = new SortedSet<string>(StringComparer.Ordinal) { "a" };
    Assert(list.AddIfNotContains("b") && !list.AddIfNotContains("a"), "List AddIfNotContains returned the wrong flags");
    Assert(set.AddIfNotContains("b") && !set.AddIfNotContains("a"), "ISet AddIfNotContains returned the wrong flags");
});

Check("AddIfNotContains(IEnumerable<T>)", () =>
{
    ICollection<int> list = new List<int> { 1 };
    Assert(list.AddIfNotContains(new[] { 1, 2, 2, 3 }) == 2, "bulk AddIfNotContains returned the wrong count");
    Assert(list.SequenceEqual(new[] { 1, 2, 3 }), "bulk AddIfNotContains left the wrong items");
});

Check("IsEmpty / IsNotEmpty", () =>
{
    ICollection<int> empty = Array.Empty<int>();
    ICollection<int> full = new Collection<int> { 1 };
    Assert(empty.IsEmpty() && !empty.IsNotEmpty(), "empty collection misreported");
    Assert(!full.IsEmpty() && full.IsNotEmpty(), "non-empty collection misreported");
});

Check("Argument validation", () =>
{
    ICollection<int>? missing = null;
    var threw = false;
    try
    {
        missing!.AddRange(new[] { 1 });
    }
    catch (ArgumentNullException ex) when (string.Equals(ex.ParamName, "source", StringComparison.Ordinal))
    {
        threw = true;
    }

    Assert(threw, "AddRange(null source) did not throw ArgumentNullException(source)");
});

if (failures.Count > 0)
{
    Console.Error.WriteLine($"AOT smoke FAILED ({failures.Count} check(s)):");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"  - {failure}");
    }

    return 1;
}

Console.WriteLine("AOT smoke OK: every public method verified under PublishAot + PublishTrimmed.");
return 0;

void Check(string name, Action check)
{
    try
    {
        check();
        Console.WriteLine($"  [ok] {name}");
    }
#pragma warning disable CA1031 // A smoke harness reports every failure instead of stopping at the first.
    catch (Exception ex)
#pragma warning restore CA1031
    {
        failures.Add($"{name}: {ex.Message}");
        Console.Error.WriteLine($"  [FAIL] {name}: {ex.Message}");
    }
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
