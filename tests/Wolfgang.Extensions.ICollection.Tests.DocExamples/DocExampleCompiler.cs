using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Wolfgang.Extensions.ICollection.Tests.DocExamples;

/// <summary>
/// Compiles an extracted &lt;example&gt;&lt;code&gt; snippet against the real library assembly
/// inside a method body, so an error means the example no longer compiles as written.
/// </summary>
public static class DocExampleCompiler
{
    /// <summary>
    /// The usings every example may rely on. The examples are written as statement lists, so
    /// they are compiled as the body of one method; nothing is ever run.
    /// </summary>
    private const string HarnessPreamble = """
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using Wolfgang.Extensions.ICollection;

        namespace DocExamplesGenerated;

        """;



    /// <summary>
    /// Compiles <paramref name="example"/> and returns every error diagnostic (warnings are
    /// ignored: an example only has to be correct, not warning-clean).
    /// </summary>
    public static IReadOnlyList<Diagnostic> Compile(DocExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        var tree = CSharpSyntaxTree.ParseText(BuildWrapperSource(example), path: "wrapper.cs");

        var compilation = CSharpCompilation.Create
        (
            assemblyName: "DocExampleCompilation",
            syntaxTrees: [tree],
            references: BuildReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        return
        [
            .. compilation
                .GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
        ];
    }



    private static string BuildWrapperSource(DocExample example)
    {
        // Forward slashes only: #line paths with backslashes are fragile across tooling.
        var normalizedPath = example.FilePath.Replace('\\', '/');

        return $$"""
            {{HarnessPreamble}}
            public static class DocExampleHarness
            {
                public static void Run()
                {
            #line {{example.FirstCodeLine}} "{{normalizedPath}}"
            {{example.Code}}
            #line default
                }
            }
            """;
    }



    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        var trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        var paths = new HashSet<string>(trustedAssemblies, StringComparer.OrdinalIgnoreCase)
        {
            typeof(ICollectionExtensions).Assembly.Location,
        };

        return [.. paths.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];
    }
}
