using System;
using System.Reflection;
using BenchmarkDotNet.Running;
using Wolfgang.Extensions.ICollection;

// Say which library build the scenarios run against, so a baseline-release run that silently
// fell back to the project reference is visible in the log (shadow.yaml checks this line).
Console.WriteLine
(
    "Library under test: "
    + typeof(ICollectionExtensions).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
