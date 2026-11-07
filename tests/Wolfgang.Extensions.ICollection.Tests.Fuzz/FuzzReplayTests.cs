using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Extensions.ICollection.Tests.Fuzz;

/// <summary>
/// Guards the fuzz replay corpus (#165): a seed recorded under a misspelt property name would
/// otherwise never run.
/// </summary>
public sealed class FuzzReplayTests
{
    [Fact]
    public async Task AllAsync_when_corpus_is_loaded_names_only_existing_properties()
    {
        var properties = typeof(FuzzProperties)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.GetCustomAttribute<FactAttribute>() is not null)
            .Select(method => method.Name)
            .ToHashSet(StringComparer.Ordinal);

        var unknown = (await FuzzReplay.AllAsync())
            .Select(entry => entry.Property)
            .Where(property => !properties.Contains(property))
            .ToList();

        Assert.Empty(unknown);
    }



    [Fact]
    public async Task AllAsync_when_corpus_is_loaded_has_a_seed_for_every_property()
    {
        var seeded = (await FuzzReplay.AllAsync())
            .Select(entry => entry.Property)
            .ToHashSet(StringComparer.Ordinal);

        var unseeded = typeof(FuzzProperties)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.GetCustomAttribute<FactAttribute>() is not null)
            .Select(method => method.Name)
            .Where(name => !seeded.Contains(name))
            .ToList();

        Assert.Empty(unseeded);
    }



    [Fact]
    public void Parse_when_lines_have_comments_and_blanks_returns_only_the_pairs()
    {
        var entries = FuzzReplay.Parse
        (
            [
                "# comment",
                "",
                "   ",
                "  Some_property   0Fpln3se3Fq1  ",
            ]
        );

        Assert.Equal
        (
            [new ReplaySeed("Some_property", "0Fpln3se3Fq1")],
            entries
        );
    }



    [Fact]
    public void Parse_when_line_is_not_a_property_seed_pair_throws_FormatException()
    {
        var exception = Assert.Throws<FormatException>
        (
            () => FuzzReplay.Parse(["Some_property"])
        );

        Assert.Contains("Some_property", exception.Message, StringComparison.Ordinal);
    }
}
