// Wolfgang.Extensions.ICollection — .NET 10.0 example
//
// Same scenarios as the .NET 8 example but compiled against the
// current LTS branch. Useful for spot-checking that nothing in the
// newer BCL has broken the extensions' semantics. Run with:
//
//   dotnet run --project examples/Wolfgang.Extensions.ICollection.DotNet10.Example1
//

using System.Collections.ObjectModel;
using Wolfgang.Extensions.ICollection;

Console.WriteLine("=== Wolfgang.Extensions.ICollection — .NET 10 example ===");
Console.WriteLine();


// ----- AddRange across different ICollection<T> implementations -----
Console.WriteLine("AddRange:");

ICollection<int> list = new List<int> { 1, 2, 3 };
list.AddRange([4, 5, 6]);                       // C# 12 collection expression
Console.WriteLine($"  List<int>          → [{string.Join(", ", list)}]");

ICollection<int> set = new HashSet<int> { 1, 2, 3 };
set.AddRange([3, 4, 5]);
Console.WriteLine($"  HashSet<int>       → [{string.Join(", ", set)}] (dedup'd)");

ICollection<int> linked = new LinkedList<int>();
linked.AddRange(Enumerable.Range(1, 5));
Console.WriteLine($"  LinkedList<int>    → [{string.Join(", ", linked)}]");

ICollection<int> collection = new Collection<int>();
collection.AddRange([10, 20, 30]);
Console.WriteLine($"  Collection<int>    → [{string.Join(", ", collection)}]");
Console.WriteLine();


// ----- Self-aliasing is safe -----
Console.WriteLine("Self-aliasing:");

ICollection<int> doubled = new List<int> { 1, 2, 3 };
doubled.AddRange(doubled);
Console.WriteLine($"  list.AddRange(list)        → [{string.Join(", ", doubled)}]");

ICollection<int> noop = new List<int> { 1, 2, 3 };
noop.ReplaceAll(noop);
Console.WriteLine($"  list.ReplaceAll(list)      → [{string.Join(", ", noop)}] (no-op)");

ICollection<int> emptied = new List<int> { 1, 2, 3 };
emptied.RemoveRange(emptied);
Console.WriteLine($"  list.RemoveRange(list)     → [{string.Join(", ", emptied)}] (empty)");
Console.WriteLine();


// ----- Predicate-gated bulk add / remove -----
Console.WriteLine("Predicate-gated bulk ops:");

ICollection<int> evens = new List<int>();
evens.AddRangeIf(Enumerable.Range(1, 20), n => n % 2 == 0);
Console.WriteLine($"  AddRangeIf (% 2 == 0)      → [{string.Join(", ", evens)}]");

int removed = evens.RemoveWhere(n => n > 10);
Console.WriteLine($"  RemoveWhere (n > 10)       → {removed} removed; now [{string.Join(", ", evens)}]");
Console.WriteLine();


// ----- ReplaceAll on an ObservableCollection -----
Console.WriteLine("ReplaceAll on ObservableCollection:");

var observable = new ObservableCollection<string>();
observable.CollectionChanged += (_, e) =>
    Console.WriteLine($"  notification: {e.Action} (new count = {observable.Count})");

ICollection<string> bindable = observable;
bindable.AddRange(["Alice", "Bob"]);
bindable.ReplaceAll(["Carol", "Dave", "Erin"]);
Console.WriteLine($"  final              → [{string.Join(", ", observable)}]");
Console.WriteLine();


// ----- AddIfNotContains: ISet fast path + list slow path -----
Console.WriteLine("AddIfNotContains:");

ICollection<int> seen = new List<int> { 1, 2, 3 };
Console.WriteLine($"  AddIfNotContains(3)        → {seen.AddIfNotContains(3)} (already present)");
Console.WriteLine($"  AddIfNotContains(4)        → {seen.AddIfNotContains(4)} (added)");

ICollection<int> tags = new HashSet<int> { 1, 2 };
int newTags = tags.AddIfNotContains([2, 3, 4]);
Console.WriteLine($"  HashSet bulk add 2,3,4     → {newTags} new; final [{string.Join(", ", tags)}]");
Console.WriteLine();


// ----- Presence checks -----
Console.WriteLine("Presence checks:");
ICollection<int> empty = new List<int>();
Console.WriteLine($"  empty.IsEmpty()            → {empty.IsEmpty()}");
Console.WriteLine($"  empty.IsNotEmpty()         → {empty.IsNotEmpty()}");

int[] buffer = [7, 8, 9];
Console.WriteLine($"  buffer.IsEmpty()           → {buffer.IsEmpty()} (int[] implements ICollection<int>)");
Console.WriteLine();


// ----- Read-only / fixed-size collections throw NotSupportedException -----
Console.WriteLine("Read-only / fixed-size:");
ICollection<int> readOnly = new ReadOnlyCollection<int>([1, 2, 3]);
try
{
    readOnly.AddRange([4, 5]);
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"  ReadOnlyCollection.AddRange → {ex.GetType().Name}: {ex.Message}");
}

ICollection<int> array = new int[3];
try
{
    array.AddRange([1]);
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"  int[].AddRange              → {ex.GetType().Name}: {ex.Message}");
}
