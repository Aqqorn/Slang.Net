namespace Slang.Sdk.Tests;

[Trait("Category", "Smoke")]
[Trait("Category", "Unit")]
[Trait("Category", "Regression")]
public class ReflectionApiShowcaseRegressionTests
{
    [Fact]
    public void ReflectionApiShowcase_ShouldNotContainPlaceholderLayoutValues()
    {
        var repoRoot = FindRepoRoot();
        var sourceFile = Path.Combine(repoRoot, "Samples", "ReflectionAPI", "ReflectionApiShowcase.cs");
        var source = File.ReadAllText(sourceFile);

        Assert.DoesNotContain("Size=<fix>", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Alignment=<fix>", source, StringComparison.Ordinal);
        Assert.Contains("GetSize(", source, StringComparison.Ordinal);
        Assert.Contains("GetAlignment(", source, StringComparison.Ordinal);
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
