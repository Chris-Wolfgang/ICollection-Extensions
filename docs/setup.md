# Development Setup Guide

This guide will help you set up a development environment for contributing to Wolfgang.Extensions.ICollection.

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

1. **.NET 8.0 SDK** (or later)
   - Download from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
   - Verify installation: `dotnet --version`

2. **Git**
   - Download from [https://git-scm.com/downloads](https://git-scm.com/downloads)
   - Verify installation: `git --version`

### Recommended Tools

1. **Visual Studio 2022** (or later)
   - Community, Professional, or Enterprise edition
   - Include the ".NET desktop development" workload
   - Or **Visual Studio Code** with C# extension

2. **ReportGenerator** (for code coverage reports)
   ```bash
   dotnet tool install -g dotnet-reportgenerator-globaltool
   ```

3. **DevSkim CLI** (for security scanning)
   ```bash
   dotnet tool install --global Microsoft.CST.DevSkim.CLI
   ```

## Getting the Source Code

### 1. Fork the Repository

Visit [https://github.com/Chris-Wolfgang/ICollection-Extensions](https://github.com/Chris-Wolfgang/ICollection-Extensions) and click the "Fork" button to create your own copy.

### 2. Clone Your Fork

```bash
git clone https://github.com/YOUR-USERNAME/ICollection-Extensions.git
cd ICollection-Extensions
```

### 3. Add Upstream Remote

```bash
git remote add upstream https://github.com/Chris-Wolfgang/ICollection-Extensions.git
git fetch upstream
```

## Building the Project

### 1. Restore Dependencies

```bash
dotnet restore
```

### 2. Build the Solution

```bash
dotnet build --configuration Release
```

### 3. Run Tests

```bash
# Run all tests
dotnet test --configuration Release

# Run tests with code coverage
dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

### 4. Generate Coverage Reports

```bash
# Find all coverage files and generate report
reportgenerator -reports:"tests/**/TestResults/**/coverage.cobertura.xml" \
                -targetdir:"CoverageReport" \
                -reporttypes:"Html;TextSummary"

# View the report
# Open CoverageReport/index.html in your browser
```

## Project Structure

```
ICollection-Extensions/
├── source/                          # Source code
│   └── Wolfgang.Extensions.ICollection/
│       ├── ICollectionExtensions.cs # Main extension methods
│       └── Wolfgang.Extensions.ICollection.csproj
├── tests/                           # Test projects
│   └── Wolfgang.Extensions.ICollection.Tests.Unit/
│       ├── ICollectionExtensionTests.cs
│       └── Wolfgang.Extensions.ICollection.Tests.Unit.csproj
├── benchmarks/                      # Reserved for optional performance benchmarks (currently empty)
├── examples/                        # Example projects
├── docs/                            # Documentation
├── docfx_project/                   # DocFX documentation source
├── .github/                         # GitHub workflows and templates
│   └── workflows/
│       └── pr.yaml                  # CI/CD pipeline
└── ICollection-Extensions.slnx      # Solution file
```

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes

- Edit code in the `source/` directory
- Add or update tests in the `tests/` directory
- Follow the existing code style (see `.editorconfig`)

### 3. Run Tests Locally

```bash
# Run tests
dotnet test --configuration Release

# Check code coverage (should be above 90%)
dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

### 4. Run Security Scanning

```bash
# Run DevSkim security scan
devskim analyze --source-code . -f text --output-file devskim-results.txt -E

# Review results
cat devskim-results.txt
```

### 5. Commit Your Changes

```bash
git add .
git commit -m "feat: add your feature description"
```

### 6. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Then visit GitHub to create a pull request.

## Code Standards

### Coding Style

- Follow the `.editorconfig` rules
- Use file-scoped namespaces
- Use explicit typing (avoid `var` unless type is obvious)
- Add XML documentation comments for public APIs

### Testing Requirements

- **Minimum 90% code coverage** is required
- Write unit tests for all new functionality
- Follow existing test patterns in the project
- Test edge cases and error conditions

### Security Requirements

- DevSkim scan must pass with no errors
- No credentials or secrets in code
- Follow secure coding practices

## Continuous Integration

The project uses GitHub Actions for CI/CD. The workflow runs:

1. **Build** - Compiles the solution
2. **Test** - Runs all unit tests
3. **Coverage** - Generates code coverage reports (minimum 90%)
4. **Security** - Runs DevSkim security analysis

All checks must pass before a pull request can be merged.

## Common Tasks

### Running Benchmarks (Optional - Not Currently Available)

The `benchmarks/` folder is currently empty and reserved for future performance benchmarks. If a benchmarks project is added in the future, you can run it with:

```bash
cd benchmarks/YourBenchmarksProject
dotnet run -c Release
```

### Building NuGet Package

```bash
dotnet pack source/Wolfgang.Extensions.ICollection/Wolfgang.Extensions.ICollection.csproj \
           --configuration Release \
           --output ./artifacts
```

### Updating Documentation

1. Edit files in `docfx_project/`
2. Build documentation:
   ```bash
   cd docfx_project
   docfx build
   ```
3. Preview locally:
   ```bash
   docfx serve _site
   ```

## Troubleshooting

### Build Failures

**Problem**: Build fails with missing SDK

**Solution**: Ensure .NET 8.0 SDK is installed:
```bash
dotnet --list-sdks
```

### Test Failures

**Problem**: Tests fail due to missing dependencies

**Solution**: Restore NuGet packages:
```bash
dotnet restore
dotnet clean
dotnet build
```

### Coverage Too Low

**Problem**: Code coverage below 90%

**Solution**: Add more test cases to cover untested code paths. Review the coverage report to identify gaps:
```bash
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"CoverageReport"
# Open CoverageReport/index.html
```

### DevSkim Warnings

**Problem**: Security scan finds issues

**Solution**: Review `devskim-results.txt` and fix any security concerns. Some warnings may be false positives and can be suppressed with appropriate comments.

## Getting Help

If you encounter issues:

1. Check existing [GitHub Issues](https://github.com/Chris-Wolfgang/ICollection-Extensions/issues)
2. Review the [CONTRIBUTING.md](../CONTRIBUTING.md) guidelines
3. Open a new issue with details about your problem

## Next Steps

- **[Introduction](introduction.md)** - Learn about the library's design
- **[Getting Started](getting-started.md)** - See usage examples
- **[Contributing Guidelines](../CONTRIBUTING.md)** - Read contribution guidelines
