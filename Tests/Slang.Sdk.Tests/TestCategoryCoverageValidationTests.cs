using System.Text.RegularExpressions;

namespace Slang.Sdk.Tests;

[Trait("Category", "Smoke")]
public class TestCategoryCoverageValidationTests
{
    [Fact]
    public void AllXunitTests_ShouldDeclareSmokeOrIntegrationCategory()
    {
        var repoRoot = FindRepoRoot();
        var testsDir = Path.Combine(repoRoot, "Tests", "Slang.Sdk.Tests");

        var files = Directory
            .EnumerateFiles(testsDir, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(f => !f.EndsWith("TestCategoryCoverageValidationTests.cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var offenders = new List<string>();

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            var hasFactOrTheory = Regex.IsMatch(text, @"\[(Fact|Theory)\]");
            if (!hasFactOrTheory)
                continue;

            var hasCategory = Regex.IsMatch(text, @"\[Trait\(\s*\"Category\"\s*,\s*\"(Smoke|Integration)\"\s*\)\]");
            if (!hasCategory)
            {
                offenders.Add(Path.GetFileName(file));
            }
        }

        Assert.True(
            offenders.Count == 0,
            "Found test files with [Fact]/[Theory] but no Category trait (Smoke/Integration): " + string.Join(", ", offenders));
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Slang.Net.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing Slang.Net.sln");
    }
}
