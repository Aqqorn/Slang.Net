#pragma once
#include "slang.h"
#include "slang-com-ptr.h"
#include "slang-com-helper.h"
#include "CompilerOptionCLI.h"
#include "PreprocessorMacroDescCLI.h"
#include "TargetCLI.h"
#include <map>
#include <memory>
#include <stdexcept>

// Forward declaration to avoid circular dependency
namespace Native
{
	class ModuleCLI;
}

namespace Native
{
	class SessionCLI
	{
	public:
		// Constructor with parameters
		SessionCLI(
			CompilerOptionCLI* options, int optionsLength,
			PreprocessorMacroDescCLI* macros, int macrosLength,
			TargetCLI* models, int modelsLength,
			char** searchPaths, int searchPathsLength);

		// Destructor
		~SessionCLI();

		// Properties
		Slang::ComPtr<slang::ISession> getNative();
		static Slang::ComPtr<slang::IGlobalSession> GetGlobalSession();

		// Module management
		unsigned int getModuleCount();
		ModuleCLI* getModuleByIndex(unsigned index);

		// Static members
		static Slang::ComPtr<slang::IGlobalSession> s_context;
		static bool s_isEnableGlsl;

	private:
		Slang::ComPtr<slang::ISession> m_session;
	};
}