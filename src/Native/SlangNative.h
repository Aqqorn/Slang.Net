#pragma once
#include "CompilerOptionCLI.h"
#include "PreprocessorMacroDescCLI.h"
#include "TargetCLI.h"
#include "ParameterInfoCLI.h"
#include "TypeDef.h"

#ifdef SLANGNATIVE_EXPORTS
#define SLANGNATIVE_API __declspec(dllexport)
#else
#define SLANGNATIVE_API __declspec(dllimport)
#endif

using namespace Native;

namespace SlangNative
{

    //Diagnostics
    extern "C" SLANGNATIVE_API const char* SlangNative_GetLastError();

    //Free char**
    extern "C" SLANGNATIVE_API void FreeChar(char** c);

    // Global Session
    extern "C" SLANGNATIVE_API void GlobalSession_SetEnableGlsl(bool value, const char** error);
    extern "C" SLANGNATIVE_API int GlobalSession_FindProfile(const char* name, const char** error);
    extern "C" SLANGNATIVE_API int GlobalSession_FindCapability(const char* name, const char** error);
    extern "C" SLANGNATIVE_API int32_t GlobalSession_CheckCompileTargetSupport(int target, const char** error);
    extern "C" SLANGNATIVE_API int32_t GlobalSession_CheckPassThroughSupport(int passThrough, const char** error);

    // Session API
    extern "C" SLANGNATIVE_API void* Session_Create(
        void* options, int optionsLength,
        void* macros, int macrosLength,
        void* models, int modelsLength,
        char* searchPaths[], int searchPathsLength,
        const char** error);
    extern "C" SLANGNATIVE_API void Session_Release(void* session, const char** error);
    extern "C" SLANGNATIVE_API unsigned int Session_GetModuleCount(void* session, const char** error);
    extern "C" SLANGNATIVE_API void* Session_GetModuleByIndex(void* session, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* Session_GetNative(void* session, const char** error);

    // Compile Request
    extern "C" SLANGNATIVE_API void* CompileRequest_Create(void* parentSession, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_Release(void* compileRequest, const char** error);
    extern "C" SLANGNATIVE_API void* CompileRequest_GetNative(void* compileRequest, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddCodeGenTarget(void* compileRequest, int target, const char** error);
    extern "C" SLANGNATIVE_API int32_t CompileRequest_AddEntryPoint(void* compileRequest, int translationUnitIndex, const char* name, int stage, const char** error);
    extern "C" SLANGNATIVE_API int32_t CompileRequest_AddEntryPointEx(void* compileRequest, int translationUnitIndex, const char* name, int stage, int genericArgCount, char const** genericArgs, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddLibraryReference(void* compileRequest, void* baseRequest, const char* libName, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddPreprocessorDefine(void* compileRequest, const char* key, const char* value, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddRef(void* compileRequest, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddSearchPath(void* compileRequest, const char* searchDir, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddTargetCapability(void* compileRequest, int targetIndex, int capability, const char** error);
    extern "C" SLANGNATIVE_API int CompileRequest_AddTranslationUnit(void* compileRequest, int language, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitPreprocessorDefine(void* compileRequest, int translationUnitIndex, const char* key, const char* value, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceBlob(void* compileRequest, int translationUnitIndex, const char* path, void* sourceBlob, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceFile(void* compileRequest, int translationUnitIndex, const char* path, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceString(void* compileRequest, int translationUnitIndex, const char* path, const char* source, const char** error);
    extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceStringSpan(void* compileRequest, int translationUnitIndex, const char* path, const char* sourceBegin, const char* sourceEnd, const char** error);

    // Module API
    extern "C" SLANGNATIVE_API void* Module_CreateFromCompileRequest(void* parentSession, void* compileRequest, const char** error);
    extern "C" SLANGNATIVE_API void* Module_Create(void* parentSession, const char* moduleName, const char* modulePath, const char* shaderSource, const char** error);
    extern "C" SLANGNATIVE_API void* Module_Import(void* parentSession, const char* moduleName, const char** error);
    extern "C" SLANGNATIVE_API void Module_Release(void* parentModule, const char** error);
	extern "C" SLANGNATIVE_API const char* Module_GetName(void* parentModule, const char** error);
	extern "C" SLANGNATIVE_API unsigned int Module_GetEntryPointCount(void* parentModule, const char** error);
    extern "C" SLANGNATIVE_API void* Module_GetEntryPointByIndex(void* parentModule, unsigned int index, const char** error);
	extern "C" SLANGNATIVE_API void* Module_FindEntryPointByName(void* parentModule, const char* entryPointName, const char** error);
    extern "C" SLANGNATIVE_API void* Module_GetNative(void* module, const char** error);

	// EntryPoint API
	extern "C" SLANGNATIVE_API void* EntryPoint_Create(void* parentModule, unsigned int entryPointIndex, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPoint_CreateByName(void* parentModule, const char* entryPointName, const char** error);
    extern "C" SLANGNATIVE_API void EntryPoint_Release(void* entryPoint, const char** error);
	extern "C" SLANGNATIVE_API int EntryPoint_GetIndex(void* entryPoint, const char** error);
    extern "C" SLANGNATIVE_API const char* EntryPoint_GetName(void* entryPoint, const char** error);
    extern "C" SLANGNATIVE_API int32_t EntryPoint_Compile(void* entryPoint, unsigned int targetIndex, const void** output, int* outputSize, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPoint_GetNative(void* entryPoint, const char** error);


    // Program API
    extern "C" SLANGNATIVE_API void* Program_Create(void* parentModule, const char** error);
    extern "C" SLANGNATIVE_API void Program_Release(void* program, const char** error);
    extern "C" SLANGNATIVE_API int32_t Program_CompileTarget(void* program, unsigned int targetIndex, const void** output, int* outputSize, const char** error);
    extern "C" SLANGNATIVE_API int32_t Program_CompileEntryPoint(void* program, unsigned int entryPointIndex, unsigned int targetIndex, const void** output, int* outputSize, const char** error);
    extern "C" SLANGNATIVE_API void* Program_GetProgramReflection(void* program, unsigned int targetIndex, const char** error);
    extern "C" SLANGNATIVE_API void* Program_GetNative(void* program, const char** error);

    // ShaderReflection API
    extern "C" SLANGNATIVE_API void ShaderReflection_Release(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetParent(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetNative(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetParameterCount(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetTypeParameterCount(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetTypeParameterByIndex(void* shaderReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_FindTypeParameter(void* shaderReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetParameterByIndex(void* shaderReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetEntryPointCount(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetEntryPointByIndex(void* shaderReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_FindEntryPointByName(void* shaderReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetGlobalConstantBufferBinding(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t ShaderReflection_GetGlobalConstantBufferSize(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_FindTypeByName(void* shaderReflection, const char* name, const char** error);    
    extern "C" SLANGNATIVE_API void* ShaderReflection_FindFunctionByName(void* shaderReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_FindFunctionByNameInType(void* shaderReflection, void* type, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_FindVarByNameInType(void* shaderReflection, void* type, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetTypeLayout(void* shaderReflection, void* type, int layoutRules, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_SpecializeType(void* shaderReflection, void* type, int argCount, void** args, const char** error);
    extern "C" SLANGNATIVE_API bool ShaderReflection_IsSubType(void* shaderReflection, void* subType, void* superType, const char** error);
    extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetHashedStringCount(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* ShaderReflection_GetHashedString(void* shaderReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetGlobalParamsTypeLayout(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetGlobalParamsVarLayout(void* shaderReflection, const char** error);
    extern "C" SLANGNATIVE_API int ShaderReflection_ToJson(void* shaderReflection, const char** output, const char** error);
    extern "C" SLANGNATIVE_API void* ShaderReflection_GetNative(void* shaderReflection, const char** error);

    // TypeReflection API
    extern "C" SLANGNATIVE_API void TypeReflection_Release(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetNative(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API int TypeReflection_GetKind(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetFieldCount(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetFieldByIndex(void* typeReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API bool TypeReflection_IsArray(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_UnwrapArray(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t TypeReflection_GetElementCount(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetElementType(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetRowCount(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetColumnCount(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API int TypeReflection_GetScalarType(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetResourceResultType(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API int TypeReflection_GetResourceShape(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API int TypeReflection_GetResourceAccess(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* TypeReflection_GetName(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetUserAttributeCount(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetUserAttributeByIndex(void* typeReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_FindAttributeByName(void* typeReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_ApplySpecializations(void* typeReflection, void* genRef, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetGenericContainer(void* typeReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeReflection_GetNative(void* typeReflection, const char** error);

    // TypeLayoutReflection API
    extern "C" SLANGNATIVE_API void TypeLayoutReflection_Release(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetNative(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetType(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API int TypeLayoutReflection_GetKind(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetSize(void* typeLayoutReflection, int category, const char** error);
    extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetStride(void* typeLayoutReflection, int category, const char** error);
    extern "C" SLANGNATIVE_API int TypeLayoutReflection_GetAlignment(void* typeLayoutReflection, int category, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeLayoutReflection_GetFieldCount(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetFieldByIndex(void* typeLayoutReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API int TypeLayoutReflection_FindFieldIndexByName(void* typeLayoutReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetExplicitCounter(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API bool TypeLayoutReflection_IsArray(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_UnwrapArray(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetElementCount(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetTotalArrayElementCount(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetElementStride(void* typeLayoutReflection, int category, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetElementTypeLayout(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetElementVarLayout(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetContainerVarLayout(void* typeLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetNative(void* typeLayoutReflection, const char** error);

    // VariableReflection API
    extern "C" SLANGNATIVE_API void VariableReflection_Release(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_GetNative(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* VariableReflection_GetName(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_GetType(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_FindModifier(void* variableReflection, int modifierId, const char** error);
    extern "C" SLANGNATIVE_API unsigned int VariableReflection_GetUserAttributeCount(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_GetUserAttributeByIndex(void* variableReflection, unsigned int index, const char** error);    
    extern "C" SLANGNATIVE_API void* VariableReflection_FindAttributeByName(void* variableReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_FindUserAttributeByName(void* variableReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API bool VariableReflection_HasDefaultValue(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API SlangResult VariableReflection_GetDefaultValueInt(void* variableReflection, int64_t* value, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_GetGenericContainer(void* variableReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_ApplySpecializations(void* variableReflection, void** specializations, int count, const char** error);
    extern "C" SLANGNATIVE_API void* VariableReflection_GetNative(void* variableReflection, const char** error);

    // VariableLayoutReflection API
    extern "C" SLANGNATIVE_API void VariableLayoutReflection_Release(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetNative(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetVariable(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* VariableLayoutReflection_GetName(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_FindModifier(void* variableLayoutReflection, int modifierId, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetTypeLayout(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API int VariableLayoutReflection_GetCategory(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetCategoryCount(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API int VariableLayoutReflection_GetCategoryByIndex(void* variableLayoutReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API size_t VariableLayoutReflection_GetOffset(void* variableLayoutReflection, int category, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetType(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetBindingIndex(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetBindingSpace(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetSpace(void* variableLayoutReflection, int category, const char** error);
    extern "C" SLANGNATIVE_API int VariableLayoutReflection_GetImageFormat(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* VariableLayoutReflection_GetSemanticName(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API size_t VariableLayoutReflection_GetSemanticIndex(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetStage(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetPendingDataLayout(void* variableLayoutReflection, const char** error);
    extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetNative(void* variableLayoutReflection, const char** error);

    // FunctionReflection API
    extern "C" SLANGNATIVE_API void FunctionReflection_Release(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetNative(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* FunctionReflection_GetName(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetReturnType(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int FunctionReflection_GetParameterCount(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetParameterByIndex(void* functionReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_FindModifier(void* functionReflection, int modifierId, const char** error);
    extern "C" SLANGNATIVE_API unsigned int FunctionReflection_GetUserAttributeCount(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetUserAttributeByIndex(void* functionReflection, unsigned int index, const char** error);    
    extern "C" SLANGNATIVE_API void* FunctionReflection_FindAttributeByName(void* functionReflection, const char* name, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetGenericContainer(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_ApplySpecializations(void* functionReflection, void* genRef, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_SpecializeWithArgTypes(void* functionReflection, unsigned int typeCount, void** types, const char** error);
    extern "C" SLANGNATIVE_API bool FunctionReflection_IsOverloaded(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int FunctionReflection_GetOverloadCount(void* functionReflection, const char** error);
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetOverload(void* functionReflection, unsigned int index, const char** error);    
    extern "C" SLANGNATIVE_API void* FunctionReflection_GetNative(void* functionReflection, const char** error);

    // EntryPointReflection API
    extern "C" SLANGNATIVE_API void EntryPointReflection_Release(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetNative(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetParent(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetNative(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_AsFunction(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* EntryPointReflection_GetName(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* EntryPointReflection_GetNameOverride(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int EntryPointReflection_GetParameterCount(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetParameterByIndex(void* entryPointReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetFunction(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API int EntryPointReflection_GetStage(void* entryPointReflection, const char** error);    
    extern "C" SLANGNATIVE_API void EntryPointReflection_GetComputeThreadGroupSize(void* entryPointReflection, unsigned int axisCount, SlangUInt* outSizeAlongAxis, const char** error);
    extern "C" SLANGNATIVE_API SlangResult EntryPointReflection_GetComputeWaveSize(void* entryPointReflection, SlangUInt* outWaveSize, const char** error);
    extern "C" SLANGNATIVE_API bool EntryPointReflection_UsesAnySampleRateInput(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetVarLayout(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetTypeLayout(void* entryPointReflection, const char** error);    
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetResultVarLayout(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API bool EntryPointReflection_HasDefaultConstantBuffer(void* entryPointReflection, const char** error);
    extern "C" SLANGNATIVE_API void* EntryPointReflection_GetNative(void* entryPointReflection, const char** error);
    
    
    // GenericReflection API
    extern "C" SLANGNATIVE_API void GenericReflection_Release(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetNative(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* GenericReflection_GetName(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int GenericReflection_GetTypeParameterCount(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetTypeParameter(void* genRefReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API unsigned int GenericReflection_GetValueParameterCount(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetValueParameter(void* genRefReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API unsigned int GenericReflection_GetTypeParameterConstraintCount(void* genRefReflection, void* typeParam, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetTypeParameterConstraintType(void* genRefReflection, void* typeParam, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API int GenericReflection_GetInnerKind(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetOuterGenericContainer(void* genRefReflection, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetConcreteType(void* genRefReflection, void* typeParam, const char** error);
    extern "C" SLANGNATIVE_API SlangResult GenericReflection_GetConcreteIntVal(void* genRefReflection, void* valueParam, int64_t* value, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_ApplySpecializations(void* genRefReflection, void* genRef, const char** error);
    extern "C" SLANGNATIVE_API void* GenericReflection_GetNative(void* genRefReflection, const char** error);

    // TypeParameterReflection API
    extern "C" SLANGNATIVE_API void TypeParameterReflection_Release(void* typeParameterReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeParameterReflection_GetNative(void* typeParameterReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* TypeParameterReflection_GetName(void* typeParameterReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeParameterReflection_GetIndex(void* typeParameterReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int TypeParameterReflection_GetConstraintCount(void* typeParameterReflection, const char** error);
    extern "C" SLANGNATIVE_API void* TypeParameterReflection_GetConstraintByIndex(void* typeParameterReflection, int index, const char** error);
    extern "C" SLANGNATIVE_API void* TypeParameterReflection_GetNative(void* typeParameterReflection, const char** error);

    // Attribute API
    extern "C" SLANGNATIVE_API void Attribute_Release(void* attributeReflection, const char** error);
    extern "C" SLANGNATIVE_API const char* Attribute_GetName(void* attributeReflection, const char** error);
    extern "C" SLANGNATIVE_API unsigned int Attribute_GetArgumentCount(void* attributeReflection, const char** error);
	extern "C" SLANGNATIVE_API void* Attribute_GetArgumentType(void* attributeReflection, unsigned int index, const char** error);
    extern "C" SLANGNATIVE_API SlangResult Attribute_GetArgumentValueInt(void* attributeReflection, unsigned int index, int* value, const char** error);
    extern "C" SLANGNATIVE_API SlangResult Attribute_GetArgumentValueFloat(void* attributeReflection, unsigned int index, float* value, const char** error);
    extern "C" SLANGNATIVE_API const char* Attribute_GetArgumentValueString(void* attributeReflection, unsigned int index, const char** error);    
    extern "C" SLANGNATIVE_API void* Attribute_GetNative(void* attributeReflection, const char** error);
    
    // Modifier API
    extern "C" SLANGNATIVE_API void Modifier_Release(void* modifier, const char** error);
    extern "C" SLANGNATIVE_API void* Modifier_GetNative(void* modifier, const char** error);
    extern "C" SLANGNATIVE_API int Modifier_GetID(void* modifier, const char** error);
    extern "C" SLANGNATIVE_API const char* Modifier_GetName(void* modifier, const char** error);
    extern "C" SLANGNATIVE_API void* Modifier_GetNative(void* modifier, const char** error);
}
