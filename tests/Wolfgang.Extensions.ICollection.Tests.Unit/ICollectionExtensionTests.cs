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
    public void AddRange_when_source_is_Array_throws_NotSupportedException()
    {
        // Array implements ICollection<T> but is fixed-size: its
        // IsReadOnly returns true and ICollection<T>.Add throws
        // NotSupportedException by contract. AddRange surfaces that
        // throw on the first item it tries to append.
        ICollection<int> source = new int[5];
        Assert.True(source.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => source.AddRange(new[] { 1, 2, 3 }));
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



    [Fact]
    public void AddRange_when_items_is_same_instance_as_source_appends_snapshot()
    {
        // Self-aliasing guard: 'list.AddRange(list)' must snapshot items
        // before mutating source; otherwise the foreach below trips the
        // mutate-during-enumerate contract on List<T>'s enumerator.
        ICollection<int> source = new List<int> { 1, 2, 3 };
        source.AddRange(source);
        Assert.Equal(new[] { 1, 2, 3, 1, 2, 3 }, source);
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



    public static TheoryData<ICollection<int>, bool> IsEmptyCollectionTypeCases =>
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
    [MemberData(nameof(IsEmptyCollectionTypeCases))]
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



    public static TheoryData<ICollection<int>, bool> IsNotEmptyCollectionTypeCases =>
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
    [MemberData(nameof(IsNotEmptyCollectionTypeCases))]
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
        // relying on List's amortised doubling. The List<int> reference is
        // held so we can inspect Capacity after the call.
        var list = new List<int>(capacity: 1) { 0 };
        // Receiver must be typed as ICollection<int> so the extension method
        // wins overload resolution against List<T>.AddRange (instance methods
        // take precedence on the receiver's static type).
        ICollection<int> source = list;
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Act
        source.AddRange(items);

        // Assert — Capacity must accommodate the final count (10) exactly,
        // proving the pre-allocation branch ran. If the optimisation regressed
        // and List had to grow via Add()'s amortised doubling, Capacity would
        // overshoot to 16 (or another power-of-two).
        Assert.Equal(10, list.Capacity);
        Assert.Equal(10, list.Count);
    }



    [Fact]
    public void AddRange_when_source_List_already_has_sufficient_capacity_does_not_shrink_it()
    {
        // Arrange — list with deliberately oversized capacity. Typing the
        // receiver as ICollection<int> ensures the extension wins overload
        // resolution; otherwise List<T>.AddRange (instance method) would
        // take precedence and skip the Math.Max guard we're trying to test.
        var list = new List<int>(capacity: 100) { 0 };
        ICollection<int> source = list;
        var items = new List<int> { 1, 2, 3 };

        // Act
        source.AddRange(items);

        // Assert — Capacity should NOT be reduced to fit; the
        // Math.Max(current, needed) guard preserves over-allocations.
        Assert.Equal(100, list.Capacity);
        Assert.Equal(4, list.Count);
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



    // =========================================================================
    // RemoveRange tests
    // =========================================================================

    [Fact]
    public void RemoveRange_when_source_is_null_throws_ArgumentNullException()
    {
        ICollection<string> source = null!;
        var ex = Assert.Throws<ArgumentNullException>(() => source.RemoveRange(new[] { "x" }));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void RemoveRange_when_items_is_null_throws_ArgumentNullException()
    {
        ICollection<string> source = new List<string>();
        var ex = Assert.Throws<ArgumentNullException>(() => source.RemoveRange(null!));
        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void RemoveRange_removes_each_listed_item()
    {
        ICollection<int> source = new List<int> { 1, 2, 3, 4, 5 };
        source.RemoveRange(new[] { 2, 4 });
        Assert.Equal(new[] { 1, 3, 5 }, source);
    }



    [Fact]
    public void RemoveRange_silently_skips_items_not_present()
    {
        ICollection<int> source = new List<int> { 1, 2, 3 };
        source.RemoveRange(new[] { 7, 8, 9 });
        Assert.Equal(new[] { 1, 2, 3 }, source);
    }



    [Fact]
    public void RemoveRange_removes_one_occurrence_per_item_in_items()
    {
        ICollection<int> source = new List<int> { 1, 2, 2, 2, 3 };
        source.RemoveRange(new[] { 2, 2 });
        Assert.Equal(new[] { 1, 2, 3 }, source);
    }



    [Fact]
    public void RemoveRange_when_items_is_same_instance_as_source_removes_all_items()
    {
        // Self-aliasing guard: 'list.RemoveRange(list)' must not throw
        // InvalidOperationException from mutate-during-enumerate. Snapshot
        // ensures the loop sees a stable view; the final state is empty.
        ICollection<int> source = new List<int> { 1, 2, 3 };
        source.RemoveRange(source);
        Assert.Empty(source);
    }



    [Fact]
    public void RemoveRange_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        ICollection<int> source =
            new System.Collections.ObjectModel.ReadOnlyCollection<int>(new List<int> { 1, 2 });
        Assert.Throws<NotSupportedException>(() => source.RemoveRange(new[] { 1 }));
    }



    // =========================================================================
    // AddRangeIf tests
    // =========================================================================

    [Fact]
    public void AddRangeIf_when_source_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = null!;
        var ex = Assert.Throws<ArgumentNullException>(
            () => source.AddRangeIf(new[] { 1 }, _ => true));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRangeIf_when_items_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = new List<int>();
        var ex = Assert.Throws<ArgumentNullException>(
            () => source.AddRangeIf(null!, _ => true));
        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void AddRangeIf_when_predicate_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = new List<int>();
        var ex = Assert.Throws<ArgumentNullException>(
            () => source.AddRangeIf(new[] { 1 }, null!));
        Assert.Equal("predicate", ex.ParamName);
    }



    [Fact]
    public void AddRangeIf_adds_only_items_matching_predicate()
    {
        ICollection<int> source = new List<int>();
        source.AddRangeIf(new[] { 1, 2, 3, 4, 5, 6 }, n => n % 2 == 0);
        Assert.Equal(new[] { 2, 4, 6 }, source);
    }



    [Fact]
    public void AddRangeIf_with_always_false_predicate_adds_nothing()
    {
        ICollection<int> source = new List<int> { 0 };
        source.AddRangeIf(new[] { 1, 2, 3 }, _ => false);
        Assert.Equal(new[] { 0 }, source);
    }



    [Fact]
    public void AddRangeIf_when_items_is_same_instance_as_source_appends_filtered_snapshot()
    {
        // Self-aliasing guard: 'list.AddRangeIf(list, predicate)' must
        // snapshot items before enumerating; otherwise the Where +
        // foreach would trip the mutate-during-enumerate contract when
        // a matching item is appended.
        ICollection<int> source = new List<int> { 1, 2, 3, 4 };
        source.AddRangeIf(source, n => n % 2 == 0);
        Assert.Equal(new[] { 1, 2, 3, 4, 2, 4 }, source);
    }



    // =========================================================================
    // RemoveWhere tests
    // =========================================================================

    [Fact]
    public void RemoveWhere_when_source_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = null!;
        var ex = Assert.Throws<ArgumentNullException>(() => source.RemoveWhere(_ => true));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void RemoveWhere_when_predicate_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = new List<int>();
        var ex = Assert.Throws<ArgumentNullException>(() => source.RemoveWhere(null!));
        Assert.Equal("predicate", ex.ParamName);
    }



    [Fact]
    public void RemoveWhere_returns_count_of_removed_items()
    {
        ICollection<int> source = new List<int> { 1, 2, 3, 4, 5, 6 };
        var removed = source.RemoveWhere(n => n % 2 == 0);
        Assert.Equal(3, removed);
        Assert.Equal(new[] { 1, 3, 5 }, source);
    }



    [Fact]
    public void RemoveWhere_returns_zero_when_nothing_matches()
    {
        ICollection<int> source = new List<int> { 1, 3, 5 };
        var removed = source.RemoveWhere(n => n % 2 == 0);
        Assert.Equal(0, removed);
        Assert.Equal(new[] { 1, 3, 5 }, source);
    }



    [Fact]
    public void RemoveWhere_handles_removing_every_item()
    {
        ICollection<int> source = new List<int> { 1, 2, 3 };
        var removed = source.RemoveWhere(_ => true);
        Assert.Equal(3, removed);
        Assert.Empty(source);
    }



    // =========================================================================
    // ReplaceAll tests
    // =========================================================================

    [Fact]
    public void ReplaceAll_when_source_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = null!;
        var ex = Assert.Throws<ArgumentNullException>(() => source.ReplaceAll(new[] { 1 }));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void ReplaceAll_when_items_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = new List<int>();
        var ex = Assert.Throws<ArgumentNullException>(() => source.ReplaceAll(null!));
        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void ReplaceAll_clears_and_repopulates()
    {
        ICollection<string> source = new List<string> { "old1", "old2", "old3" };
        source.ReplaceAll(new[] { "new1", "new2" });
        Assert.Equal(new[] { "new1", "new2" }, source);
    }



    [Fact]
    public void ReplaceAll_with_empty_items_just_clears()
    {
        ICollection<int> source = new List<int> { 1, 2, 3 };
        source.ReplaceAll(Array.Empty<int>());
        Assert.Empty(source);
    }



    [Fact]
    public void ReplaceAll_when_items_is_same_instance_as_source_is_a_no_op()
    {
        // Self-aliasing guard: 'list.ReplaceAll(list)' must not silently
        // wipe the collection (the Clear would otherwise drain the source
        // before the AddRange enumeration started).
        ICollection<int> source = new List<int> { 1, 2, 3 };
        source.ReplaceAll(source);
        Assert.Equal(new[] { 1, 2, 3 }, source);
    }



    // =========================================================================
    // AddIfNotContains tests
    // =========================================================================

    [Fact]
    public void AddIfNotContains_single_when_source_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = null!;
        var ex = Assert.Throws<ArgumentNullException>(() => source.AddIfNotContains(1));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddIfNotContains_single_returns_true_when_item_was_added()
    {
        ICollection<int> source = new List<int> { 1, 2 };
        var added = source.AddIfNotContains(3);
        Assert.True(added);
        Assert.Equal(new[] { 1, 2, 3 }, source);
    }



    [Fact]
    public void AddIfNotContains_single_returns_false_when_item_already_present()
    {
        ICollection<int> source = new List<int> { 1, 2, 3 };
        var added = source.AddIfNotContains(2);
        Assert.False(added);
        Assert.Equal(new[] { 1, 2, 3 }, source);
    }



    [Fact]
    public void AddIfNotContains_single_uses_ISet_Add_when_source_is_HashSet()
    {
        // HashSet<T> is ISet<T> — the extension's ISet fast path should
        // delegate to set.Add (one lookup) instead of Contains+Add (two).
        // We can't directly observe the call count, but behaviour parity
        // is the contract: returns the same Boolean as a direct set.Add
        // would have.
        var set = new HashSet<int> { 1, 2 };
        ICollection<int> source = set;
        Assert.True(source.AddIfNotContains(3));
        Assert.False(source.AddIfNotContains(2));
        // Order-independent: HashSet<T> enumeration order is not guaranteed,
        // so compare set contents via SetEquals rather than sequence equality.
        Assert.True(set.SetEquals(new[] { 1, 2, 3 }));
    }



    [Fact]
    public void AddIfNotContains_many_when_source_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = null!;
        var ex = Assert.Throws<ArgumentNullException>(
            () => source.AddIfNotContains(new[] { 1, 2 }));
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddIfNotContains_many_when_items_is_null_throws_ArgumentNullException()
    {
        ICollection<int> source = new List<int>();
        var ex = Assert.Throws<ArgumentNullException>(
            () => source.AddIfNotContains((IEnumerable<int>)null!));
        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void AddIfNotContains_many_returns_count_of_items_added()
    {
        ICollection<int> source = new List<int> { 1, 2 };
        var added = source.AddIfNotContains(new[] { 2, 3, 4 });
        Assert.Equal(2, added);
        Assert.Equal(new[] { 1, 2, 3, 4 }, source);
    }



    [Fact]
    public void AddIfNotContains_many_dedupes_within_items_after_first_addition()
    {
        ICollection<int> source = new List<int>();
        var added = source.AddIfNotContains(new[] { 1, 1, 2, 2, 3 });
        Assert.Equal(3, added);
        Assert.Equal(new[] { 1, 2, 3 }, source);
    }



    // =========================================================================
    // ReadOnly NotSupportedException coverage for the new methods
    // =========================================================================

    [Fact]
    public void AddRangeIf_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        ICollection<int> source =
            new System.Collections.ObjectModel.ReadOnlyCollection<int>(new List<int> { 0 });
        Assert.Throws<NotSupportedException>(
            () => source.AddRangeIf(new[] { 1, 2, 3 }, _ => true));
    }



    [Fact]
    public void RemoveWhere_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        ICollection<int> source =
            new System.Collections.ObjectModel.ReadOnlyCollection<int>(new List<int> { 1, 2 });
        Assert.Throws<NotSupportedException>(() => source.RemoveWhere(_ => true));
    }



    [Fact]
    public void ReplaceAll_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        ICollection<int> source =
            new System.Collections.ObjectModel.ReadOnlyCollection<int>(new List<int> { 1, 2 });
        Assert.Throws<NotSupportedException>(() => source.ReplaceAll(new[] { 9 }));
    }



    [Fact]
    public void AddIfNotContains_single_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        ICollection<int> source =
            new System.Collections.ObjectModel.ReadOnlyCollection<int>(new List<int> { 1, 2 });
        Assert.Throws<NotSupportedException>(() => source.AddIfNotContains(3));
    }



    [Fact]
    public void AddIfNotContains_many_when_source_is_ReadOnly_throws_NotSupportedException()
    {
        ICollection<int> source =
            new System.Collections.ObjectModel.ReadOnlyCollection<int>(new List<int> { 1, 2 });
        Assert.Throws<NotSupportedException>(
            () => source.AddIfNotContains(new[] { 3, 4 }));
    }
}
