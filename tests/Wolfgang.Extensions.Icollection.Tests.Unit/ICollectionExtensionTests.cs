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
    public void AddRange_adds_items_to_source_collection()
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
    public void AddRange_with_empty_items_does_not_modify_source_collection()
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
    public void AddRange_to_empty_source_adds_all_items()
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
    public void AddRange_throws_NotSupportedException_for_ReadOnlyCollection()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        ICollection<string> source = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { "existing" });

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => source.AddRange(items));
    }


    [Fact]
    public void AddRange_works_with_value_types()
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
    public void AddRange_with_nullable_reference_types_including_nulls()
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
    public void AddRange_to_HashSet_only_adds_unique_items()
    {
        // Arrange
        ICollection<string> source = new HashSet<string> { "item1" };
        var items = new List<string> { "item2", "item1", "item3" }; // "item1" is already in the HashSet; Add will return false for this duplicate and not add it again

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count); // Only 3 unique items (item1, item2, item3)
        Assert.Contains("item1", source);
        Assert.Contains("item2", source);
        Assert.Contains("item3", source);
    }


    [Fact]
    public void AddRange_with_snapshot_of_collection_contents()
    {
        // Arrange
        var source = new List<string> { "item1", "item2" };
        var snapshot = new List<string>(source); // Create a snapshot of current items

        // Act
        source.AddRange(snapshot);

        // Assert
        Assert.Equal(4, source.Count);
        Assert.Equal(2, source.Count(s => s == "item1"));
        Assert.Equal(2, source.Count(s => s == "item2"));
    }


    [Fact]
    public void AddRange_with_single_item()
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
    public void AddRange_works_with_Collection_type()
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
    public void AddRange_with_large_collection()
    {
        // Arrange
        var source = new List<int>();
        var items = Enumerable.Range(1, 10000);

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(10000, source.Count);
        Assert.Equal(1, source[0]); // First element
        Assert.Equal(5000, source[4999]); // Middle element
        Assert.Equal(10000, source[9999]); // Last element
    }


    [Fact]
    public void AddRange_works_with_LinkedList()
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
    public void AddRange_with_lazy_IEnumerable_from_LINQ()
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
}
