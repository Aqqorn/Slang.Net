namespace Slang.Sdk.Tests;

[Trait("Category", "Smoke")]
public class TestPathHelperRegressionTests
{
    [Fact]
    public void TestPathHelper_ShouldUseCanonicalTestsDirectoryCasing_ForRepoShaderFallback()
    {
        var repoRoot = FindRepoRoot();
        var helperSource = Path.Combine(repoRoot, "Tests", "Slang.Sdk.Tests", "TestPathHelper.cs");

        Assert.True(File.Exists(helperSource), $"Expected helper source file was not found: {helperSource}");

        var text = File.ReadAllText(helperSource);

        Assert.Contains("Path.Combine(repoRoot, \"Tests\", \"Slang.Sdk.Tests\", \"Shaders\")", text, StringComparison.Ordinal);
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
