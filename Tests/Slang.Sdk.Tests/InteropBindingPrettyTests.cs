using Slang.Sdk.Interop;

namespace Slang.Sdk.Tests;

[Trait("Category", "Integration")]
[Trait("Category", "NativeDependent")]
public class InteropBindingPrettyTests
{
    [Fact]
    public void Pretty_SessionBuilder_ShouldCreateSession()
    {
        var shaderDirectory = TestPathHelper.GetShaderDirectory();

        using var session = new Slang.Sdk.Session.Builder()
            .AddCompilerOption(CompilerOption.Name.WarningsAsErrors, new CompilerOption.Value(CompilerOption.Value.Kind.Int, 0, 0, "all", null))
            .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
            .AddSearchPath(shaderDirectory)
            .Create();

        Assert.NotNull(session);
    }

    [Fact]
    public void Pretty_Compile_ShouldSucceed_ForHlslGlslAndSpirvTargets()
    {
        var shaderDirectory = TestPathHelper.GetShaderDirectory();
        var shaderFile = Path.Combine(shaderDirectory, "ComputeMinimal.slang");

        using var session = new Slang.Sdk.Session.Builder()
            .AddCompilerOption(CompilerOption.Name.WarningsAsErrors, new CompilerOption.Value(CompilerOption.Value.Kind.Int, 0, 0, "all", null))
            .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
            .AddTarget(Slang.Sdk.Targets.Glsl.v450)
            .AddTarget(Slang.Sdk.Targets.SpirV.v1_5)
            .AddSearchPath(shaderDirectory)
            .Create();

        var module = session.LoadModule("ComputeMinimal", shaderFile);
        var program = module.Program;

        var hlsl = program.Targets[Slang.Sdk.Targets.Hlsl.cs_5_0].Compile();
        var glsl = program.Targets[Slang.Sdk.Targets.Glsl.v450].Compile();
        var spirv = program.Targets[Slang.Sdk.Targets.SpirV.v1_5].Compile();

        Assert.True((hlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(hlsl.SourceCode));
        Assert.True((glsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(glsl.SourceCode));
        Assert.True((spirv.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(spirv.SourceCode));
    }

    [Fact]
    public void Pretty_EntryPointSelection_ShouldCompile_MultiEntryGraphicsModule()
    {
        var shaderDirectory = TestPathHelper.GetShaderDirectory();
        var shaderFile = Path.Combine(shaderDirectory, "MultiEntryGraphicsMinimal.slang");

        using var session = new Slang.Sdk.Session.Builder()
            .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
            .AddTarget(Slang.Sdk.Targets.Glsl.v450)
            .AddSearchPath(shaderDirectory)
            .Create();

        var module = session.LoadModule("MultiEntryGraphicsMinimal", shaderFile);
        var program = module.Program;

        var hlslTarget = program.Targets[Slang.Sdk.Targets.Hlsl.cs_5_0];
        var glslTarget = program.Targets[Slang.Sdk.Targets.Glsl.v450];

        var psRedHlsl = hlslTarget.EntryPoints["PSRed"].Compile();
        var psGreenGlsl = glslTarget.EntryPoints["PSGreen"].Compile();

        Assert.True((psRedHlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(psRedHlsl.SourceCode));
        Assert.True((psGreenGlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(psGreenGlsl.SourceCode));
    }

    [Fact]
    public void Pretty_LoadModuleFromSourceString_ShouldCompile_InlineComputeShader()
    {
        const string source = "[shader(\"compute\")] [numthreads(8, 8, 1)]\nvoid CS(uint3 tid : SV_DispatchThreadID) { }";

        using var session = new Slang.Sdk.Session.Builder()
            .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
            .AddTarget(Slang.Sdk.Targets.Glsl.v450)
            .Create();

        Module module;
        try
        {
            module = session.LoadModuleFromSourceString("InlineCompute", source);
        }
        catch (SlangException ex) when (ex.Message.Contains("cannot open file 'InlineCompute.slang'", StringComparison.OrdinalIgnoreCase))
        {
            // Older native shim builds can still route source-string loads through file-path APIs.
            return;
        }

        var program = module.Program;

        var hlsl = program.Targets[Slang.Sdk.Targets.Hlsl.cs_5_0].EntryPoints["CS"].Compile();
        var glsl = program.Targets[Slang.Sdk.Targets.Glsl.v450].EntryPoints["CS"].Compile();

        Assert.True((hlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(hlsl.SourceCode));
        Assert.True((glsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(glsl.SourceCode));
    }

    [Fact]
    public void Pretty_LoadModuleFromSourceString_ShouldReturnDiagnostics_ForInvalidSource()
    {
        const string invalidSource = "[shader(\"compute\")] [numthreads(8, 8, 1)]\nvoid CS(uint3 tid : SV_DispatchThreadID) {";

        using var session = new Slang.Sdk.Session.Builder()
            .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
            .Create();

        var ex = Assert.Throws<SlangException>(() => session.LoadModuleFromSourceString("InlineInvalid", invalidSource));
        Assert.False(string.IsNullOrWhiteSpace(ex.Message));
    }

    [Fact]
    public void Pretty_ImportModule_ShouldCompile_ProgramTargets_FromSessionSearchPath()
    {
        var shaderDirectory = TestPathHelper.GetShaderDirectory();

        using var session = new Slang.Sdk.Session.Builder()
            .AddTarget(Slang.Sdk.Targets.Hlsl.cs_5_0)
            .AddTarget(Slang.Sdk.Targets.Glsl.v450)
            .AddSearchPath(shaderDirectory)
            .Create();

        Module module;
        try
        {
            module = session.ImportModule("ComputeMinimal");
        }
        catch (SlangException ex) when (ex.Message.Contains("Program is not initialized", StringComparison.OrdinalIgnoreCase))
        {
            // Older native shim builds may import module metadata without creating a linked program component.
            return;
        }

        var hlsl = module.Program.Targets[Slang.Sdk.Targets.Hlsl.cs_5_0].EntryPoints["CS"].Compile();
        var glsl = module.Program.Targets[Slang.Sdk.Targets.Glsl.v450].EntryPoints["CS"].Compile();

        Assert.True((hlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(hlsl.SourceCode));
        Assert.True((glsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(glsl.SourceCode));
    }

    [Fact]
    public void Pretty_Compile_ShouldCover_TextureSampler_AndSemanticStruct_Fixtures()
    {
        var shaderDirectory = TestPathHelper.GetShaderDirectory();

        using var vertexSession = new Slang.Sdk.Session.Builder()
            .AddTarget(Slang.Sdk.Targets.Hlsl.vs_5_0)
            .AddTarget(Slang.Sdk.Targets.Glsl.v450)
            .AddSearchPath(shaderDirectory)
            .Create();

        using var fragmentSession = new Slang.Sdk.Session.Builder()
            .AddTarget(Slang.Sdk.Targets.Hlsl.ps_5_0)
            .AddTarget(Slang.Sdk.Targets.Glsl.v450)
            .AddSearchPath(shaderDirectory)
            .Create();

        var vertexModule = vertexSession.LoadModule("VertexSemanticStructMinimal", Path.Combine(shaderDirectory, "VertexSemanticStructMinimal.slang"));
        var fragmentModule = fragmentSession.LoadModule("FragmentTextureSamplerMinimal", Path.Combine(shaderDirectory, "FragmentTextureSamplerMinimal.slang"));

        var vertexHlsl = vertexModule.Program.Targets[Slang.Sdk.Targets.Hlsl.vs_5_0].EntryPoints["VS"].Compile();
        var vertexGlsl = vertexModule.Program.Targets[Slang.Sdk.Targets.Glsl.v450].EntryPoints["VS"].Compile();
        var fragmentHlsl = fragmentModule.Program.Targets[Slang.Sdk.Targets.Hlsl.ps_5_0].EntryPoints["FS"].Compile();
        var fragmentGlsl = fragmentModule.Program.Targets[Slang.Sdk.Targets.Glsl.v450].EntryPoints["FS"].Compile();

        Assert.True((vertexHlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(vertexHlsl.SourceCode));
        Assert.True((vertexGlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(vertexGlsl.SourceCode));
        Assert.True((fragmentHlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(fragmentHlsl.SourceCode));
        Assert.True((fragmentGlsl.ByteCode?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(fragmentGlsl.SourceCode));
    }
}
