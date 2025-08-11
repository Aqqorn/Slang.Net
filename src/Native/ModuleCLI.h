#pragma once
#include "slang.h"
#include "slang-com-ptr.h"
#include "slang-com-helper.h"
#include "CompileRequest.h"
#include <string>
#include <vector>
#include <memory>
#include <map>

#ifdef SLANGNATIVE_EXPORTS
#define SLANGNATIVE_API __declspec(dllexport)
#else
#define SLANGNATIVE_API __declspec(dllimport)
#endif

// Forward declarations to avoid circular dependency
namespace Native
{
	class SessionCLI;
	class CompileRequestCLI;
	class EntryPointCLI;
	class ProgramCLI;
}

namespace Native
{
	class SLANGNATIVE_API ModuleCLI
	{
	public:
		// Compile Request constructor
		ModuleCLI(SessionCLI* parent, CompileRequestCLI* compileRequest);

		// Constructor module from source
		ModuleCLI(SessionCLI* parent, const char* moduleName, const char* modulePath, const char* shaderSource);

		// Module import constructor
		ModuleCLI(SessionCLI* parent, const char* moduleName);

		// Module import constructor (from native IModule)
		ModuleCLI(SessionCLI* parent, slang::IModule* nativeModule);

		// Copy constructor for caching
		ModuleCLI(const ModuleCLI& other);

		// Destructor
		~ModuleCLI();

		// Properties
		Slang::ComPtr<slang::ISession> getParent();
		Slang::ComPtr<slang::IModule> getNative();
		const char* getName();
		slang::IComponentType* getProgramComponent();
		
		// Entry points
		unsigned int getEntryPointCount();
		EntryPointCLI* getEntryPointByIndex(unsigned index);
		EntryPointCLI* findEntryPointByName(const char* name);

		// Program access
		std::unique_ptr<ProgramCLI> getProgram();

	private:
		void initializeFromCompileRequest(SessionCLI* parent, CompileRequestCLI* compileRequest, unsigned int moduleIndex);

		// parent should not be a ComPtr here, should be SessionCLI* instead
		Slang::ComPtr<slang::ISession> m_parent;
		Slang::ComPtr<slang::IModule> m_slangModule;
		Slang::ComPtr<slang::IComponentType> m_programComponent;
	};
}

