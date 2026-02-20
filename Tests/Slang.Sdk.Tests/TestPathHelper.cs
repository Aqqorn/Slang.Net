namespace Slang.Sdk.Tests;

internal static class TestPathHelper
{
    public static string GetShaderDirectory()
    {
        var outputShaderDir = Path.Combine(AppContext.BaseDirectory, "Tests", "Shaders");
        if (Directory.Exists(outputShaderDir))
            return outputShaderDir;

        var repoRoot = FindRepoRoot();
        var repoShaderDir = Path.Combine(repoRoot, "Tests", "Slang.Sdk.Tests", "Shaders");
        if (Directory.Exists(repoShaderDir))
            return repoShaderDir;

        throw new DirectoryNotFoundException($"Shader directory not found. Checked: {outputShaderDir} and {repoShaderDir}");
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
