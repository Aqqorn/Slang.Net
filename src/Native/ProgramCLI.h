#pragma once
#include "slang.h"
#include "slang-com-ptr.h"
#include "slang-com-helper.h"
#include <stdexcept>

// Reflection Includes
#include "LayoutRules.h"
#include "TypeDef.h"
#include "GenericArgType.h"
#include "GenericArgReflection.h"

namespace Native
{
    // Forward declarations
    struct TypeParameterReflection;
    struct TypeReflection;
    struct TypeLayoutReflection;
    struct FunctionReflection;
    struct VariableLayoutReflection;
    struct VariableReflection;
    struct EntryPointReflection;
    struct GenericReflection;
    class ModuleCLI;
}

#ifdef SLANGNATIVE_EXPORTS
#define SLANGNATIVE_API __declspec(dllexport)
#else
#define SLANGNATIVE_API __declspec(dllimport)
#endif

namespace Native
{
	class SLANGNATIVE_API ProgramCLI
	{
	public:
		// Constructor
		ProgramCLI(ModuleCLI* parent);

		// Copy constructor for caching
		ProgramCLI(const ProgramCLI& other);

		// Destructor
		~ProgramCLI();

		// Properties
		slang::IComponentType* getNative();
		ModuleCLI* getParent();
		
		// Compilation methods
		SlangResult GetCompiled(unsigned int targetIndex, const void** output, int* outputSize);
		SlangResult GetCompiled(unsigned int entryPointIndex, unsigned int targetIndex, const void** output, int* outputSize);
		
		// Layout
		slang::ProgramLayout* GetLayout(int targetIndex);

	private:
		ModuleCLI* m_parent;
		
		// Helper method for getting program components (deprecated)
		slang::IComponentType** getProgramComponents();
	};
}