using System;
using System.Collections.Generic;
using System.Linq;

namespace Wolfgang.Extensions.ICollection;


/// <summary>
/// A collection of extension methods to <see cref="ICollection{T}"/>.
/// </summary>
public static class ICollectionExtensions
{

    /// <summary>
    /// Add all the specified items to the source collection.
    /// </summary>
    /// <param name="source">
    /// The source collection to add the items to.
    /// </param>
    /// <param name="items">
    /// The items to add to the source collection.
    /// </param>
    /// <typeparam name="T">
    /// The type of items in the collection.
    /// </typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown if source or items is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if the collection is read-only.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This extension method provides a convenient way to add multiple items to any collection
    /// that implements <see cref="ICollection{T}"/>, similar to the AddRange method available
    /// on <see cref="List{T}"/>.
    /// </para>
    /// <para>
    /// The method iterates through each item in the <paramref name="items"/> enumerable and
    /// adds them one by one to the <paramref name="source"/> collection using the Add method.
    /// </para>
    /// <para>
    /// <strong>Edge Cases and Behavior:</strong>
    /// <list type="bullet">
    /// <item><description>If <paramref name="items"/> is an empty enumerable, no items are added and the source collection remains unchanged.</description></item>
    /// <item><description>If the source collection has constraints (e.g., unique items in HashSet), the Add method's behavior is preserved.</description></item>
    /// <item><description>If the source collection is read-only, the Add method will throw a <see cref="NotSupportedException"/>.</description></item>
    /// <item><description>The method does not check for duplicates; duplicate handling depends on the underlying collection implementation.</description></item>
    /// <item><description>When <paramref name="source"/> is a <see cref="List{T}"/> and <paramref name="items"/> implements <see cref="ICollection{T}"/>, the list's capacity is pre-allocated to avoid repeated resizing.</description></item>
    /// <item><description>Self-aliasing (passing the same instance for both <paramref name="source"/> and <paramref name="items"/>) is safe: <paramref name="items"/> is snapshotted before mutation, so the call effectively appends a copy of the current contents to the collection.</description></item>
    /// <item><description>This method is not thread-safe. If multiple threads access the collection concurrently, external synchronization is required.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Add multiple strings to a list
    /// ICollection&lt;string&gt; names = new List&lt;string&gt; { "Alice" };
    /// names.AddRange(new[] { "Bob", "Charlie" });
    /// // names now contains: "Alice", "Bob", "Charlie"
    /// 
    /// // Works with any ICollection&lt;T&gt; implementation
    /// ICollection&lt;int&gt; numbers = new HashSet&lt;int&gt; { 1, 2 };
    /// numbers.AddRange(new[] { 3, 4, 5 });
    /// // numbers now contains: 1, 2, 3, 4, 5
    /// 
    /// // Add items from LINQ query results (requires using System.Linq)
    /// var evenNumbers = Enumerable.Range(1, 10).Where(n => n % 2 == 0);
    /// ICollection&lt;int&gt; myCollection = new List&lt;int&gt;();
    /// myCollection.AddRange(evenNumbers);
    /// // myCollection now contains: 2, 4, 6, 8, 10
    /// </code>
    /// </example>
    public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        // Self-aliasing guard: 'list.AddRange(list)' would mutate the
        // collection during the foreach below, which most BCL enumerators
        // reject with InvalidOperationException. Snapshot first so the
        // loop sees a stable view; the effective behaviour is "append a
        // copy of the current contents to the collection".
        if (ReferenceEquals(items, source))
        {
            items = new List<T>(source);
        }

        // Pre-allocate capacity on the target when it's a List<T> (the only
        // ICollection<T> with a settable Capacity) and items exposes Count
        // up-front via ICollection<T>. Both conditions must hold; without
        // them the loop below just relies on the target's own growth policy.
        if (items is ICollection<T> itemsCollection && source is List<T> list)
        {
            list.Capacity = Math.Max(list.Capacity, list.Count + itemsCollection.Count);
        }

        foreach (var item in items)
        {
            source.Add(item);
        }
    }



    /// <summary>
    /// Determines whether the collection contains no elements.
    /// </summary>
    /// <param name="source">
    /// The collection to check.
    /// </param>
    /// <typeparam name="T">
    /// The type of items in the collection.
    /// </typeparam>
    /// <returns>
    /// <c>true</c> if the collection contains no elements; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This extension method provides a cleaner, more readable alternative to checking
    /// <c>source.Count == 0</c>. It works with any <see cref="ICollection{T}"/> implementation.
    /// </para>
    /// <para>
    /// This method checks the <see cref="ICollection{T}.Count"/> property directly, providing
    /// a clear, self-documenting emptiness check. Typically <c>O(1)</c> for standard
    /// <see cref="ICollection{T}"/> implementations; the <see cref="ICollection{T}.Count"/>
    /// contract does not formally guarantee constant time, so a custom implementation could
    /// be slower.
    /// </para>
    /// <para>
    /// <strong>Edge Cases and Behavior:</strong>
    /// <list type="bullet">
    /// <item><description>Returns <c>true</c> for a newly created, empty collection.</description></item>
    /// <item><description>Returns <c>false</c> if the collection has one or more elements.</description></item>
    /// <item><description>This method is not thread-safe. If multiple threads modify the collection concurrently, external synchronization is required.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Check if a list is empty
    /// ICollection&lt;string&gt; names = new List&lt;string&gt;();
    /// bool empty = names.IsEmpty(); // true
    /// names.Add("Alice");
    /// empty = names.IsEmpty(); // false
    ///
    /// // Use in conditional logic
    /// ICollection&lt;int&gt; results = GetResults();
    /// if (results.IsEmpty())
    /// {
    ///     Console.WriteLine("No results found.");
    /// }
    ///
    /// // Works with any ICollection&lt;T&gt; implementation
    /// ICollection&lt;string&gt; set = new HashSet&lt;string&gt;(StringComparer.Ordinal);
    /// bool isEmpty = set.IsEmpty(); // true
    /// </code>
    /// </example>
    public static bool IsEmpty<T>(this ICollection<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Count == 0;
    }



    /// <summary>
    /// Determines whether the collection contains one or more elements.
    /// </summary>
    /// <param name="source">
    /// The collection to check.
    /// </param>
    /// <typeparam name="T">
    /// The type of items in the collection.
    /// </typeparam>
    /// <returns>
    /// <c>true</c> if the collection contains one or more elements; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This extension method provides a cleaner, more readable alternative to checking
    /// <c>source.Count &gt; 0</c>. It works with any <see cref="ICollection{T}"/> implementation.
    /// </para>
    /// <para>
    /// This method checks the <see cref="ICollection{T}.Count"/> property directly, providing
    /// a clear, self-documenting non-emptiness check. Typically <c>O(1)</c> for standard
    /// <see cref="ICollection{T}"/> implementations; the <see cref="ICollection{T}.Count"/>
    /// contract does not formally guarantee constant time, so a custom implementation could
    /// be slower.
    /// </para>
    /// <para>
    /// <strong>Edge Cases and Behavior:</strong>
    /// <list type="bullet">
    /// <item><description>Returns <c>false</c> for a newly created, empty collection.</description></item>
    /// <item><description>Returns <c>true</c> if the collection has one or more elements.</description></item>
    /// <item><description>This method is not thread-safe. If multiple threads modify the collection concurrently, external synchronization is required.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Check if a list has items
    /// ICollection&lt;string&gt; names = new List&lt;string&gt; { "Alice" };
    /// bool hasItems = names.IsNotEmpty(); // true
    ///
    /// // Use in conditional logic
    /// ICollection&lt;int&gt; results = GetResults();
    /// if (results.IsNotEmpty())
    /// {
    ///     ProcessResults(results);
    /// }
    ///
    /// // Works with any ICollection&lt;T&gt; implementation
    /// ICollection&lt;int&gt; queue = new LinkedList&lt;int&gt;();
    /// bool notEmpty = queue.IsNotEmpty(); // false
    /// queue.Add(42);
    /// notEmpty = queue.IsNotEmpty(); // true
    /// </code>
    /// </example>
    public static bool IsNotEmpty<T>(this ICollection<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Count > 0;
    }



    /// <summary>
    /// Removes one occurrence of each item in <paramref name="items"/>
    /// from <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The collection to remove items from.</param>
    /// <param name="items">The items to remove.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="items"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if <paramref name="source"/> is read-only.
    /// </exception>
    /// <remarks>
    /// Each item in <paramref name="items"/> is removed using
    /// <see cref="ICollection{T}.Remove"/>; if the target collection allows
    /// duplicates and the same value appears multiple times in
    /// <paramref name="items"/>, multiple occurrences are removed (one per
    /// call). Items in <paramref name="items"/> that are not present in
    /// <paramref name="source"/> are silently skipped. Self-aliasing
    /// (passing the same instance for both <paramref name="source"/> and
    /// <paramref name="items"/>) is safe: <paramref name="items"/> is
    /// snapshotted before mutation, so the call empties the collection
    /// cleanly without tripping the mutate-during-enumerate contract.
    /// </remarks>
    public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        // Self-aliasing guard: callers can legitimately invoke
        // 'list.RemoveRange(list)' to mean "drop everything". Iterating
        // 'source' while mutating it would throw InvalidOperationException
        // on most BCL enumerators; snapshot first so the loop sees a
        // stable view.
        if (ReferenceEquals(items, source))
        {
            items = new List<T>(source);
        }

        foreach (var item in items)
        {
            source.Remove(item);
        }
    }



    /// <summary>
    /// Adds every item from <paramref name="items"/> to
    /// <paramref name="source"/> for which <paramref name="predicate"/>
    /// returns <c>true</c>.
    /// </summary>
    /// <param name="source">The collection to add to.</param>
    /// <param name="items">The candidate items.</param>
    /// <param name="predicate">A function returning <c>true</c> for items
    /// that should be added.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/>, <paramref name="items"/>, or
    /// <paramref name="predicate"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if <paramref name="source"/> is read-only.
    /// </exception>
    /// <remarks>
    /// Self-aliasing (passing the same instance for both
    /// <paramref name="source"/> and <paramref name="items"/>) is safe:
    /// <paramref name="items"/> is snapshotted before mutation, so the
    /// predicate sees a stable view and the matching items are appended
    /// to the collection without tripping the mutate-during-enumerate
    /// contract.
    /// </remarks>
    public static void AddRangeIf<T>(
        this ICollection<T> source,
        IEnumerable<T> items,
        Func<T, bool> predicate)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        // Self-aliasing guard: snapshot when items === source so the
        // Where + foreach below doesn't trip the mutate-during-enumerate
        // contract on most BCL enumerators.
        if (ReferenceEquals(items, source))
        {
            items = new List<T>(source);
        }

        foreach (var item in items.Where(predicate))
        {
            source.Add(item);
        }
    }



    /// <summary>
    /// Removes every item from <paramref name="source"/> for which
    /// <paramref name="predicate"/> returns <c>true</c>.
    /// </summary>
    /// <param name="source">The collection to remove from.</param>
    /// <param name="predicate">A function returning <c>true</c> for items
    /// that should be removed.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <returns>The number of items removed.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="predicate"/>
    /// is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if <paramref name="source"/> is read-only.
    /// </exception>
    /// <remarks>
    /// Matching items are materialised into a temporary list before
    /// removal so the underlying collection can be mutated safely without
    /// invalidating the enumerator. When <paramref name="source"/> is a
    /// <see cref="HashSet{T}"/> the call delegates to the native
    /// <see cref="HashSet{T}.RemoveWhere(System.Predicate{T})"/>, which
    /// skips the temporary-list allocation.
    /// </remarks>
    public static int RemoveWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        // Fast path: HashSet<T> already exposes a native RemoveWhere that
        // avoids the temp-list allocation + second pass below. We don't
        // special-case List<T>.RemoveAll (which has shipped since .NET
        // Framework 2.0) because the generic two-pass pattern handles
        // every other ICollection<T> uniformly and the extra type-check
        // cost outweighs the savings at the small-to-medium sizes typical
        // for this extension.
        if (source is HashSet<T> set)
        {
            // Use Invoke method-group so the conversion to Predicate<T>
            // doesn't allocate a closure over `predicate`.
            return set.RemoveWhere(predicate.Invoke);
        }

        // Materialise matches before mutating so the underlying collection
        // can be modified safely without invalidating the enumerator.
        // Using Where + ToList here keeps Sonar S3267 happy and reads
        // closer to the intent than the manual filter loop.
        var toRemove = source.Where(predicate).ToList();
        foreach (var item in toRemove)
        {
            source.Remove(item);
        }

        return toRemove.Count;
    }



    /// <summary>
    /// Clears <paramref name="source"/> and then adds every item from
    /// <paramref name="items"/>.
    /// </summary>
    /// <param name="source">The collection to replace the contents of.</param>
    /// <param name="items">The new contents.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="items"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if <paramref name="source"/> is read-only.
    /// </exception>
    /// <remarks>
    /// The operation is not atomic. If enumeration of
    /// <paramref name="items"/> throws midway through, the collection is
    /// left empty (or with whatever items were already appended). Callers
    /// that need atomic replacement should materialise the new contents
    /// first. Self-aliasing (passing the same instance for both
    /// <paramref name="source"/> and <paramref name="items"/>) is safe:
    /// <paramref name="items"/> is snapshotted before the <c>Clear</c>, so
    /// the call is effectively a no-op rather than silently wiping the
    /// collection.
    /// </remarks>
    public static void ReplaceAll<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        // Self-aliasing guard: 'list.ReplaceAll(list)' should be a no-op
        // (or — for collections where Clear changes identity — at least
        // not silently wipe the data). Snapshotting before the Clear
        // means the final state is the original contents copied back in.
        if (ReferenceEquals(items, source))
        {
            items = new List<T>(source);
        }

        source.Clear();
        source.AddRange(items);
    }



    /// <summary>
    /// Adds <paramref name="item"/> to <paramref name="source"/> if it is
    /// not already present (per
    /// <see cref="ICollection{T}.Contains"/>).
    /// </summary>
    /// <param name="source">The collection to add to.</param>
    /// <param name="item">The candidate item.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <returns><c>true</c> if the item was added; <c>false</c> if it was
    /// already present.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if <paramref name="source"/> is read-only.
    /// </exception>
    /// <remarks>
    /// Containment is determined by the target collection's own
    /// <see cref="ICollection{T}.Contains"/> implementation, which usually
    /// uses <see cref="EqualityComparer{T}.Default"/>. When
    /// <paramref name="source"/> is an <see cref="ISet{T}"/> the call is
    /// delegated to <see cref="ISet{T}.Add"/>, which returns the same
    /// Boolean signal in a single lookup (and preserves the set's native
    /// equality semantics, e.g. a custom <see cref="IEqualityComparer{T}"/>).
    /// For other <see cref="ICollection{T}"/> implementations the
    /// extension generalises the behaviour using
    /// <see cref="ICollection{T}.Contains"/> + <see cref="ICollection{T}.Add"/>.
    /// </remarks>
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        // Fast path: ISet<T> (HashSet<T>, SortedSet<T>, etc.) — ISet.Add
        // already returns the "did we add?" Boolean in a single lookup,
        // so we skip the redundant Contains call.
        if (source is ISet<T> set)
        {
            return set.Add(item);
        }

        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }



    /// <summary>
    /// Adds each item from <paramref name="items"/> to
    /// <paramref name="source"/> if it is not already present.
    /// </summary>
    /// <param name="source">The collection to add to.</param>
    /// <param name="items">The candidate items.</param>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <returns>The number of items actually added (items already present
    /// in <paramref name="source"/>, or repeated within
    /// <paramref name="items"/> after the first addition, are skipped).</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="items"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown if <paramref name="source"/> is read-only.
    /// </exception>
    public static int AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        // Count() with a side-effecting predicate: AddIfNotContains returns
        // true exactly for items it actually added, so the count is the
        // number of newly added items. This replaces a manual foreach +
        // counter that S3267 (Sonar) flags as a Where-able pattern.
        return items.Count(source.AddIfNotContains);
    }
}
