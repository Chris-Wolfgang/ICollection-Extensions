using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Wolfgang.Extensions.ICollection.Tests.DocExamples;

/// <summary>
/// Compiles every XML-doc &lt;example&gt;&lt;code&gt; block in the library against the real
/// assembly, so an example that references a renamed or removed member, or an undefined
/// helper, fails the build instead of rotting silently (#174).
/// </summary>
public sealed class DocExampleTests
{
    // Floor for ExtractAll_when_examples_exist_finds_at_least_the_known_count: the number of
    // <example> blocks in src/ today (AddRange, IsEmpty, IsNotEmpty).
    private const int KnownExampleCount = 3;

    private static readonly IReadOnlyList<DocExample> Examples = DocExampleSource.ExtractAll();



    public static IEnumerable<object[]> ExampleCases() => Examples.Select(example => new object[] { example });



    [Theory]
    [MemberData(nameof(ExampleCases))]
    public void Example_compiles(DocExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        var errors = DocExampleCompiler.Compile(example);

        Assert.True
        (
            errors.Count == 0,
            $"{example} failed to compile:{Environment.NewLine}" +
            string.Join(Environment.NewLine, errors)
        );
    }



    [Fact]
    public void Compile_when_example_calls_an_undefined_helper_reports_an_error()
    {
        // Proves the harness can fail: this is the kind of example #308 removed
        // (`GetResults()` was never defined).
        var example = new DocExample("synthetic.cs", 1, "var results = GetResults();");

        var errors = DocExampleCompiler.Compile(example);

        Assert.Contains(errors, e => string.Equals(e.Id, "CS0103", StringComparison.Ordinal));
    }



    [Fact]
    public void FindSrcDirectory_when_no_ancestor_has_a_src_directory_throws_DirectoryNotFoundException()
    {
        var isolatedStart = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var ex = Assert.Throws<DirectoryNotFoundException>
        (
            () => DocExampleSource.FindSrcDirectory(isolatedStart)
        );

        Assert.Contains(isolatedStart, ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void ExtractAll_when_examples_exist_finds_at_least_the_known_count()
    {
        // Guards the extractor: if its <example>/<code> matching breaks, this fails instead of
        // the Theory above running zero cases and passing vacuously.
        Assert.True
        (
            Examples.Count >= KnownExampleCount,
            $"Expected at least {KnownExampleCount} <example> blocks, found {Examples.Count}. " +
            "If this is a real removal, lower the floor deliberately; do not just delete this test."
        );
    }
}
