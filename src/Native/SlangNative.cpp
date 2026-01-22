#include "SlangNative.h"
#include <cstdlib>  // For malloc and strcpy_s
#include "SessionCLI.h"
#include "ModuleCLI.h"
#include "EntryPointCLI.h"
#include "ProgramCLI.h"
#include "CompileRequest.h"
#include "Attribute.h"
#include "EntryPointReflection.h"
#include "FunctionReflection.h"
#include "GenericReflection.h"
#include "ShaderReflection.h"
#include "TypeLayoutReflection.h"
#include "TypeParameterReflection.h"
#include "TypeReflection.h"
#include "VariableLayoutReflection.h"
#include "VariableReflection.h"
#include "Modifier.h"
#include <string>
#include <cstring>

namespace SlangNative
{
	// Diagnostics - use thread_local storage for error messages to avoid memory issues
	static thread_local std::string g_lastError;
	
	// Helper function to set error message safely
	static const char* SetError(const std::string& errorMessage)
	{
		// Store last error for debugging purposes
		g_lastError = errorMessage;

		// Allocate a heap copy that the managed side can free via FreeChar/FreePointer
		const size_t len = errorMessage.size();
		char* buffer = (char*)std::malloc(len + 1);
		if (!buffer)
		{
			return nullptr; // out of memory, caller will see null
		}
		#ifdef _MSC_VER
			strcpy_s(buffer, len + 1, errorMessage.c_str());
		#else
			std::memcpy(buffer, errorMessage.c_str(), len);
			buffer[len] = '\0';
		#endif
		return buffer;
	}
	
	extern "C" SLANGNATIVE_API const char* SlangNative_GetLastError()
	{
		return g_lastError.c_str();
	}

	extern "C" SLANGNATIVE_API void FreeChar(char** c)
	{
		std::free(*c);
		*c = nullptr;
	}

	// New: generic pointer free for buffers allocated with malloc
	extern "C" SLANGNATIVE_API void FreePointer(void** p)
	{
		std::free(*p);
		*p = nullptr;
	}

	// Global Session
	extern "C" SLANGNATIVE_API void GlobalSession_SetEnableGlsl(bool value, const char** error)
	{
		if (Native::SessionCLI::s_context == nullptr)
		{
			Native::SessionCLI::s_isEnableGlsl = value;
		}
		else
		{
			// If the global session already exists, we cannot change the setting
			// This is because the global session is natively a singleton
			*error = SetError("Cannot change GLSL setting after any sessions have been created.");
		}
	}

	// Session API
	extern "C" SLANGNATIVE_API void* Session_Create(
		void* options, int optionsLength,
		void* macros, int macrosLength,
		void* models, int modelsLength,
		char* searchPaths[], int searchPathsLength,
		const char** error)
	{
		// All params as valid as null here

		try
		{
			return new SessionCLI((CompilerOptionCLI*)options, optionsLength, (PreprocessorMacroDescCLI*)macros, macrosLength, (TargetCLI*)models, modelsLength, searchPaths, searchPathsLength);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void Session_Release(void* session, const char** error)
	{
		if (!session) return;

		try
		{
			Native::SessionCLI* sessionCLI = (Native::SessionCLI*)session;
			delete sessionCLI;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API unsigned int Session_GetModuleCount(void* session, const char** error)
	{
		if (!session)
		{
			*error = SetError("Argument Null: session");
			return -1;
		}

		try
		{
			return ((SessionCLI*)session)->getModuleCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}
	extern "C" SLANGNATIVE_API void* Session_GetModuleByIndex(void* session, unsigned int index, const char** error)
	{
		if (!session)
		{
			*error = SetError("Argument Null: session");
			return nullptr;
		}

		try
		{
			return ((SessionCLI*)session)->getModuleByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
	extern "C" SLANGNATIVE_API void* Session_GetNative(void* session, const char** error)
	{
		if (!session)
		{
			*error = SetError("Argument Null: session");
			return nullptr;
		}

		try
		{
			return ((SessionCLI*)session)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// CompileRequest API
	extern "C" SLANGNATIVE_API void* CompileRequest_Create(void* parentSession, const char** error)
	{
		if (!parentSession)
		{
			*error = SetError("Argument Null: parentSession");
			return nullptr;
		}

		try
		{
			return new CompileRequestCLI((SessionCLI*)parentSession);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_Release(void* compileRequest, const char** error)
	{
		if (!compileRequest) return;

		try
		{
			Native::CompileRequestCLI* compileRequestCLI = (Native::CompileRequestCLI*)compileRequest;
			delete compileRequestCLI;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void* CompileRequest_GetNative(void* compileRequest, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return nullptr;
		}

		try
		{
			return ((CompileRequestCLI*)compileRequest)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddCodeGenTarget(void* compileRequest, int target, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addCodeGenTarget((SlangCompileTarget)target);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API int32_t CompileRequest_AddEntryPoint(void* compileRequest, int translationUnitIndex, const char* name, int stage, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return SLANG_FAIL;
		}

		try
		{
			return ((CompileRequestCLI*)compileRequest)->addEntryPoint(translationUnitIndex, name, (SlangStage)stage);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API int32_t CompileRequest_AddEntryPointEx(void* compileRequest, int translationUnitIndex, const char* name, int stage, int genericArgCount, char const** genericArgs, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return SLANG_FAIL;
		}

		try
		{
			return ((CompileRequestCLI*)compileRequest)->addEntryPointEx(translationUnitIndex, name, (SlangStage)stage, genericArgCount, genericArgs);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddLibraryReference(void* compileRequest, void* baseRequest, const char* libName, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addLibraryReference((SlangCompileRequest*)baseRequest, libName);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddPreprocessorDefine(void* compileRequest, const char* key, const char* value, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addPreprocessorDefine(key, value);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddRef(void* compileRequest, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addRef();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddSearchPath(void* compileRequest, const char* searchDir, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addSearchPath(searchDir);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddTargetCapability(void* compileRequest, int targetIndex, int capability, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addTargetCapability(targetIndex, (SlangCapabilityID)capability);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API int CompileRequest_AddTranslationUnit(void* compileRequest, int language, const char* name, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return -1;
		}

		try
		{
			return ((CompileRequestCLI*)compileRequest)->addTranslationUnit((SlangSourceLanguage)language, name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitPreprocessorDefine(void* compileRequest, int translationUnitIndex, const char* key, const char* value, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addTranslationUnitPreprocessorDefine(translationUnitIndex, key, value);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceBlob(void* compileRequest, int translationUnitIndex, const char* path, void* sourceBlob, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addTranslationUnitSourceBlob(translationUnitIndex, path, (ISlangBlob*)sourceBlob);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceFile(void* compileRequest, int translationUnitIndex, const char* path, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addTranslationUnitSourceFile(translationUnitIndex, path);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceString(void* compileRequest, int translationUnitIndex, const char* path, const char* source, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addTranslationUnitSourceString(translationUnitIndex, path, source);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void CompileRequest_AddTranslationUnitSourceStringSpan(void* compileRequest, int translationUnitIndex, const char* path, const char* sourceBegin, const char* sourceEnd, const char** error)
	{
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return;
		}

		try
		{
			((CompileRequestCLI*)compileRequest)->addTranslationUnitSourceStringSpan(translationUnitIndex, path, sourceBegin, sourceEnd);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	// Module API
	extern "C" SLANGNATIVE_API void* Module_CreateFromCompileRequest(void* parentSession, void* compileRequest, const char** error)
	{
		if (!parentSession)
		{
			*error = SetError("Argument Null: parentSession");
			return nullptr;
		}
		if (!compileRequest)
		{
			*error = SetError("Argument Null: compileRequest");
			return nullptr;
		}
		try
		{
			return new ModuleCLI((SessionCLI*)parentSession, (CompileRequestCLI*)compileRequest);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
	extern "C" SLANGNATIVE_API void* Module_Create(void* parentSession, const char* moduleName, const char* modulePath, const char* shaderSource, const char** error)
	{
		try
		{
			return new ModuleCLI((SessionCLI*)parentSession, moduleName, modulePath, shaderSource);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* Module_Import(void* parentSession, const char* moduleName, const char** error)
	{
		try
		{
			return new ModuleCLI((SessionCLI*)parentSession, moduleName);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void Module_Release(void* parent_module, const char** error)
	{
		if (!parent_module) return;

		try
		{
			Native::ModuleCLI* moduleCLI = (Native::ModuleCLI*)parent_module;
			delete moduleCLI;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API const char* Module_GetName(void* parent_module, const char** error)
	{
		if (!parent_module)
		{
			*error = SetError("Argument Null: parent_module");
			return nullptr;
		}
		try
		{
			return ((ModuleCLI*)parent_module)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int Module_GetEntryPointCount(void* parent_module, const char** error)
	{
		if (!parent_module)
		{
			*error = SetError("Argument Null: parent_module");
			return -1;
		}

		try
		{
			return ((ModuleCLI*)parent_module)->getEntryPointCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API void* Module_GetEntryPointByIndex(void* parent_module, unsigned int index, const char** error)
	{
		if (!parent_module)
		{
			*error = SetError("Argument Null: parent_module");
			return nullptr;
		}

		try
		{
			return ((ModuleCLI*)parent_module)->getEntryPointByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* Module_FindEntryPointByName(void* parent_module, const char* entryPointName, const char** error)
	{
		if (!parent_module)
		{
			*error = SetError("Argument Null: parent_module");
			return nullptr;
		}

		if (!entryPointName)
		{
			*error = SetError("Argument Null: entryPointName");
			return nullptr;
		}

		try
		{
			return ((ModuleCLI*)parent_module)->findEntryPointByName(entryPointName);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* Module_GetNative(void* parent_module, const char** error)
	{
		if (!parent_module)
		{
			*error = SetError("Argument Null: parentModule");
			return nullptr;
		}

		try
		{
			return ((ModuleCLI*)parent_module)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// EntryPoint API
	extern "C" SLANGNATIVE_API void* EntryPoint_Create(void* parentModule, unsigned int entryPointIndex, const char** error)
	{
		if (!parentModule)
		{
			*error = SetError("Argument Null: parentModule");
			return nullptr;
		}
		try
		{
			return new EntryPointCLI((ModuleCLI*)parentModule, entryPointIndex);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
	extern "C" SLANGNATIVE_API void* EntryPoint_CreateByName(void* parentModule, const char* entryPointName, const char** error)
	{
		if (!parentModule)
		{
			*error = SetError("Argument Null: parentModule");
			return nullptr;
		}
		if (!entryPointName)
		{
			*error = SetError("Argument Null: entryPointName");
			return nullptr;
		}
		try
		{
			return new EntryPointCLI((ModuleCLI*)parentModule, entryPointName);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
	extern "C" SLANGNATIVE_API void EntryPoint_Release(void* entryPoint, const char** error)
	{
		if (!entryPoint) return;
		try
		{
			Native::EntryPointCLI* entryPointCLI = (Native::EntryPointCLI*)entryPoint;
			delete entryPointCLI;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}
	extern "C" SLANGNATIVE_API int EntryPoint_GetIndex(void* entryPoint, const char** error)
	{
		if (!entryPoint) return -1;

		try
		{
			return ((EntryPointCLI*)entryPoint)->getIndex();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}
	extern "C" SLANGNATIVE_API const char* EntryPoint_GetName(void* entryPoint, const char** error)
	{
		if (!entryPoint) return nullptr;
		try
		{
			return ((EntryPointCLI*)entryPoint)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int32_t EntryPoint_Compile(void* entryPoint, unsigned int targetIndex, const void** output, int* outputSize, const char** error)
	{
		try
		{
			// [Work Around] Maybe try slang::IEntryPoint::getLayout() instead?
			EntryPointCLI* asEntryPoint = ((EntryPointCLI*)entryPoint);
			ModuleCLI* parent = asEntryPoint->getParent();
			ProgramCLI* program = new ProgramCLI(parent);
			// Get compiled code (pointer valid only while blob is alive)
			program->GetCompiled(asEntryPoint->getIndex(), targetIndex, output, outputSize);
			// Make a durable heap copy because the blob will be released when program is deleted
			if (output && *output && outputSize && *outputSize > 0)
			{
				void* copy = std::malloc(static_cast<size_t>(*outputSize));
				if (!copy)
				{
					delete program;
					*error = SetError("Out of memory while copying compiled entry point output.");
					return SLANG_FAIL;
				}
				std::memcpy(copy, *output, static_cast<size_t>(*outputSize));
				*output = copy;
			}
			delete program;

			//Try this in a later update
			//this doesn't work for some reason, maybe because not linked or something
			//return ((EntryPointCLI*)entryPoint)->Compile(targetIndex, output);
			return SLANG_OK;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API void* EntryPoint_GetNative(void* entryPoint, const char** error)
	{
		if (!entryPoint)
		{
			*error = SetError("Argument Null: entryPoint");
			return nullptr;
		}

		try
		{
			return ((EntryPointCLI*)entryPoint)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// Program API
	extern "C" SLANGNATIVE_API void* Program_Create(void* parentModule, const char** error)
	{
		try
		{
			return new ProgramCLI((ModuleCLI*)parentModule);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void Program_Release(void* program, const char** error)
	{
		if (!program) return;

		try
		{
			Native::ProgramCLI* programCLI = (Native::ProgramCLI*)program;
			delete programCLI;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API int32_t Program_CompileTarget(void* program, unsigned int targetIndex, const void** output, int* outputSize, const char** error)
	{
		try
		{
			SlangResult res = ((ProgramCLI*)program)->GetCompiled(targetIndex, output, outputSize);
			// Make a durable heap copy because the blob will be released when returning
			if (output && *output && outputSize && *outputSize > 0)
			{
				void* copy = std::malloc(static_cast<size_t>(*outputSize));
				if (!copy)
				{
					*error = SetError("Out of memory while copying compiled target output.");
					return SLANG_FAIL;
				}
				std::memcpy(copy, *output, static_cast<size_t>(*outputSize));
				*output = copy;
			}
			return res;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API int32_t Program_CompileEntryPoint(void* program, unsigned int entryPointIndex, unsigned int targetIndex, const void** output, int* outputSize, const char** error)
	{
		try
		{
			SlangResult res = ((ProgramCLI*)program)->GetCompiled(entryPointIndex, targetIndex, output, outputSize);
			// Make a durable heap copy because the blob will be released when returning
			if (output && *output && outputSize && *outputSize > 0)
			{
				void* copy = std::malloc(static_cast<size_t>(*outputSize));
				if (!copy)
				{
					*error = SetError("Out of memory while copying compiled entry point output.");
					return SLANG_FAIL;
				}
				std::memcpy(copy, *output, static_cast<size_t>(*outputSize));
				*output = copy;
			}
			return res;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API void* Program_GetProgramReflection(void* program, unsigned int targetIndex, const char** error)
	{
		if (!program) return nullptr;
		try
		{
			return new Native::ShaderReflection((ProgramCLI*)program, targetIndex);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* Program_GetNative(void* program, const char** error)
	{
		if (!program)
		{
			*error = SetError("Argument Null: program");
			return nullptr;
		}

		try
		{
			return ((ProgramCLI*)program)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// ShaderReflection API
	extern "C" SLANGNATIVE_API void ShaderReflection_Release(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return;

		try
		{
			Native::ShaderReflection* reflection = (Native::ShaderReflection*)shaderReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetParent(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getParent();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetParameterCount(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return 0;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getParameterCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetTypeParameterCount(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return 0;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getTypeParameterCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetTypeParameterByIndex(void* shaderReflection, unsigned int index, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getTypeParameterByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_FindTypeParameter(void* shaderReflection, const char* name, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->findTypeParameter(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetParameterByIndex(void* shaderReflection, unsigned int index, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getParameterByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetEntryPointCount(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return 0;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getEntryPointCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}
	extern "C" SLANGNATIVE_API void* ShaderReflection_GetEntryPointByIndex(void* shaderReflection, unsigned int index, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getEntryPointByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_FindEntryPointByName(void* shaderReflection, const char* name, const char** error)
	{
		if (!shaderReflection) return nullptr;
		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->findEntryPointByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetGlobalConstantBufferBinding(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return 0;

		try
		{
			return (unsigned int)((Native::ShaderReflection*)shaderReflection)->getGlobalConstantBufferBinding();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API size_t ShaderReflection_GetGlobalConstantBufferSize(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return 0;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getGlobalConstantBufferSize();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_FindTypeByName(void* shaderReflection, const char* name, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->findTypeByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_FindFunctionByName(void* shaderReflection, const char* name, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->findFunctionByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_FindFunctionByNameInType(void* shaderReflection, void* type, const char* name, const char** error)
	{
		if (!shaderReflection || !type) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->findFunctionByNameInType((Native::TypeReflection*)type, name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_FindVarByNameInType(void* shaderReflection, void* type, const char* name, const char** error)
	{
		if (!shaderReflection || !type) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->findVarByNameInType((Native::TypeReflection*)type, name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetTypeLayout(void* shaderReflection, void* type, int layoutRules, const char** error)
	{
		if (!shaderReflection || !type) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getTypeLayout((Native::TypeReflection*)type, (Native::LayoutRules)layoutRules);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_SpecializeType(void* shaderReflection, void* type, int argCount, void** args, const char** error)
	{
		if (!shaderReflection || !type) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->specializeType((Native::TypeReflection*)type, argCount, (Native::TypeReflection**)args, nullptr);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API bool ShaderReflection_IsSubType(void* shaderReflection, void* subType, void* superType, const char** error)
	{
		if (!shaderReflection || !subType || !superType) return false;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->isSubType((Native::TypeReflection*)subType, (Native::TypeReflection*)superType);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int ShaderReflection_GetHashedStringCount(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return 0;

		try
		{
			return (unsigned int)((Native::ShaderReflection*)shaderReflection)->getHashedStringCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API const char* ShaderReflection_GetHashedString(void* shaderReflection, unsigned int index, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			size_t count;
			return ((Native::ShaderReflection*)shaderReflection)->getHashedString(index, &count);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetGlobalParamsTypeLayout(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getGlobalParamsTypeLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetGlobalParamsVarLayout(void* shaderReflection, const char** error)
	{
		if (!shaderReflection) return nullptr;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->getGlobalParamsVarLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int ShaderReflection_ToJson(void* shaderReflection, const char** output, const char** error)
	{
		if (!shaderReflection || !output) return SLANG_FAIL;

		try
		{
			return ((Native::ShaderReflection*)shaderReflection)->toJson(output);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API void* ShaderReflection_GetNative(void* shaderReflection, const char** error)
	{
		if (!shaderReflection)
		{
			*error = SetError("Argument Null: shaderReflection");
			return nullptr;
		}

		try
		{
			return ((ShaderReflection*)shaderReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// TypeReflection API
	extern "C" SLANGNATIVE_API void TypeReflection_Release(void* typeReflection, const char** error)
	{
		if (!typeReflection) return;

		try
		{
			Native::TypeReflection* reflection = (Native::TypeReflection*)typeReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API int TypeReflection_GetKind(void* typeReflection, const char** error)
	{
		if (!typeReflection) return 0;

		try
		{
			return (int)((Native::TypeReflection*)typeReflection)->getKind();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetFieldCount(void* typeReflection, const char** error)
	{
		if (!typeReflection) return 0;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getFieldCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return 0;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_GetFieldByIndex(void* typeReflection, unsigned int index, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getFieldByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API bool TypeReflection_IsArray(void* typeReflection, const char** error)
	{
		if (!typeReflection) return false;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->isArray();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_UnwrapArray(void* typeReflection, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->unwrapArray();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API size_t TypeReflection_GetElementCount(void* typeReflection, const char** error)
	{
		if (!typeReflection) return 0;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getElementCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_GetElementType(void* typeReflection, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getElementType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetRowCount(void* typeReflection, const char** error)
	{
		if (!typeReflection) return -1;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getRowCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetColumnCount(void* typeReflection, const char** error)
	{
		if (!typeReflection) return -1;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getColumnCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API int TypeReflection_GetScalarType(void* typeReflection, const char** error)
	{
		if (!typeReflection) return -1;

		try
		{
			return (int)((Native::TypeReflection*)typeReflection)->getScalarType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_GetResourceResultType(void* typeReflection, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getResourceResultType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int TypeReflection_GetResourceShape(void* typeReflection, const char** error)
	{
		if (!typeReflection) return -1;

		try
		{
			return (int)((Native::TypeReflection*)typeReflection)->getResourceShape();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API int TypeReflection_GetResourceAccess(void* typeReflection, const char** error)
	{
		if (!typeReflection) return -1;

		try
		{
			return (int)((Native::TypeReflection*)typeReflection)->getResourceAccess();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API const char* TypeReflection_GetName(void* typeReflection, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeReflection_GetUserAttributeCount(void* typeReflection, const char** error)
	{
		if (!typeReflection) return 0;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getUserAttributeCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_GetUserAttributeByIndex(void* typeReflection, unsigned int index, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getUserAttributeByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_FindAttributeByName(void* typeReflection, const char* name, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->findAttributeByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
	extern "C" SLANGNATIVE_API void* TypeReflection_ApplySpecializations(void* typeReflection, void* genRef, const char** error)
	{
		if (!typeReflection || !genRef) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->applySpecializations((Native::GenericReflection*)genRef);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}    
	extern "C" SLANGNATIVE_API void* TypeReflection_GetGenericContainer(void* typeReflection, const char** error)
	{
		if (!typeReflection) return nullptr;

		try
		{
			return ((Native::TypeReflection*)typeReflection)->getGenericContainer();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeReflection_GetNative(void* typeReflection, const char** error)
	{
		if (!typeReflection)
		{
			*error = SetError("Argument Null: typeReflection");
			return nullptr;
		}

		try
		{
			return ((TypeReflection*)typeReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// TypeLayoutReflection API
	extern "C" SLANGNATIVE_API void TypeLayoutReflection_Release(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return;

		try
		{
			Native::TypeLayoutReflection* reflection = (Native::TypeLayoutReflection*)typeLayoutReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetType(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int TypeLayoutReflection_GetKind(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return (int)((Native::TypeLayoutReflection*)typeLayoutReflection)->getKind();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetSize(void* typeLayoutReflection, int category, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getSize((Native::ParameterCategory)category);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetStride(void* typeLayoutReflection, int category, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getStride((Native::ParameterCategory)category);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API int TypeLayoutReflection_GetAlignment(void* typeLayoutReflection, int category, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getAlignment((Native::ParameterCategory)category);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeLayoutReflection_GetFieldCount(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getFieldCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetFieldByIndex(void* typeLayoutReflection, unsigned int index, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getFieldByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int TypeLayoutReflection_FindFieldIndexByName(void* typeLayoutReflection, const char* name, const char** error)
	{
		if (!typeLayoutReflection) return -1;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->findFieldIndexByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetExplicitCounter(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getExplicitCounter();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API bool TypeLayoutReflection_IsArray(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return false;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->isArray();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_UnwrapArray(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->unwrapArray();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetElementCount(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getElementCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetTotalArrayElementCount(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getTotalArrayElementCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API size_t TypeLayoutReflection_GetElementStride(void* typeLayoutReflection, int category, const char** error)
	{
		if (!typeLayoutReflection) return 0;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getElementStride((Native::ParameterCategory)category);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetElementTypeLayout(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getElementTypeLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetElementVarLayout(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getElementVarLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetContainerVarLayout(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection) return nullptr;

		try
		{
			return ((Native::TypeLayoutReflection*)typeLayoutReflection)->getContainerVarLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeLayoutReflection_GetNative(void* typeLayoutReflection, const char** error)
	{
		if (!typeLayoutReflection)
		{
			*error = SetError("Argument Null: typeLayoutReflection");
			return nullptr;
		}

		try
		{
			return ((TypeLayoutReflection*)typeLayoutReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// VariableReflection API
	extern "C" SLANGNATIVE_API void VariableReflection_Release(void* variableReflection, const char** error)
	{
		if (!variableReflection) return;

		try
		{
			Native::VariableReflection* reflection = (Native::VariableReflection*)variableReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API const char* VariableReflection_GetName(void* variableReflection, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableReflection_GetType(void* variableReflection, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->getType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableReflection_FindModifier(void* variableReflection, int modifierId, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->findModifier((Native::Modifier::ID)modifierId);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int VariableReflection_GetUserAttributeCount(void* variableReflection, const char** error)
	{
		if (!variableReflection) return -1;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->getUserAttributeCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableReflection_GetUserAttributeByIndex(void* variableReflection, unsigned int index, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->getUserAttributeByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableReflection_FindAttributeByName(void* variableReflection, const char* name, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->findAttributeByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}    
	extern "C" SLANGNATIVE_API void* VariableReflection_FindUserAttributeByName(void* variableReflection, const char* name, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->findUserAttributeByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}    
	extern "C" SLANGNATIVE_API bool VariableReflection_HasDefaultValue(void* variableReflection, const char** error)
	{
		if (!variableReflection) return false;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->hasDefaultValue();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		}
	}    
	extern "C" SLANGNATIVE_API SlangResult VariableReflection_GetDefaultValueInt(void* variableReflection, int64_t* value, const char** error)
	{
		if (!variableReflection || !value) return SLANG_FAIL;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->getDefaultValueInt(value);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}    
	extern "C" SLANGNATIVE_API void* VariableReflection_GetGenericContainer(void* variableReflection, const char** error)
	{
		if (!variableReflection) return nullptr;

		try
		{
			return ((Native::VariableReflection*)variableReflection)->getGenericContainer();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}    
	extern "C" SLANGNATIVE_API void* VariableReflection_ApplySpecializations(void* variableReflection, void** specializations, int count, const char** error)
	{		if (!variableReflection) return nullptr;
		// For simplicity, assume specializations[0] is a GenericReflection*
		if (count > 0 && specializations && specializations[0])
		{
			return ((Native::VariableReflection*)variableReflection)->applySpecializations((Native::GenericReflection*)specializations[0]);
		}

		*error = SetError("Failed to apply specializations.");
		return nullptr;
	}

	extern "C" SLANGNATIVE_API void* VariableReflection_GetNative(void* variableReflection, const char** error)
	{
		if (!variableReflection)
		{
			*error = SetError("Argument Null: variableReflection");
			return nullptr;
		}

		try
		{
			return ((VariableReflection*)variableReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// VariableLayoutReflection API
	extern "C" SLANGNATIVE_API void VariableLayoutReflection_Release(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return;

		try
		{
			Native::VariableLayoutReflection* reflection = (Native::VariableLayoutReflection*)variableLayoutReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetVariable(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getVariable();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API const char* VariableLayoutReflection_GetName(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_FindModifier(void* variableLayoutReflection, int modifierId, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->findModifier((Native::Modifier::ID)modifierId);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetTypeLayout(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getTypeLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int VariableLayoutReflection_GetCategory(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return 0;

		try
		{
			return (int)((Native::VariableLayoutReflection*)variableLayoutReflection)->getCategory();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetCategoryCount(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return 0;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getCategoryCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API int VariableLayoutReflection_GetCategoryByIndex(void* variableLayoutReflection, unsigned int index, const char** error)
	{
		if (!variableLayoutReflection) return 0;

		try
		{
			return static_cast<int>(((Native::VariableLayoutReflection*)variableLayoutReflection)->getCategoryByIndex(index));
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API size_t VariableLayoutReflection_GetOffset(void* variableLayoutReflection, int category, const char** error)
	{
		if (!variableLayoutReflection) return 0;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getOffset((SlangParameterCategory)category);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetType(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetBindingIndex(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return -1;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getBindingIndex();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetBindingSpace(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return -1;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getBindingSpace();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetSpace(void* variableLayoutReflection, int category, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			size_t space = ((Native::VariableLayoutReflection*)variableLayoutReflection)->getBindingSpace((SlangParameterCategory)category);
			return (void*)space;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int VariableLayoutReflection_GetImageFormat(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return 0;

		try
		{
			return static_cast<int>(((Native::VariableLayoutReflection*)variableLayoutReflection)->getImageFormat());
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API const char* VariableLayoutReflection_GetSemanticName(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getSemanticName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API size_t VariableLayoutReflection_GetSemanticIndex(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return -1;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getSemanticIndex();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int VariableLayoutReflection_GetStage(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return -1;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getStage();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetPendingDataLayout(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection) return nullptr;

		try
		{
			return ((Native::VariableLayoutReflection*)variableLayoutReflection)->getPendingDataLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* VariableLayoutReflection_GetNative(void* variableLayoutReflection, const char** error)
	{
		if (!variableLayoutReflection)
		{
			*error = SetError("Argument Null: variableLayoutReflection");
			return nullptr;
		}

		try
		{
			return ((VariableLayoutReflection*)variableLayoutReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// FunctionReflection API
	extern "C" SLANGNATIVE_API void FunctionReflection_Release(void* functionReflection, const char** error)
	{
		if (!functionReflection) return;

		try
		{
			Native::FunctionReflection* reflection = (Native::FunctionReflection*)functionReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API const char* FunctionReflection_GetName(void* functionReflection, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_GetReturnType(void* functionReflection, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getReturnType();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int FunctionReflection_GetParameterCount(void* functionReflection, const char** error)
	{
		if (!functionReflection) return 0;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getParameterCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_GetParameterByIndex(void* functionReflection, unsigned int index, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getParameterByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_FindModifier(void* functionReflection, int modifierId, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->findModifier((Native::Modifier::ID)modifierId);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int FunctionReflection_GetUserAttributeCount(void* functionReflection, const char** error)
	{
		if (!functionReflection) return -1;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getUserAttributeCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_GetUserAttributeByIndex(void* functionReflection, unsigned int index, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getUserAttributeByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_FindAttributeByName(void* functionReflection, const char* name, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->findAttributeByName(name);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_GetGenericContainer(void* functionReflection, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getGenericContainer();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_ApplySpecializations(void* functionReflection, void* genRef, const char** error)
	{
		if (!functionReflection || !genRef) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->applySpecializations((Native::GenericReflection*)genRef);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_SpecializeWithArgTypes(void* functionReflection, unsigned int typeCount, void** types, const char** error)
	{
		if (!functionReflection || !types) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->specializeWithArgTypes(typeCount, (Native::TypeReflection**)types);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API bool FunctionReflection_IsOverloaded(void* functionReflection, const char** error)
	{
		if (!functionReflection) return false;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->isOverloaded();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int FunctionReflection_GetOverloadCount(void* functionReflection, const char** error)
	{
		if (!functionReflection) return 0;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getOverloadCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_GetOverload(void* functionReflection, unsigned int index, const char** error)
	{
		if (!functionReflection) return nullptr;

		try
		{
			return ((Native::FunctionReflection*)functionReflection)->getOverload(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* FunctionReflection_GetNative(void* functionReflection, const char** error)
	{
		if (!functionReflection)
		{
			*error = SetError("Argument Null: functionReflection");
			return nullptr;
		}

		try
		{
			return ((FunctionReflection*)functionReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}


	// EntryPointReflection API
	extern "C" SLANGNATIVE_API void EntryPointReflection_Release(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return;

		try
		{
			Native::EntryPointReflection* reflection = (Native::EntryPointReflection*)entryPointReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetParent(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getParent();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API const char* EntryPointReflection_GetName(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API const char* EntryPointReflection_GetNameOverride(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getNameOverride();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int EntryPointReflection_GetParameterCount(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return 0;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getParameterCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetParameterByIndex(void* entryPointReflection, unsigned int index, const char** error)
	{
		if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getParameterByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetFunction(void* entryPointReflection, const char** error)
	{
	 if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getFunction();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int EntryPointReflection_GetStage(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return 0;

		try
		{
			return (int)((Native::EntryPointReflection*)entryPointReflection)->getStage();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void EntryPointReflection_GetComputeThreadGroupSize(void* entryPointReflection, unsigned int axisCount, SlangUInt* outSizeAlongAxis, const char** error)
	{
		if (!entryPointReflection) return;

		try
		{
			((Native::EntryPointReflection*)entryPointReflection)->getComputeThreadGroupSize(3, (Native::SlangUInt*)outSizeAlongAxis);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	// TODO: [Fix] Inconsistency
	extern "C" SLANGNATIVE_API SlangResult EntryPointReflection_GetComputeWaveSize(void* entryPointReflection, SlangUInt* outWaveSize, const char** error)
	{
		if (!entryPointReflection || !outWaveSize) return SLANG_FAIL;

		try
		{
			((Native::EntryPointReflection*)entryPointReflection)->getComputeWaveSize(outWaveSize);
			return SLANG_OK;
		}
		catch (...)
		{
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API bool EntryPointReflection_UsesAnySampleRateInput(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return false;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->usesAnySampleRateInput();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		}
	}

	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetVarLayout(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getVarLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetTypeLayout(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return nullptr;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getTypeLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetResultVarLayout(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return nullptr;

	 try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->getResultVarLayout();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API bool EntryPointReflection_HasDefaultConstantBuffer(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection) return false;

		try
		{
			return ((Native::EntryPointReflection*)entryPointReflection)->hasDefaultConstantBuffer();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return false;
		 }
	 }

	extern "C" SLANGNATIVE_API void* EntryPointReflection_GetNative(void* entryPointReflection, const char** error)
	{
		if (!entryPointReflection)
		{
			*error = SetError("Argument Null: entryPointReflection");
			return nullptr;
		}

		try
		{
			return ((EntryPointReflection*)entryPointReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// GenericReflection API
	extern "C" SLANGNATIVE_API void GenericReflection_Release(void* genRefReflection, const char** error)
	{
		if (!genRefReflection) return;

		try
		{
			Native::GenericReflection* reflection = (Native::GenericReflection*)genRefReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API const char* GenericReflection_GetName(void* genRefReflection, const char** error)
	{
		if (!genRefReflection) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int GenericReflection_GetTypeParameterCount(void* genRefReflection, const char** error)
	{
		if (!genRefReflection) return 0;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getTypeParameterCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_GetTypeParameter(void* genRefReflection, unsigned int index, const char** error)
	{
		if (!genRefReflection) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getTypeParameter(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int GenericReflection_GetValueParameterCount(void* genRefReflection, const char** error)
	{
		if (!genRefReflection) return 0;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getValueParameterCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_GetValueParameter(void* genRefReflection, unsigned int index, const char** error)
	{
		if (!genRefReflection) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getValueParameter(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int GenericReflection_GetTypeParameterConstraintCount(void* genRefReflection, void* typeParam, const char** error)
	{
		if (!genRefReflection || !typeParam) return 0;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getTypeParameterConstraintCount((Native::VariableReflection*)typeParam);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_GetTypeParameterConstraintType(void* genRefReflection, void* typeParam, unsigned int index, const char** error)
	{
		if (!genRefReflection || !typeParam) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getTypeParameterConstraintType((Native::VariableReflection*)typeParam, index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API int GenericReflection_GetInnerKind(void* genRefReflection, const char** error)
	{
		if (!genRefReflection) return 0;

		try
		{
			return (int)((Native::GenericReflection*)genRefReflection)->getInnerKind();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_GetOuterGenericContainer(void* genRefReflection, const char** error)
	{
		if (!genRefReflection) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getOuterGenericContainer();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_GetConcreteType(void* genRefReflection, void* typeParam, const char** error)
	{
		if (!genRefReflection || !typeParam) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->getConcreteType((Native::VariableReflection*)typeParam);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// TODO: [Fix] Inconsistency
	extern "C" SLANGNATIVE_API SlangResult GenericReflection_GetConcreteIntVal(void* genRefReflection, void* valueParam, int64_t* value, const char** error)
	{
		if (!genRefReflection || !valueParam || !value) return SLANG_FAIL;

		try
		{
			*value = ((Native::GenericReflection*)genRefReflection)->getConcreteIntVal((Native::VariableReflection*)valueParam);
			return SLANG_OK;
		}
		catch (...)
		{
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_ApplySpecializations(void* genRefReflection, void* genRef, const char** error)
	{
		if (!genRefReflection || !genRef) return nullptr;

		try
		{
			return ((Native::GenericReflection*)genRefReflection)->applySpecializations((Native::GenericReflection*)genRef);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* GenericReflection_GetNative(void* genRefReflection, const char** error)
	{
		if (!genRefReflection)
		{
			*error = SetError("Argument Null: genRefReflection");
			return nullptr;
		}

		try
		{
			return ((GenericReflection*)genRefReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// TypeParameterReflection API
	extern "C" SLANGNATIVE_API void TypeParameterReflection_Release(void* typeParameterReflection, const char** error)
	{
		if (!typeParameterReflection) return;

		try
		{
			Native::TypeParameterReflection* reflection = (Native::TypeParameterReflection*)typeParameterReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API const char* TypeParameterReflection_GetName(void* typeParameterReflection, const char** error)
	{
		if (!typeParameterReflection) return nullptr;

		try
		{
			return ((Native::TypeParameterReflection*)typeParameterReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeParameterReflection_GetIndex(void* typeParameterReflection, const char** error)
	{
		if (!typeParameterReflection) return 0;

		try
		{
			return ((Native::TypeParameterReflection*)typeParameterReflection)->getIndex();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int TypeParameterReflection_GetConstraintCount(void* typeParameterReflection, const char** error)
	{
		if (!typeParameterReflection) return 0;

		try
		{
			return ((Native::TypeParameterReflection*)typeParameterReflection)->getConstraintCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeParameterReflection_GetConstraintByIndex(void* typeParameterReflection, int index, const char** error)
	{
		if (!typeParameterReflection) return nullptr;

		try
		{
			return ((Native::TypeParameterReflection*)typeParameterReflection)->getConstraintByIndex(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* TypeParameterReflection_GetNative(void* typeParameterReflection, const char** error)
	{
		if (!typeParameterReflection)
		{
			*error = SetError("Argument Null: typeParameterReflection");
			return nullptr;
		}

		try
		{
			return ((TypeParameterReflection*)typeParameterReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// Attribute API
	extern "C" SLANGNATIVE_API void Attribute_Release(void* attributeReflection, const char** error)
	{
		if (!attributeReflection) return;

		try
		{
			Native::Attribute* reflection = (Native::Attribute*)attributeReflection;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API const char* Attribute_GetName(void* attributeReflection, const char** error)
	{
		if (!attributeReflection) return nullptr;

		try
		{
			return ((Native::Attribute*)attributeReflection)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API unsigned int Attribute_GetArgumentCount(void* attributeReflection, const char** error)
	{
		if (!attributeReflection) return 0;

		try
		{
			return ((Native::Attribute*)attributeReflection)->getArgumentCount();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API void* Attribute_GetArgumentType(void* attributeReflection, unsigned int index, const char** error)
	{
		if (!attributeReflection) return nullptr;

		try
		{
			return ((Native::Attribute*)attributeReflection)->getArgumentType(index);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API SlangResult Attribute_GetArgumentValueInt(void* attributeReflection, unsigned int index, int* value, const char** error)
	{
		if (!attributeReflection || !value) return SLANG_FAIL;

		try
		{
			return ((Native::Attribute*)attributeReflection)->getArgumentValueInt(index, value);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API SlangResult Attribute_GetArgumentValueFloat(void* attributeReflection, unsigned int index, float* value, const char** error)
	{
		if (!attributeReflection || !value) return SLANG_FAIL;

		try
		{
			return ((Native::Attribute*)attributeReflection)->getArgumentValueFloat(index, value);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return SLANG_FAIL;
		}
	}

	extern "C" SLANGNATIVE_API const char* Attribute_GetArgumentValueString(void* attributeReflection, unsigned int index, const char** error)
	{
		if (!attributeReflection) return nullptr;

		try
		{
			size_t outSize;
			return ((Native::Attribute*)attributeReflection)->getArgumentValueString(index, &outSize);
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* Attribute_GetNative(void* attributeReflection, const char** error)
	{
		if (!attributeReflection)
		{
			*error = SetError("Argument Null: attributeReflection");
			return nullptr;
		}

		try
		{
			return ((Attribute*)attributeReflection)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	// Modifier API
	extern "C" SLANGNATIVE_API void Modifier_Release(void* modifier, const char** error)
	{
		if (!modifier) return;

		try
		{
			Native::Modifier* reflection = (Native::Modifier*)modifier;
			delete reflection;
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
		}
	}

	extern "C" SLANGNATIVE_API int Modifier_GetID(void* modifier, const char** error)
	{
		if (!modifier) return 0;

		try
		{
			return (int)((Native::Modifier*)modifier)->getID();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return -1;
		}
	}

	extern "C" SLANGNATIVE_API const char* Modifier_GetName(void* modifier, const char** error)
	{
		if (!modifier) return nullptr;

		try
		{
			return ((Native::Modifier*)modifier)->getName();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}

	extern "C" SLANGNATIVE_API void* Modifier_GetNative(void* modifier, const char** error)
	{
		if (!modifier)
		{
			*error = SetError("Argument Null: modifier");
			return nullptr;
		}

		try
		{
			return ((Modifier*)modifier)->getNative();
		}
		catch (const std::exception& e)
		{
			*error = SetError(e.what());
			return nullptr;
		}
	}
}