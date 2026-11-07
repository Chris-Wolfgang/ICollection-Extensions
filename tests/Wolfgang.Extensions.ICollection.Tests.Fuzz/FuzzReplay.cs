using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wolfgang.Extensions.ICollection.Tests.Fuzz;

/// <summary>
/// Loads the committed fuzz replay corpus (<c>replay/seeds.txt</c>, #165): CsCheck seeds of
/// cases that every run of <see cref="FuzzProperties"/> replays deterministically before its
/// random cases.
/// </summary>
internal static class FuzzReplay
{
    /// <summary>
    /// Gets the path of the corpus file copied next to the test assembly.
    /// </summary>
    internal static string CorpusPath { get; } = Path.Combine(AppContext.BaseDirectory, "replay", "seeds.txt");



    /// <summary>
    /// Returns every seed in the corpus. The file is small and read only by the fuzz properties'
    /// setup, so it is read on each call rather than cached behind a shared task.
    /// </summary>
    internal static Task<IReadOnlyList<ReplaySeed>> AllAsync() => LoadAsync();



    /// <summary>
    /// Returns the seeds recorded for <paramref name="property"/>, in file order.
    /// </summary>
    internal static async Task<IReadOnlyList<string>> SeedsForAsync(string property) =>
        (await LoadAsync())
            .Where(entry => string.Equals(entry.Property, property, StringComparison.Ordinal))
            .Select(entry => entry.Seed)
            .ToList();



    /// <summary>
    /// Parses corpus lines: blank lines and <c>#</c> comments are skipped, every other line must
    /// be exactly <c>&lt;property&gt; &lt;seed&gt;</c>.
    /// </summary>
    /// <exception cref="FormatException">A line is not a property/seed pair.</exception>
    internal static IReadOnlyList<ReplaySeed> Parse(IEnumerable<string> lines) =>
        lines
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith('#'))
            .Select(line => line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .Select
            (
                parts => parts.Length == 2
                    ? new ReplaySeed(parts[0], parts[1])
                    : throw new FormatException
                    (
                        $"Fuzz replay line '{string.Join(" ", parts)}' must be '<property> <seed>'."
                    )
            )
            .ToList();



    private static async Task<IReadOnlyList<ReplaySeed>> LoadAsync() =>
        Parse(await File.ReadAllLinesAsync(CorpusPath));
}



/// <summary>
/// One replay entry: the <see cref="FuzzProperties"/> method name and the CsCheck seed it replays.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed record ReplaySeed(string Property, string Seed);
