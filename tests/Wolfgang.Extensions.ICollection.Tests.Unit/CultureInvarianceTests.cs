using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Wolfgang.Extensions.ICollection.Tests.Unit;

/// <summary>
/// Runs the string-sensitive paths of every method under hostile cultures and asserts the
/// results do not change.
/// </summary>
/// <remarks>
/// Culture contract: no public method in <c>ICollectionExtensions</c> is culture-sensitive. The
/// library formats and parses nothing; equality is always the target collection's own
/// (<c>Contains</c>/<c>Remove</c>/<c>ISet.Add</c>, normally
/// <see cref="EqualityComparer{T}.Default"/>, which is ordinal for strings). The only way culture
/// can enter is a comparer the caller chose, such as
/// <see cref="StringComparer.CurrentCultureIgnoreCase"/> on a <see cref="HashSet{T}"/>, and the
/// library then honours that comparer unchanged
/// (<see cref="AddIfNotContains_when_set_uses_a_culture_comparer_follows_that_comparer"/>).
/// The cultures cover Turkish dotted/dotless I, German decimal comma, Chinese collation,
/// Arabic right-to-left digit shapes and Japanese full-width forms. Both
/// <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>
/// are swapped and restored after each test.
/// </remarks>
public class CultureInvarianceTests
{
    public static TheoryData<string> Cultures =>
        new()
        {
            "en-US",
            "tr-TR",
            "de-DE",
            "zh-CN",
            "ar-SA",
            "ja-JP",
        };



    [Theory]
    [MemberData(nameof(Cultures))]
    public void AddIfNotContains_when_culture_changes_compares_strings_ordinally(string culture)
    {
        InCulture
        (
            culture,
            () =>
            {
                ICollection<string> source = new List<string> { "i", "ı", "1" };

                Assert.True(source.AddIfNotContains("I"));
                Assert.True(source.AddIfNotContains("İ"));
                Assert.False(source.AddIfNotContains("i"));
                Assert.Equal(2, source.AddIfNotContains(new[] { "１", "1", "١" }));
                Assert.Equal(new[] { "i", "ı", "1", "I", "İ", "１", "١" }, source);
            }
        );
    }



    [Theory]
    [MemberData(nameof(Cultures))]
    public void RemoveRange_and_RemoveWhere_when_culture_changes_match_strings_ordinally(string culture)
    {
        InCulture
        (
            culture,
            () =>
            {
                ICollection<string> source = new List<string> { "i", "I", "ı", "İ", "1,5", "1.5" };

                source.RemoveRange(new[] { "I", "1.5" });
                var removed = source.RemoveWhere(s => string.Equals(s, "ı", StringComparison.Ordinal));

                Assert.Equal(1, removed);
                Assert.Equal(new[] { "i", "İ", "1,5" }, source);
            }
        );
    }



    [Theory]
    [MemberData(nameof(Cultures))]
    public void AddRange_ReplaceAll_and_AddRangeIf_when_culture_changes_preserve_values_and_order(string culture)
    {
        InCulture
        (
            culture,
            () =>
            {
                ICollection<double> numbers = new List<double>();
                numbers.AddRange(new[] { 1.5, -0.25 });
                numbers.AddRangeIf(new[] { 2.75, 3.5 }, n => n > 3.0);
                Assert.Equal(new[] { 1.5, -0.25, 3.5 }, numbers);

                ICollection<string> words = new List<string> { "old" };
                words.ReplaceAll(new[] { "İstanbul", "Straße", "北京" });
                Assert.Equal(new[] { "İstanbul", "Straße", "北京" }, words);
                Assert.True(words.IsNotEmpty());
                Assert.False(words.IsEmpty());
            }
        );
    }



    [Fact]
    public void AddIfNotContains_when_set_uses_a_culture_comparer_follows_that_comparer()
    {
        var turkish = false;
        var english = false;

        InCulture("tr-TR", () => turkish = NewCultureSet().AddIfNotContains("I"));
        InCulture("en-US", () => english = NewCultureSet().AddIfNotContains("I"));

        // tr-TR: "I" lower-cases to dotless "ı", so it is NOT a case-insensitive match for "i".
        Assert.True(turkish);
        Assert.False(english);
    }



    private static ICollection<string> NewCultureSet()
    {
        return new HashSet<string>(StringComparer.CurrentCultureIgnoreCase) { "i" };
    }



    private static void InCulture(string name, Action action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var culture = new CultureInfo(name);
        try
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
