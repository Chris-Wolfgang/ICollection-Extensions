using System;
using System.Collections.Generic;

namespace Wolfgang.Extensions.ICollection;

// ReSharper disable once InconsistentNaming
/// <summary>
/// A collection of extension methods to ICollection{T}
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
    public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        foreach (var item in items)
        {
            source.Add(item);
        }
    }
}
