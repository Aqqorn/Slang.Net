using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Slang.Sdk.Interop;

/// <summary>
/// P/Invoke declarations for SlangNative.dll functions.
/// These provide direct access to the native Slang functionality.
/// </summary>
internal static unsafe partial class SlangNativeInterop
{
    private const string LibraryName = "SlangNative";

    static SlangNativeInterop()
    {
        IntPtr DllImportResolver(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName == LibraryName)
            {
                // Use the Runtime.Directory to get the correct runtime path
                var runtimeDirectory = Slang.Runtime.SlangNative_Directory;
                var libraryPath = Path.Combine(runtimeDirectory, "SlangNative.dll");

                if (File.Exists(libraryPath))
                {
                    // Preload dependencies first to ensure they are available
                    PreloadSlangDependencies(runtimeDirectory);
                    
                    // Then load the requested library
                    return NativeLibrary.Load(libraryPath);
                }
            }

            // Fall back to default resolution
            return IntPtr.Zero;
        }

        // Register a custom DL resolver to look in the runtime-specific directory
        // This is required for testing Slang.Sdk as an executable
        NativeLibrary.SetDllImportResolver(typeof(SlangNativeInterop).Assembly, DllImportResolver);
    }

    /// <summary>
    /// Preloads common Slang dependencies to ensure they are available when needed
    /// </summary>
    private static void PreloadSlangDependencies(string runtimeDirectory)
    {
        var dependencies = new[]
        {
            "slang.dll",
            "slang-glslang.dll", 
            "slang-glsl-module.dll",
            "slang-llvm.dll",
            "slang-rt.dll",
            "gfx.dll"
        };

        foreach (var dependency in dependencies)
        {
            var dependencyPath = Path.Combine(runtimeDirectory, dependency);
            if (File.Exists(dependencyPath))
            {
                try
                {
                    NativeLibrary.Load(dependencyPath);
                }
                catch
                {
                    // Ignore errors during preloading - not all dependencies may be needed
                }
            }
        }
    }

    #region Free char**
    [LibraryImport(LibraryName)]
    internal static partial void FreeChar(char** c);
    
    // New: Free generic pointer allocated by native using malloc
    [LibraryImport(LibraryName)]
    internal static partial void FreePointer(void** p);
    #endregion

    #region Global Session API
    [LibraryImport(LibraryName)]
    internal static partial void GlobalSession_SetEnableGlsl([MarshalAs(UnmanagedType.U1)] bool value, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int GlobalSession_FindProfile(char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int GlobalSession_FindCapability(char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial SlangResult GlobalSession_CheckCompileTargetSupport(int target, char** error);

    [LibraryImport(LibraryName)]
    internal static partial SlangResult GlobalSession_CheckPassThroughSupport(int passThrough, char** error);
    #endregion

    #region Session API

    [LibraryImport(LibraryName)]
    internal static partial nint Session_Create(
        void* options, int optionsLength,
        void* macros, int macrosLength,
        void* models, int modelsLength,
        char*[] searchPaths, int searchPathsLength, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void Session_Release(nint session, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint Session_GetModuleCount(nint module, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Session_GetModuleByIndex(nint module, uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Session_GetNative(nint session, char** error);

    #endregion

    #region CompileRequest API

    [LibraryImport(LibraryName)]
    internal static partial nint CompileRequest_Create(nint parentSession, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_Release(nint compileRequest, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint CompileRequest_GetNative(nint compileRequest, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddCodeGenTarget(nint compileRequest, int target, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int CompileRequest_AddEntryPoint(
        nint compileRequest,
        int translationUnitIndex,
        char* name,
        int stage,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial int CompileRequest_AddEntryPointEx(
        nint compileRequest,
        int translationUnitIndex,
        char* name,
        int stage,
        int genericArgCount,
        char*[] genericArgs,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddLibraryReference(
        nint compileRequest,
        nint baseRequest,
        char* libName,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddPreprocessorDefine(
        nint compileRequest,
        char* key,
        char* value,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddRef(nint compileRequest, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddSearchPath(
        nint compileRequest,
        char* searchDir,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddTargetCapability(
        nint compileRequest,
        int targetIndex,
        int capability,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial int CompileRequest_AddTranslationUnit(
        nint compileRequest,
        int language,
        char* name,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddTranslationUnitPreprocessorDefine(
        nint compileRequest,
        int translationUnitIndex,
        char* key,
        char* value,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddTranslationUnitSourceBlob(
        nint compileRequest,
        int translationUnitIndex,
        char* path,
        nint sourceBlob,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddTranslationUnitSourceFile(
        nint compileRequest,
        int translationUnitIndex,
        char* path,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddTranslationUnitSourceString(
        nint compileRequest,
        int translationUnitIndex,
        char* path,
        char* source,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial void CompileRequest_AddTranslationUnitSourceStringSpan(
        nint compileRequest,
        int translationUnitIndex,
        char* path,
        char* sourceBegin,
        char* sourceEnd,
        char** error);

    #endregion

    #region Module API

    [LibraryImport(LibraryName)]
    internal static partial nint Module_CreateFromCompileRequest(
    nint parentSession,
    nint compileRequest,
    char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Module_Create(
        nint parentSession, 
        char* moduleName,
        char* modulePath,
        char* shaderSource,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Module_Import(
    nint parentSession,
    char* moduleName, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void Module_Release(nint mod, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* Module_GetName(nint mod, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint Module_GetEntryPointCount(nint mod, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Module_GetEntryPointByIndex(nint mod, uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Module_FindEntryPointByName(nint mod, char* entryPointName, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Module_GetNative(nint mod, char** error);

    #endregion

    #region EntryPoint API

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPoint_Create(
        nint parentModule, 
        uint entryPointIndex, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPoint_CreateByName(
        nint parentModule, 
        char* entryPointName, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void EntryPoint_Release(nint entryPoint, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int EntryPoint_GetIndex(nint entryPoint, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* EntryPoint_GetName(nint entryPoint, char** error);

    [LibraryImport(LibraryName)]
    internal static partial SlangResult EntryPoint_Compile(nint entryPoint, int targetIndex, void** output, int* outputSize, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPoint_GetNative(nint entryPoint, char** error);
    #endregion

    #region Program API

    [LibraryImport(LibraryName)]
    internal static partial nint Program_Create(nint parentModule, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void Program_Release(nint program, char** error);

    [LibraryImport(LibraryName)]
    internal static partial SlangResult Program_CompileTarget(
    nint program,
    uint targetIndex,
    void** output,
    int* outputSize,
    char** error);

    [LibraryImport(LibraryName)]
    internal static partial SlangResult Program_CompileEntryPoint(
        nint program,
        uint entryPointIndex,
        uint targetIndex,
        void** output,
        int* outputSize,
        char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Program_GetProgramReflection(
        nint program, 
        uint targetIndex, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Program_GetNative(nint program, char** error);   
    #endregion

    #region ShaderReflection API

    [LibraryImport(LibraryName)]
    internal static partial void ShaderReflection_Release(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetParent(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint ShaderReflection_GetParameterCount(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint ShaderReflection_GetTypeParameterCount(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetTypeParameterByIndex(
        nint shaderReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_FindTypeParameter(nint shaderReflection, char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetParameterByIndex(nint shaderReflection, uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint ShaderReflection_GetEntryPointCount(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetEntryPointByIndex(
        nint shaderReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_FindEntryPointByName(
        nint shaderReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint ShaderReflection_GetGlobalConstantBufferBinding(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint ShaderReflection_GetGlobalConstantBufferSize(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_FindTypeByName(
        nint shaderReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_FindFunctionByName(
        nint shaderReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_FindFunctionByNameInType(
        nint shaderReflection, 
        nint type, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_FindVarByNameInType(
        nint shaderReflection, 
        nint type, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetTypeLayout(
        nint shaderReflection, 
        nint type, 
        int layoutRules, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_SpecializeType(
        nint shaderReflection, 
        nint type, 
        int argCount, 
        void** args, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool ShaderReflection_IsSubType(
        nint shaderReflection, 
        nint subType, 
        nint superType, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint ShaderReflection_GetHashedStringCount(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* ShaderReflection_GetHashedString(
        nint shaderReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetGlobalParamsTypeLayout(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetGlobalParamsVarLayout(nint shaderReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int ShaderReflection_ToJson(
        nint shaderReflection, 
        char** output, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint ShaderReflection_GetNative(nint shaderReflection, char** error);

    #endregion

    #region TypeReflection API

    [LibraryImport(LibraryName)]
    internal static partial void TypeReflection_Release(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeReflection_GetKind(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeReflection_GetFieldCount(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_GetFieldByIndex(
        nint typeReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool TypeReflection_IsArray(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_UnwrapArray(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint TypeReflection_GetElementCount(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_GetElementType(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeReflection_GetRowCount(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeReflection_GetColumnCount(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeReflection_GetScalarType(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_GetResourceResultType(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeReflection_GetResourceShape(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeReflection_GetResourceAccess(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* TypeReflection_GetName(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeReflection_GetUserAttributeCount(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_GetUserAttributeByIndex(
        nint typeReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_FindAttributeByName(
        nint typeReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_ApplySpecializations(
        nint typeReflection, 
        nint genRef, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_GetGenericContainer(nint typeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeReflection_GetNative(nint typeReflection, char** error);

    #endregion

    #region TypeLayoutReflection API

    [LibraryImport(LibraryName)]
    internal static partial void TypeLayoutReflection_Release(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetType(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeLayoutReflection_GetKind(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint TypeLayoutReflection_GetSize(
        nint typeLayoutReflection, 
        int category, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint TypeLayoutReflection_GetStride(
        nint typeLayoutReflection, 
        int category, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeLayoutReflection_GetAlignment(
        nint typeLayoutReflection, 
        int category, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeLayoutReflection_GetFieldCount(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetFieldByIndex(
        nint typeLayoutReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int TypeLayoutReflection_FindFieldIndexByName(
        nint typeLayoutReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetExplicitCounter(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool TypeLayoutReflection_IsArray(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_UnwrapArray(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint TypeLayoutReflection_GetElementCount(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint TypeLayoutReflection_GetTotalArrayElementCount(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint TypeLayoutReflection_GetElementStride(
        nint typeLayoutReflection, 
        int category, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetElementTypeLayout(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetElementVarLayout(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetContainerVarLayout(nint typeLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeLayoutReflection_GetNative(nint typeLayoutReflection, char** error);

    #endregion

    #region VariableReflection API

    [LibraryImport(LibraryName)]
    internal static partial void VariableReflection_Release(nint variableReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* VariableReflection_GetName(nint variableReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_GetType(nint variableReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_FindModifier(
        nint variableReflection, 
        int modifierId, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint VariableReflection_GetUserAttributeCount(nint variableReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_GetUserAttributeByIndex(
        nint variableReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_FindAttributeByName(
        nint variableReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_FindUserAttributeByName(
        nint variableReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool VariableReflection_HasDefaultValue(nint variableReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int VariableReflection_GetDefaultValueInt(
        nint variableReflection, 
        long* value, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_GetGenericContainer(nint variableReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_ApplySpecializations(
        nint variableReflection, 
        void** specializations, 
        int count, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableReflection_GetNative(nint variableReflection, char** error);

    #endregion

    #region VariableLayoutReflection API

    [LibraryImport(LibraryName)]
    internal static partial void VariableLayoutReflection_Release(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_GetVariable(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* VariableLayoutReflection_GetName(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_FindModifier(
        nint variableLayoutReflection, 
        int modifierId, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_GetTypeLayout(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int VariableLayoutReflection_GetCategory(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint VariableLayoutReflection_GetCategoryCount(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int VariableLayoutReflection_GetCategoryByIndex(
        nint variableLayoutReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nuint VariableLayoutReflection_GetOffset(
        nint variableLayoutReflection, 
        int category, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_GetType(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint VariableLayoutReflection_GetBindingIndex(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint VariableLayoutReflection_GetBindingSpace(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_GetSpace(
        nint variableLayoutReflection, 
        int category, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int VariableLayoutReflection_GetImageFormat(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* VariableLayoutReflection_GetSemanticName(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint VariableLayoutReflection_GetSemanticIndex(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint VariableLayoutReflection_GetStage(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_GetPendingDataLayout(nint variableLayoutReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint VariableLayoutReflection_GetNative(nint variableLayoutReflection, char** error);

    #endregion

    #region FunctionReflection API

    [LibraryImport(LibraryName)]
    internal static partial void FunctionReflection_Release(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* FunctionReflection_GetName(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_GetReturnType(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint FunctionReflection_GetParameterCount(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_GetParameterByIndex(
        nint functionReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_FindModifier(
        nint functionReflection, 
        int modifierId, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint FunctionReflection_GetUserAttributeCount(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_GetUserAttributeByIndex(
        nint functionReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_FindAttributeByName(
        nint functionReflection, 
        char* name, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_GetGenericContainer(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_ApplySpecializations(
        nint functionReflection, 
        nint genRef, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_SpecializeWithArgTypes(
        nint functionReflection, 
        uint typeCount, 
        void** types, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool FunctionReflection_IsOverloaded(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint FunctionReflection_GetOverloadCount(nint functionReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_GetOverload(
        nint functionReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint FunctionReflection_GetNative(nint functionReflection, char** error);

    #endregion

    #region EntryPointReflection API

    [LibraryImport(LibraryName)]
    internal static partial void EntryPointReflection_Release(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetParent(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_AsFunction(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* EntryPointReflection_GetName(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* EntryPointReflection_GetNameOverride(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint EntryPointReflection_GetParameterCount(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetParameterByIndex(
        nint entryPointReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetFunction(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int EntryPointReflection_GetStage(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial void EntryPointReflection_GetComputeThreadGroupSize(
        nint entryPointReflection, 
        uint axisCount, 
        ulong* outSizeAlongAxis, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int EntryPointReflection_GetComputeWaveSize(
        nint entryPointReflection, 
        ulong* outWaveSize, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool EntryPointReflection_UsesAnySampleRateInput(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetVarLayout(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetTypeLayout(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetResultVarLayout(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool EntryPointReflection_HasDefaultConstantBuffer(nint entryPointReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint EntryPointReflection_GetNative(nint entryPointReflection, char** error);

    #endregion

    #region GenericReflection API

    [LibraryImport(LibraryName)]
    internal static partial void GenericReflection_Release(nint genRefReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* GenericReflection_GetName(nint genRefReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint GenericReflection_GetTypeParameterCount(nint genRefReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_GetTypeParameter(
        nint genRefReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint GenericReflection_GetValueParameterCount(nint genRefReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_GetValueParameter(
        nint genRefReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint GenericReflection_GetTypeParameterConstraintCount(
        nint genRefReflection, 
        nint typeParam, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_GetTypeParameterConstraintType(
        nint genRefReflection, 
        nint typeParam, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int GenericReflection_GetInnerKind(nint genRefReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_GetOuterGenericContainer(nint genRefReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_GetConcreteType(
        nint genRefReflection, 
        nint typeParam, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int GenericReflection_GetConcreteIntVal(
        nint genRefReflection, 
        nint valueParam, 
        long* value, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_ApplySpecializations(
        nint genRefReflection, 
        nint genRef, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint GenericReflection_GetNative(nint genRefReflection, char** error);

    #endregion

    #region TypeParameterReflection API

    [LibraryImport(LibraryName)]
    internal static partial void TypeParameterReflection_Release(nint typeParameterReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* TypeParameterReflection_GetName(nint typeParameterReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeParameterReflection_GetIndex(nint typeParameterReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint TypeParameterReflection_GetConstraintCount(nint typeParameterReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeParameterReflection_GetConstraintByIndex(
        nint typeParameterReflection, 
        int index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint TypeParameterReflection_GetNative(nint typeParameterReflection, char** error);

    #endregion

    #region Attribute API

    [LibraryImport(LibraryName)]
    internal static partial void Attribute_Release(nint attributeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* Attribute_GetName(nint attributeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial uint Attribute_GetArgumentCount(nint attributeReflection, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Attribute_GetArgumentType(
        nint attributeReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int Attribute_GetArgumentValueInt(
        nint attributeReflection, 
        uint index, 
        int* value, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int Attribute_GetArgumentValueFloat(
        nint attributeReflection, 
        uint index, 
        float* value, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* Attribute_GetArgumentValueString(
        nint attributeReflection, 
        uint index, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Attribute_GetNative(nint attributeReflection, char** error);

    #endregion

    #region Modifier API

    [LibraryImport(LibraryName)]
    internal static partial void Modifier_Release(nint modifier, char** error);

    [LibraryImport(LibraryName)]
    internal static partial int Modifier_GetID(nint modifier, char** error);

    [LibraryImport(LibraryName)]
    internal static partial char* Modifier_GetName(nint modifier, char** error);

    [LibraryImport(LibraryName)]
    internal static partial nint Modifier_GetNative(nint modifier, char** error);

    #endregion
}