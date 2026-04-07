using System;
using System.Collections.Generic;

namespace Wolfgang.Extensions.ICollection;

// ReSharper disable once InconsistentNaming
/// <summary>
/// A collection of extension methods to <see cref="ICollection{T}"/>
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
    /// This method uses the <see cref="ICollection{T}.Count"/> property directly, providing
    /// a clear, self-documenting O(1) check. While <c>Enumerable.Any()</c> also uses a Count
    /// fast-path for <see cref="ICollection{T}"/>, this method expresses intent more clearly
    /// when checking for emptiness.
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
    /// This method uses the <see cref="ICollection{T}.Count"/> property directly, providing
    /// a clear, self-documenting O(1) check. While <c>Enumerable.Any()</c> also uses a Count
    /// fast-path for <see cref="ICollection{T}"/>, this method expresses intent more clearly
    /// when checking for emptiness.
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
}
