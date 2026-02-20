using System.Runtime.InteropServices;

namespace Slang.Sdk.Interop;

/// <summary>
/// Result codes returned by Slang native functions.
/// Corresponds to SlangResult in the native code.
/// </summary>
public enum SlangResult : int
{
    /// <summary>
    /// Success.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Generic failure.
    /// </summary>
    Fail = unchecked((int)0x80004005),

    /// <summary>
    /// Interface not supported.
    /// </summary>
    NoInterface = unchecked((int)0x80004002),

    /// <summary>
    /// Operation aborted.
    /// </summary>
    Abort = unchecked((int)0x80004004),

    /// <summary>
    /// Invalid argument.
    /// </summary>
    InvalidArg = unchecked((int)0x80070057),

    /// <summary>
    /// Not implemented.
    /// </summary>
    NotImplemented = unchecked((int)0x80004001),

    /// <summary>
    /// Out of memory.
    /// </summary>
    OutOfMemory = unchecked((int)0x8007000E),

    /// <summary>
    /// Invalid pointer.
    /// </summary>
    Pointer = unchecked((int)0x80004003),

    /// <summary>
    /// Invalid handle.
    /// </summary>
    Handle = unchecked((int)0x80070006),

    /// <summary>
    /// Compilation error.
    /// </summary>
    CompilationFailed = -1,

    /// <summary>
    /// Internal compiler error.
    /// </summary>
    InternalError = -2
}

public struct ParameterCategoryStruct : IEquatable<ParameterCategoryStruct>
{
    ParameterCategory Value { get; set; }

    public ParameterCategoryStruct(ParameterCategory value)
    {
        Value = value;
    }

    #region Equality
    public static bool operator ==(ParameterCategoryStruct left, ParameterCategoryStruct right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(ParameterCategoryStruct left, ParameterCategoryStruct right)
    {
        return left.Value != right.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ParameterCategoryStruct entryPoint) return Equals(entryPoint);
        return false;
    }

    public bool Equals(ParameterCategoryStruct other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    #endregion

    public static implicit operator ParameterCategoryStruct(ParameterCategory value)
    {
        return new ParameterCategoryStruct(value);
    }
}

public enum ParameterCategory
{
    None,
    Mixed,
    ConstantBuffer,
    ShaderResource,
    UnorderedAccess,
    VaryingInput,
    VaryingOutput,
    SamplerState,
    Uniform,
    DescriptorTableSlot,
    SpecializationConstant,
    PushConstantBuffer,
    RegisterSpace,
    GenericResource,
    ExistentialTypeParam,
    ExistentialObjectParam,
    SubElementRegisterSpace,
    InputAttachmentIndex,
    MetalBuffer,
    MetalTexture,
    MetalSampler,
    MetalArgumentBuffer,
    MetalAttribute,
    VertexInput,
    FragmentOutput,
    RayPayload,
    HitAttributes,
    CallablePayload,
    ShaderRecord,
    Metal,
    VulkanStorageClass,
    InlineUniformData
}

/// <summary>
/// Scalar types.
/// </summary>

public enum ScalarType
{
    None,
    Void,
    Bool,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Float16,
    Float32,
    Float64,
    Int8,
    UInt8,
    Int16,
    UInt16,
    IntPtr,
    UIntPtr
};

public enum CapabilityID
{
    Unknown = 0,
    ShaderModel60 = 1001,
    ShaderModel65 = 1005,
    ShaderModel66 = 1006,
    ShaderModel67 = 1007,
    RayTracing = 2001,
    Geometry = 2002,
    MeshShaderNv = 2101,
    MeshShaderExt = 2102,
    WaveOps = 3001,
    BindlessTexture = 4001,
    BindlessResource = 4002,
    // We ask the Slang.Net community to contribute more capabilities as needed.
};

public enum SourceLanguage
{
    Unknown,
    Slang,
    Hlsl,
    Glsl,
    C,
    Cpp,
    Cuda,
    Spirv,
    Metal,
    Wgsl,
    CountOf,
};

public enum PassThrough
{
    None,
    Fxc,
    Dxc,
    Glslang,
    SpirvDis,
    Clang,
    VisualStudio,
    Gcc,
    GenericCCpp,
    Nvrtc,
    Llvm,
    SpirvOpt,
    Metal,
    Tint,
    SpirvLink,
    CountOf,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CompilerOption
{
    public Name name;
    public Value value;

    public CompilerOption(Name name, Value value)
    {
        this.name = name;
        this.value = value;
    }

    public enum Name
    {
        MacroDefine, // stringValue0: macro name;  stringValue1: macro value
        DepFile,
        EntryPointName,
        Specialize,
        Help,
        HelpStyle,
        Include, // stringValue: additional include path.
        Language,
        MatrixLayoutColumn,         // bool
        MatrixLayoutRow,            // bool
        ZeroInitialize,             // bool
        IgnoreCapabilities,         // bool
        RestrictiveCapabilityCheck, // bool
        ModuleName,                 // stringValue0: module name.
        Output,
        Profile, // intValue0: profile
        Stage,   // intValue0: stage
        Target,  // intValue0: CodeGenTarget
        Version,
        WarningsAsErrors, // stringValue0: "all" or comma separated list of warning codes or names.
        DisableWarnings,  // stringValue0: comma separated list of warning codes or names.
        EnableWarning,    // stringValue0: warning code or name.
        DisableWarning,   // stringValue0: warning code or name.
        DumpWarningDiagnostics,
        InputFilesRemain,
        EmitIr,                        // bool
        ReportDownstreamTime,          // bool
        ReportPerfBenchmark,           // bool
        ReportCheckpointIntermediates, // bool
        SkipSPIRVValidation,           // bool
        SourceEmbedStyle,
        SourceEmbedName,
        SourceEmbedLanguage,
        DisableShortCircuit,            // bool
        MinimumSlangOptimization,       // bool
        DisableNonEssentialValidations, // bool
        DisableSourceMap,               // bool
        UnscopedEnum,                   // bool
        PreserveParameters, // bool: preserve all resource parameters in the output code.

        // Target

        Capability,                // intValue0: CapabilityName
        DefaultImageFormatUnknown, // bool
        DisableDynamicDispatch,    // bool
        DisableSpecialization,     // bool
        FloatingPointMode,         // intValue0: FloatingPointMode
        DebugInformation,          // intValue0: DebugInfoLevel
        LineDirectiveMode,
        Optimization, // intValue0: OptimizationLevel
        Obfuscate,    // bool

        VulkanBindShift, // intValue0 (higher 8 bits): kind; intValue0(lower bits): set; intValue1:
                         // shift
        VulkanBindGlobals,       // intValue0: index; intValue1: set
        VulkanInvertY,           // bool
        VulkanUseDxPositionW,    // bool
        VulkanUseEntryPointName, // bool
        VulkanUseGLLayout,       // bool
        VulkanEmitReflection,    // bool

        GLSLForceScalarLayout,   // bool
        EnableEffectAnnotations, // bool

        EmitSpirvViaGLSL,     // bool (will be deprecated)
        EmitSpirvDirectly,    // bool (will be deprecated)
        SPIRVCoreGrammarJSON, // stringValue0: json path
        IncompleteLibrary,    // bool, when set, will not issue an error when the linked program has
                              // unresolved extern function symbols.

        // Downstream

        CompilerPath,
        DefaultDownstreamCompiler,
        DownstreamArgs, // stringValue0: downstream compiler name. stringValue1: argument list, one
                        // per line.
        PassThrough,

        // Repro

        DumpRepro,
        DumpReproOnError,
        ExtractRepro,
        LoadRepro,
        LoadReproDirectory,
        ReproFallbackDirectory,

        // Debugging

        DumpAst,
        DumpIntermediatePrefix,
        DumpIntermediates, // bool
        DumpIr,            // bool
        DumpIrIds,
        PreprocessorOutput,
        OutputIncludes,
        ReproFileSystem,
        SerialIr,    // bool
        SkipCodeGen, // bool
        ValidateIr,  // bool
        VerbosePaths,
        VerifyDebugSerialIr,
        NoCodeGen, // Not used.

        // Experimental

        FileSystem,
        Heterogeneous,
        NoMangle,
        NoHLSLBinding,
        NoHLSLPackConstantBufferElements,
        ValidateUniformity,
        AllowGLSL,
        EnableExperimentalPasses,
        BindlessSpaceIndex, // int

        // Internal

        ArchiveType,
        CompileCoreModule,
        Doc,
        IrCompression,
        LoadCoreModule,
        ReferenceModule,
        SaveCoreModule,
        SaveCoreModuleBinSource,
        TrackLiveness,
        LoopInversion, // bool, enable loop inversion optimization

        // Deprecated
        ParameterBlocksUseRegisterSpaces,

        CountOfParsableOptions,

        // Used in parsed options only.
        DebugInformationFormat,  // intValue0: DebugInfoFormat
        VulkanBindShiftAll,      // intValue0: kind; intValue1: shift
        GenerateWholeProgram,    // bool
        UseUpToDateBinaryModule, // bool, when set, will only load
                                 // precompiled modules if it is up-to-date with its source.
        EmbedDownstreamIR,       // bool
        ForceDXLayout,           // bool

        // Add this new option to the end of the list to avoid breaking ABI as much as possible.
        // Setting of EmitSpirvDirectly or EmitSpirvViaGLSL will turn into this option internally.
        EmitSpirvMethod, // enum SlangEmitSpirvMethod

        EmitReflectionJSON, // bool
        SaveGLSLModuleBinSource,

        SkipDownstreamLinking, // bool, experimental
        DumpModule,
        CountOf,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Value : IDisposable
    {
        public Kind kind = Kind.Int;
        public int intValue0 = 0;
        public int intValue1 = 0;
        public IntPtr stringValue0 = IntPtr.Zero;
        public IntPtr stringValue1 = IntPtr.Zero;

        public Value(Kind kind, int intValue0, int intValue1, string? stringValue0, string? stringValue1)
        {
            this.kind = kind;
            this.intValue0 = intValue0;
            this.intValue1 = intValue1;
            this.stringValue0 = Marshal.StringToHGlobalAnsi(stringValue0);
            this.stringValue1 = Marshal.StringToHGlobalAnsi(stringValue1);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(stringValue0);
            Marshal.FreeHGlobal(stringValue1);
        }

        public enum Kind
        {
            Int,
            String
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct PreprocessorMacro : IDisposable
{
    IntPtr name;
    IntPtr value;

    public PreprocessorMacro(string name, string value)
    {
        this.name = Marshal.StringToHGlobalAnsi(name);
        this.value = Marshal.StringToHGlobalAnsi(value);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(name);
        Marshal.FreeHGlobal(value);
    }
}

public struct Target : IDisposable, IEquatable<Target>
{
    public CompileTarget target;
    public IntPtr profile;

    internal Target(CompileTarget target, string profile)
    {
        this.target = target;
        this.profile = Marshal.StringToHGlobalAnsi(profile);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(profile);
    }

    #region Equality
    public bool Equals(Target other)
    {
        if (target != other.target)
            return false;

        // Compare the actual string content, not the pointer values
        string? thisProfile = profile != IntPtr.Zero ? Marshal.PtrToStringAnsi(profile) : null;
        string? otherProfile = other.profile != IntPtr.Zero ? Marshal.PtrToStringAnsi(other.profile) : null;
        
        return target == other.target && string.Equals(thisProfile, otherProfile, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is Target other && Equals(other);
    }

    public override int GetHashCode()
    {
        string? profileString = profile != IntPtr.Zero ? Marshal.PtrToStringAnsi(profile) : null;
        return HashCode.Combine(target, profileString);
    }

    public static bool operator ==(Target left, Target right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Target left, Target right)
    {
        return !left.Equals(right);
    }
    #endregion

    public override string ToString()
    {
        string? profileString = profile != IntPtr.Zero ? Marshal.PtrToStringAnsi(profile) : "<null>";
        string result = Targets._CustomTargets.Contains(this) ? 
            $"Targets.Custom(Target.CompileTarget.{target}, { profileString})" :
            $"Targets.{target}.{profileString}";
        return result;
    }

    /// <summary>
    /// Compilation target formats.
    /// Corresponds to SlangCompileTarget in the native Slang API.
    /// </summary>
    public enum CompileTarget
    {
        TargetUnknown,
        TargetNone,
        Glsl,
        GlslVulkanDeprecated,          //< deprecated and removed: just use `GLSL`.
        GlslVulkanOneDescDeprecated,   //< deprecated and removed.
        Hlsl,
        SpirV,
        SpirVAsm,
        Dxbc,
        DxbcAsm,
        Dxil,
        DxilAsm,
        CSource,              ///< The C language
        CppSource,            ///< C++ code for shader kernels.
        HostExecutable,       ///< Standalone binary executable (for hosting CPU/OS)
        ShaderSharedLibrary,  ///< A shared library/Dll for shader kernels (for hosting
                              ///< CPU/OS)
        ShaderHostCallable,   ///< A CPU target that makes the compiled shader code available
                              ///< to be run immediately
        CudaSource,           ///< Cuda source
        Ptx,                  ///< PTX
        CudaObjectCode,       ///< Object code that contains CUDA functions.
        ObjectCode,           ///< Object code that can be used for later linking
        HostCppSource,        ///< C++ code for host library or executable.
        HostHostCallable,     ///< Host callable host code (ie non kernel/shader)
        CppPytorchBinding,    ///< C++ PyTorch binding code.
        Metal,                ///< Metal shading language
        MetalLib,             ///< Metal library
        MetalLibAsm,          ///< Metal library assembly
        HostSharedLibrary,    ///< A shared library/Dll for host code (for hosting CPU/OS)
        Wgsl,                 ///< WebGPU shading language
        WgslSpirvAsm,         ///< SPIR-V assembly via WebGPU shading language
        WgslSpirv,            ///< SPIR-V via WebGPU shading language

        HostVm,               ///< Bytecode that can be interpreted by the Slang VM
        TargetCountOf,
    }

    public enum CompileOutputType
    {
        ByteCode,
        SourceCode,
    }
}

public enum Stage
{
    None,
    Vertex,
    Hull,
    Domain,
    Geometry,
    Fragment,
    Compute,
    RayGeneration,
    Intersection,
    AnyHit,
    ClosestHit,
    Miss,
    Callable,
    Mesh,
    Amplification,
    Dispatch,
    //
    Count,

    // alias:
    Pixel = Fragment,
};

#region Reflection Slang Types

/// <summary>
/// Type kinds for reflection.
/// </summary>
public enum TypeKind
{
    None,
    Struct,
    Array,
    Matrix,
    Vector,
    Scalar,
    ConstantBuffer,
    Resource,
    SamplerState,
    TextureBuffer,
    ShaderStorageBuffer,
    ParameterBlock,
    GenericTypeParameter,
    Interface,
    OutputStream,
    Specialized,
    Feedback,
    Pointer,
    DynamicResource,
    MeshOutput,
};

public enum ResourceShape
{
    ResourceBaseShapeMask = 0x0F,

    ResourceNone = 0x00,

    Texture1D = 0x01,
    Texture2D = 0x02,
    Texture3D = 0x03,
    TextureCube = 0x04,
    TextureBuffer = 0x05,

    StructuredBuffer = 0x06,
    ByteAddressBuffer = 0x07,
    ResourceUnknown = 0x08,
    AccelerationStructure = 0x09,
    TextureSubpass = 0x0A,

    ResourceExtShapeMask = 0x1F0,

    TextureFeedbackFlag = 0x10,
    TextureShadowFlag = 0x20,
    TextureArrayFlag = 0x40,
    TextureMultiSampleFlag = 0x80,
    TextureCombinedFlag = 0x100,

    Texture1DArray = Texture1D | TextureArrayFlag,
    Texture2DArray = Texture2D | TextureArrayFlag,
    TextureCubeArray = TextureCube | TextureArrayFlag,

    Texture2DMultiSample = Texture2D | TextureMultiSampleFlag,
    Texture2DMultiSampleArray =
        Texture2D | TextureMultiSampleFlag | TextureArrayFlag,
    TextureSubpassMultiSample = TextureSubpass | TextureMultiSampleFlag,
};

#endregion