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
    public void AddRange_works_with_HashSet_and_respects_uniqueness()
    {
        // Arrange
        ICollection<int> source = new HashSet<int> { 1, 2, 3 };
        var items = new[] { 3, 4, 5, 5 }; // Contains duplicates

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(5, source.Count); // Should have 1, 2, 3, 4, 5 (duplicates ignored)
        Assert.Contains(4, source);
        Assert.Contains(5, source);
    }

    [Fact]
    public void AddRange_works_with_LinkedList()
    {
        // Arrange
        ICollection<string> source = new LinkedList<string>();
        var items = new[] { "first", "second", "third" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(3, source.Count);
        Assert.Contains("first", source);
        Assert.Contains("second", source);
        Assert.Contains("third", source);
    }

    [Fact]
    public void AddRange_works_with_large_collection()
    {
        // Arrange
        ICollection<int> source = new List<int>();
        var items = Enumerable.Range(1, 10000);

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(10000, source.Count);
        Assert.Contains(1, source);
        Assert.Contains(5000, source);
        Assert.Contains(10000, source);
    }

    [Fact]
    public void AddRange_preserves_order_in_List()
    {
        // Arrange
        ICollection<int> source = new List<int> { 1, 2, 3 };
        var items = new[] { 4, 5, 6 };

        // Act
        source.AddRange(items);

        // Assert
        var list = source as List<int>;
        Assert.NotNull(list);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, list);
    }

    [Fact]
    public void AddRange_adds_duplicate_items_to_List()
    {
        // Arrange
        ICollection<string> source = new List<string> { "item1" };
        var items = new[] { "item1", "item1", "item2" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(4, source.Count); // List allows duplicates
        var list = source as List<string>;
        Assert.NotNull(list);
        Assert.Equal(3, list.Count(x => x == "item1"));
    }

    [Fact]
    public void AddRange_works_with_LINQ_query_results()
    {
        // Arrange
        ICollection<int> source = new List<int>();
        var items = Enumerable.Range(1, 10).Where(n => n % 2 == 0); // Even numbers

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(5, source.Count);
        Assert.Contains(2, source);
        Assert.Contains(4, source);
        Assert.Contains(6, source);
        Assert.Contains(8, source);
        Assert.Contains(10, source);
    }

    [Fact]
    public void AddRange_works_with_custom_objects()
    {
        // Arrange
        ICollection<TestObject> source = new List<TestObject>();
        var items = new[]
        {
            new TestObject { Id = 1, Name = "First" },
            new TestObject { Id = 2, Name = "Second" }
        };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(2, source.Count);
        Assert.Contains(source, obj => obj.Id == 1 && obj.Name == "First");
        Assert.Contains(source, obj => obj.Id == 2 && obj.Name == "Second");
    }

    [Fact]
    public void AddRange_works_with_heterogeneous_object_collection()
    {
        // Arrange - using base type collection
        ICollection<object> source = new List<object> { "string", 123 };
        var items = new object[] { 45.6, DateTime.Now, true };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(5, source.Count);
    }

    [Fact]
    public void AddRange_adds_null_items_if_type_allows()
    {
        // Arrange
        ICollection<string?> source = new List<string?> { "item1" };
        var items = new string?[] { "item2", null, "item3" };

        // Act
        source.AddRange(items);

        // Assert
        Assert.Equal(4, source.Count);
        Assert.Contains(null, source);
    }

    // Helper class for testing with custom objects
    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
