using System.Collections.ObjectModel;

namespace Wolfgang.Extensions.ICollection.Tests.Unit;

// ReSharper disable once InconsistentNaming
public class ICollectionExtensionTests
{
    [Fact]
    public void AddRange_when_source_is_null_throws_ArgumentNullException()
    {
        // Arrange
        ICollection<string> source = null!;
        var items = new List<string> { "item1", "item2" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>  source.AddRange(items));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_when_items_is_null_throws_ArgumentNullException()
    {
        // Arrange
        ICollection<string> source = new List<string>();
        IEnumerable<string> items = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>  source.AddRange(items));
        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void AddRange_when_source_has_items_adds_all_items()
    {
        // Arrange
        ICollection<string> source = new List<string> { "item1" };
        var items = new List<string> { "item2", "item3" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count);
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains("item3", source);
    }



    [Fact]
    public void AddRange_when_items_is_empty_does_not_modify_source_collection()
    {
        // Arrange
        ICollection<string> source = new List<string> { "item1" };

        // Act
        source.AddRange([]);

        // Assert
        Assert.Single(source);
        Assert.Contains("item1", source);
    }



    [Fact]
    public void AddRange_when_source_is_empty_adds_all_items()
    {
        // Arrange
        ICollection<string> source = new List<string>();
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count);
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains("item3", source);
    }



    [Fact]
    public void AddRange_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        ICollection<string> source = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { "existing" });

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => source.AddRange(items));
    }



    [Fact]
    public void AddRange_when_items_are_value_types_adds_all_items()
    {
        // Arrange
        ICollection<int> source = new List<int> { 1, 2, 3 };
        var items = new List<int> { 4, 5, 6 };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(6, source.Count);
        Assert.Contains(4, source);
        Assert.Contains(5, source);
        Assert.Contains(6, source);
    }



    [Fact]
    public void AddRange_when_items_contain_nulls_adds_all_including_nulls()
    {
        // Arrange
        ICollection<string?> source = new List<string?> { "item1" };
        var items = new List<string?> { "item2", null, "item3" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(4, source.Count);
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains(null, source);
        Assert.Contains("item3", source);
    }



    [Fact]
    public void AddRange_when_source_is_HashSet_only_adds_unique_items()
    {
        // Arrange
        ICollection<string> source = new HashSet<string>(StringComparer.Ordinal) { "item1" };
        var items = new List<string> { "item2", "item1", "item3" }; // "item1" is already in the HashSet

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count); // Only 3 unique items (item1, item2, item3)
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains("item3", source);
    }



    [Fact]
    public void AddRange_when_items_are_snapshot_of_source_adds_duplicates()
    {
        // Arrange
        ICollection<string> source = new List<string> { "item1", "item2" };
        var snapshot = new List<string>(source); // Create a snapshot of current items

        // Act
        source.AddRange(snapshot);

        // Assert
        Assert.Equal(4, source.Count);
        Assert.Equal(2, source.Count(s => s.Equals("item1", StringComparison.Ordinal)));
        Assert.Equal(2, source.Count(s => s.Equals("item2", StringComparison.Ordinal)));
    }



    [Fact]
    public void AddRange_when_items_has_single_item_adds_it()
    {
        // Arrange
        ICollection<string> source = new List<string> { "existing" };
        var items = new List<string> { "single" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(2, source.Count);
        Assert.Contains("existing", source);
        Assert.Contains("single", source);
    }



    [Fact]
    public void AddRange_when_source_is_Collection_type_adds_all_items()
    {
        // Arrange
        ICollection<string> source = new System.Collections.ObjectModel.Collection<string> { "item1" };
        var items = new List<string> { "item2", "item3" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count);
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains("item3", source);
    }



    [Fact]
    public void AddRange_when_items_is_large_collection_adds_all_items()
    {
        // Arrange
        ICollection<int> source = new List<int>();
        var items = Enumerable.Range(1, 10000);

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(10000, source.Count);
        Assert.Equal(1, source.First());
        Assert.Equal(10000, source.Last());
    }



    [Fact]
    public void AddRange_when_source_is_LinkedList_adds_all_items()
    {
        // Arrange
        ICollection<string> source = new LinkedList<string>();
        source.Add("item1");
        var items = new List<string> { "item2", "item3" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count);
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains("item3", source);
    }



    [Fact]
    public void AddRange_when_items_is_lazy_enumerable_adds_all_items()
    {
        // Arrange
        ICollection<int> source = new List<int> { 1, 2, 3 };
        var items = Enumerable.Range(4, 3).Where(x => x > 3); // Lazy evaluation

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(6, source.Count);
        Assert.Contains(4, source);
        Assert.Contains(5, source);
        Assert.Contains(6, source);
    }



    // =========================================================================
    // IsEmpty tests
    // =========================================================================

    [Fact]
    public void IsEmpty_when_source_is_null_throws_ArgumentNullException()
    {
        // Arrange
        ICollection<string> source = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => source.IsEmpty());
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void IsEmpty_when_source_is_empty_returns_true()
    {
        // Arrange
        ICollection<string> source = new List<string>();

        // Act
        var result = source.IsEmpty();

        // Assert
        Assert.True(result);
    }



    [Fact]
    public void IsEmpty_when_source_has_items_returns_false()
    {
        // Arrange
        ICollection<string> source = new List<string> { "item1" };

        // Act
        var result = source.IsEmpty();

        // Assert
        Assert.False(result);
    }



    public static TheoryData<ICollection<int>, bool> IsEmpty_collection_type_cases =>
        new()
        {
            { new List<int>(),                  true  },
            { new List<int> { 1, 2, 3 },        false },
            { new HashSet<int>(),               true  },
            { new HashSet<int> { 1, 2, 3 },     false },
            { new LinkedList<int>(),            true  },
            { LinkedListWith(1, 2, 3),          false },
            { new Collection<int>(),            true  },
            { new Collection<int> { 1, 2, 3 },  false },
        };

    [Theory]
    [MemberData(nameof(IsEmpty_collection_type_cases))]
    public void IsEmpty_when_source_is_any_ICollection_implementation_returns_correct_result(
        ICollection<int> source,
        bool expected)
    {
        // Act
        var result = source.IsEmpty();

        // Assert
        Assert.Equal(expected, result);
    }



    // =========================================================================
    // IsNotEmpty tests
    // =========================================================================

    [Fact]
    public void IsNotEmpty_when_source_is_null_throws_ArgumentNullException()
    {
        // Arrange
        ICollection<string> source = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => source.IsNotEmpty());
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void IsNotEmpty_when_source_is_empty_returns_false()
    {
        // Arrange
        ICollection<string> source = new List<string>();

        // Act
        var result = source.IsNotEmpty();

        // Assert
        Assert.False(result);
    }



    [Fact]
    public void IsNotEmpty_when_source_has_items_returns_true()
    {
        // Arrange
        ICollection<string> source = new List<string> { "item1" };

        // Act
        var result = source.IsNotEmpty();

        // Assert
        Assert.True(result);
    }



    public static TheoryData<ICollection<int>, bool> IsNotEmpty_collection_type_cases =>
        new()
        {
            { new List<int>(),                  false },
            { new List<int> { 1, 2, 3 },        true  },
            { new HashSet<int>(),               false },
            { new HashSet<int> { 1, 2, 3 },     true  },
            { new LinkedList<int>(),            false },
            { LinkedListWith(1, 2, 3),          true  },
            { new Collection<int>(),            false },
            { new Collection<int> { 1, 2, 3 },  true  },
        };

    [Theory]
    [MemberData(nameof(IsNotEmpty_collection_type_cases))]
    public void IsNotEmpty_when_source_is_any_ICollection_implementation_returns_correct_result(
        ICollection<int> source,
        bool expected)
    {
        // Act
        var result = source.IsNotEmpty();

        // Assert
        Assert.Equal(expected, result);
    }



    // =========================================================================
    // Implementation-detail tests: capacity pre-allocation path
    // =========================================================================

    [Fact]
    public void AddRange_when_source_is_List_and_items_is_ICollection_pre_allocates_capacity()
    {
        // Arrange — start with a list whose Capacity is less than the final
        // Count so we can prove the extension grew it in one step rather than
        // relying on List's amortised doubling.
        var source = new List<int>(capacity: 1) { 0 };
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Act
        source.AddRange(items);

        // Assert — Capacity must accommodate the final count (10) exactly,
        // proving the pre-allocation branch ran. If the optimisation regressed
        // and List had to grow via Add()'s amortised doubling, Capacity would
        // overshoot to 16 (or another power-of-two).
        Assert.Equal(10, source.Capacity);
        Assert.Equal(10, source.Count);
    }



    [Fact]
    public void AddRange_when_source_List_already_has_sufficient_capacity_does_not_shrink_it()
    {
        // Arrange — list with deliberately oversized capacity.
        var source = new List<int>(capacity: 100) { 0 };
        var items = new List<int> { 1, 2, 3 };

        // Act
        source.AddRange(items);

        // Assert — Capacity should NOT be reduced to fit; the
        // Math.Max(current, needed) guard preserves over-allocations.
        Assert.Equal(100, source.Capacity);
        Assert.Equal(4, source.Count);
    }



    // =========================================================================
    // Helpers
    // =========================================================================

    private static LinkedList<T> LinkedListWith<T>(params T[] items)
    {
        var list = new LinkedList<T>();
        foreach (var item in items)
        {
            list.AddLast(item);
        }
        return list;
    }
}
