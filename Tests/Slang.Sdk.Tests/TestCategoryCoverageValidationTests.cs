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

        var offenders = GetUncategorizedTestFiles(testsDir);

        Assert.True(
            offenders.Count == 0,
            "Found test files with [Fact]/[Theory] but no Category trait (Smoke/Integration): " + string.Join(", ", offenders));
    }

    [Fact]
    public void Validator_ShouldFlagNestedTestFiles_WithoutCategoryTrait()
    {
        var tempRoot = CreateTempTestsDirectory();

        try
        {
            var nestedDir = Directory.CreateDirectory(Path.Combine(tempRoot, "Nested"));
            var nestedTestFile = Path.Combine(nestedDir.FullName, "NestedTests.cs");
            File.WriteAllText(nestedTestFile, """
namespace Slang.Sdk.Tests;

public class NestedTests
{
    [Fact]
    public void MissingCategory() { }
}
""");

            var offenders = GetUncategorizedTestFiles(tempRoot);

            Assert.Contains("NestedTests.cs", offenders);
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    [Fact]
    public void Validator_ShouldAcceptNestedTestFiles_WithIntegrationCategoryTrait()
    {
        var tempRoot = CreateTempTestsDirectory();

        try
        {
            var nestedDir = Directory.CreateDirectory(Path.Combine(tempRoot, "Nested"));
            var nestedTestFile = Path.Combine(nestedDir.FullName, "CategorizedNestedTests.cs");
            File.WriteAllText(nestedTestFile, """
namespace Slang.Sdk.Tests;

[Trait("Category", "Integration")]
public class CategorizedNestedTests
{
    [Fact]
    public void HasCategory() { }
}
""");

            var offenders = GetUncategorizedTestFiles(tempRoot);

            Assert.DoesNotContain("CategorizedNestedTests.cs", offenders);
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    private static List<string> GetUncategorizedTestFiles(string testsDir)
    {
        var files = Directory
            .EnumerateFiles(testsDir, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith("TestCategoryCoverageValidationTests.cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var offenders = new List<string>();

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            if (!HasFactOrTheory(text))
                continue;

            if (!HasRequiredCategoryTrait(text))
            {
                offenders.Add(Path.GetFileName(file));
            }
        }

        return offenders;
    }

    private static bool HasFactOrTheory(string text) => Regex.IsMatch(text, @"\[(Fact|Theory)\]");

    private static bool HasRequiredCategoryTrait(string text) =>
        Regex.IsMatch(text, @"\[Trait\(\s*""Category""\s*,\s*""(Smoke|Integration)""\s*\)\]");

    private static string CreateTempTestsDirectory()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"slang-sdk-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempPath);
        return tempPath;
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
