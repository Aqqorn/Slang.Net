namespace Slang.Sdk.Tests;

[Trait("Category", "Unit")]
[Trait("Category", "Regression")]
public class SessionSearchPathGuardTests
{
    [Fact]
    public void FindModule_ShouldThrowActionableMessage_WhenNoSearchPathsConfigured()
    {
        using var session = new Slang.Sdk.Session.Builder().Create();

        var ex = Assert.Throws<InvalidOperationException>(() => session.FindModule("AnyModule"));

        Assert.False(string.IsNullOrWhiteSpace(ex.Message));
        Assert.Contains("AddSearchPath", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void LoadModule_WithNameAndPath_ShouldThrowActionableMessage_WhenNoSearchPathsConfigured()
    {
        using var session = new Slang.Sdk.Session.Builder().Create();

        var ex = Assert.Throws<InvalidOperationException>(() => session.LoadModule("AnyModule", "AnyPath.slang"));

        Assert.False(string.IsNullOrWhiteSpace(ex.Message));
        Assert.Contains("AddSearchPath", ex.Message, StringComparison.Ordinal);
    }
}
