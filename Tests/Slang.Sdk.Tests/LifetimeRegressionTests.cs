namespace Slang.Sdk.Tests;

[Trait("Category", "Integration")]
[Trait("Category", "Stress")]
public class LifetimeRegressionTests
{
    [Fact]
    public void Session_Module_CreateDispose_Loop_ShouldRemainStable()
    {
        var shaderDirectory = TestPathHelper.GetShaderDirectory();
        var shaderFile = Path.Combine(shaderDirectory, "ComputeMinimal.slang");

        var memBefore = GC.GetTotalMemory(true);

        for (var i = 0; i < 100; i++)
        {
            using var session = new Slang.Sdk.Session.Builder()
                .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
                .AddSearchPath(shaderDirectory)
                .Create();

            var module = session.LoadModule($"AverageColor_{i}", shaderFile);
            _ = module.Program;

            if (i % 20 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memAfter = GC.GetTotalMemory(false);
        var memDiffMb = (memAfter - memBefore) / 1024.0 / 1024.0;

        Assert.True(memDiffMb < 10.0, $"Unexpected memory growth: {memDiffMb:F2} MB");
    }
}
