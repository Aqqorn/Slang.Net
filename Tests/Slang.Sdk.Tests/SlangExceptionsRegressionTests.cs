using Slang.Sdk.Interop;

namespace Slang.Sdk.Tests;

[Trait("Category", "Unit")]
[Trait("Category", "Regression")]
public class SlangExceptionsRegressionTests
{
    [Theory]
    [InlineData(SlangResult.Ok, true)]
    [InlineData((SlangResult)1, true)]
    [InlineData((SlangResult)int.MaxValue, true)]
    [InlineData((SlangResult)(-1), false)]
    [InlineData((SlangResult)int.MinValue, false)]
    public void IsSuccess_ShouldFollowSignedBoundaryAtZero(SlangResult result, bool expected)
    {
        var isSuccess = SlangResultHelper.IsSuccess(result);
        var isFailure = SlangResultHelper.IsFailure(result);

        Assert.Equal(expected, isSuccess);
        Assert.Equal(!expected, isFailure);
    }

    [Theory]
    [InlineData(SlangResult.Ok, false)]
    [InlineData((SlangResult)1, false)]
    [InlineData((SlangResult)int.MaxValue, false)]
    [InlineData((SlangResult)(-1), true)]
    [InlineData((SlangResult)int.MinValue, true)]
    public void IsFailure_ShouldFollowSignedBoundaryAtZero(SlangResult result, bool expected)
    {
        Assert.Equal(expected, SlangResultHelper.IsFailure(result));
        Assert.Equal(!expected, SlangResultHelper.IsSuccess(result));
    }

    [Theory]
    [InlineData(SlangResult.Ok)]
    [InlineData((SlangResult)1)]
    [InlineData((SlangResult)int.MaxValue)]
    public void ThrowOnFailure_ShouldNotThrow_ForSuccessCodes(SlangResult result)
    {
        var ex = Record.Exception(() => SlangResultHelper.ThrowOnFailure(result));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(SlangResult.CompilationFailed)]
    [InlineData(SlangResult.InternalError)]
    [InlineData((SlangResult)int.MinValue)]
    public void ThrowOnFailure_ShouldThrowSlangException_ForFailureCodes(SlangResult result)
    {
        var ex = Assert.Throws<SlangException>(() => SlangResultHelper.ThrowOnFailure(result));

        Assert.Equal(result, ex.Result);
    }

    [Fact]
    public void ThrowOnFailure_ShouldUseProvidedCustomMessage_WhenFailure()
    {
        const string customMessage = "Custom failure";

        var ex = Assert.Throws<SlangException>(() => SlangResultHelper.ThrowOnFailure(SlangResult.Fail, customMessage));

        Assert.Equal(SlangResult.Fail, ex.Result);
        Assert.Equal(customMessage, ex.Message);
    }

    [Theory]
    [InlineData(SlangResult.Ok, "Success")]
    [InlineData(SlangResult.Fail, "Generic failure")]
    [InlineData(SlangResult.NoInterface, "Interface not supported")]
    [InlineData(SlangResult.Abort, "Operation aborted")]
    [InlineData(SlangResult.InvalidArg, "Invalid argument")]
    [InlineData(SlangResult.NotImplemented, "Not implemented")]
    [InlineData(SlangResult.OutOfMemory, "Out of memory")]
    [InlineData(SlangResult.Pointer, "Invalid pointer")]
    [InlineData(SlangResult.Handle, "Invalid handle")]
    [InlineData(SlangResult.CompilationFailed, "Compilation failed")]
    [InlineData(SlangResult.InternalError, "Internal compiler error")]
    public void SlangException_ShouldMapKnownCodes_ToExpectedMessages(SlangResult result, string expectedMessage)
    {
        var ex = new SlangException(result);

        Assert.Equal(result, ex.Result);
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-123456)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void SlangException_ShouldUseUnknownFallback_ForUnmappedCodes(int rawCode)
    {
        var result = (SlangResult)rawCode;

        var ex = new SlangException(result);

        Assert.Equal($"Unknown error (code: {rawCode})", ex.Message);
    }
}
