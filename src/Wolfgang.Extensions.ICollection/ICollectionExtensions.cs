using System;
using System.Collections.Generic;

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
    /// a clear, self-documenting emptiness check. The exact performance depends on the concrete
    /// collection type, but is typically O(1) for common implementations.
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
    /// a clear, self-documenting emptiness check. The exact performance depends on the concrete
    /// collection type, but is typically O(1) for common implementations.
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
    /// <paramref name="source"/> are silently skipped.
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

        foreach (var item in items)
        {
            if (predicate(item))
            {
                source.Add(item);
            }
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
        // avoids the temp-list allocation + second pass below. Skipped for
        // List<T> because List<T>.RemoveAll on netstandard2.0/net462 was a
        // late addition and the in-place enumerator pattern below is just
        // as cheap for the small-to-medium sizes typical of this extension.
        if (source is HashSet<T> set)
        {
            return set.RemoveWhere(item => predicate(item));
        }

        var toRemove = new List<T>();
        foreach (var item in source)
        {
            if (predicate(item))
            {
                toRemove.Add(item);
            }
        }

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
    /// first.
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
    /// uses <see cref="EqualityComparer{T}.Default"/>. <see cref="HashSet{T}"/>'s
    /// own <see cref="HashSet{T}.Add"/> already returns a Boolean to the
    /// same effect; this extension generalises the behaviour to every
    /// <see cref="ICollection{T}"/>.
    /// </remarks>
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
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

        var added = 0;
        foreach (var item in items)
        {
            if (source.AddIfNotContains(item))
            {
                added++;
            }
        }
        return added;
    }
}
