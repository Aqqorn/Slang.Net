#include "ProgramCLI.h"
#include "ModuleCLI.h"
#include "EntryPointCLI.h"
#include <iostream>
#include <stdexcept>

// Reflection includes
#include "TypeParameterReflection.h"
#include "TypeReflection.h"
#include "TypeLayoutReflection.h"
#include "FunctionReflection.h"
#include "VariableLayoutReflection.h"
#include "VariableReflection.h"
#include "EntryPointReflection.h"
#include "GenericReflection.h"
#include "GenericArgReflection.h"
#include "ShaderReflection.h"

Native::ProgramCLI::ProgramCLI(ModuleCLI* parent)
	: m_parent(parent)
{
	if (!parent)
		throw std::invalid_argument("Parent module cannot be null");
}

Native::ProgramCLI::ProgramCLI(const ProgramCLI& other)
{
	// Copy constructor for caching support
	m_parent = other.m_parent;
}

Native::ProgramCLI::~ProgramCLI()
{
	// ComPtr will automatically release the composed program
	// m_parent is not owned by this class, so no cleanup needed
}

SlangResult Native::ProgramCLI::GetCompiled(unsigned int targetIndex, const void** output, int* outputSize)
{
	if (!output)
		throw std::invalid_argument("Output pointer cannot be null");
	if (!outputSize)
		throw std::invalid_argument("Output size pointer cannot be null");

	if (!m_parent->getProgramComponent())
		throw std::runtime_error("Program is not initialized");

	Slang::ComPtr<slang::IBlob> targetCode;
	Slang::ComPtr<slang::IBlob> diagnosticsBlob;
	
	SlangResult result = m_parent->getProgramComponent()->getTargetCode(
		targetIndex, 
		targetCode.writeRef(), 
		diagnosticsBlob.writeRef());

	// Handle diagnostics
	if (diagnosticsBlob && diagnosticsBlob->getBufferSize() > 0)
	{
		std::string diagnosticsText = std::string((const char*)diagnosticsBlob->getBufferPointer());
		
		if (SLANG_FAILED(result))
		{
			throw std::runtime_error("Program compilation failed: " + diagnosticsText);
		}
		else
		{
			std::cout << "Program compilation diagnostics: " << diagnosticsText << std::endl;
		}
	}
	else if (SLANG_FAILED(result))
	{
		throw std::runtime_error("Program compilation failed. Error code: " + std::to_string(result) + 
			". Target index " + std::to_string(targetIndex) + " may not exist.");
	}

	if (!targetCode)
	{
		throw std::runtime_error("No target code generated. Target index " + std::to_string(targetIndex) + " may be invalid.");
	}

	// Set output values
	*output = targetCode->getBufferPointer();
	*outputSize = static_cast<int>(targetCode->getBufferSize());

	return result;
}

SlangResult Native::ProgramCLI::GetCompiled(unsigned int entryPointIndex, unsigned int targetIndex, const void** output, int* outputSize)
{
	if (!output)
		throw std::invalid_argument("Output pointer cannot be null");
	if (!outputSize)
		throw std::invalid_argument("Output size pointer cannot be null");

	if (!m_parent->getProgramComponent())
		throw std::runtime_error("Program is not initialized");

	Slang::ComPtr<slang::IBlob> targetCode;
	Slang::ComPtr<slang::IBlob> diagnosticsBlob;
	
	SlangResult result = m_parent->getProgramComponent()->getEntryPointCode(
		entryPointIndex,
		targetIndex,
		targetCode.writeRef(),
		diagnosticsBlob.writeRef());

	// Handle diagnostics
	if (diagnosticsBlob && diagnosticsBlob->getBufferSize() > 0)
	{
		std::string diagnosticsText = std::string((const char*)diagnosticsBlob->getBufferPointer());
		
		if (SLANG_FAILED(result))
		{
			throw std::runtime_error("Entry point compilation failed: " + diagnosticsText);
		}
		else
		{
			std::cout << "Entry point compilation diagnostics: " << diagnosticsText << std::endl;
		}
	}
	else if (SLANG_FAILED(result))
	{
		throw std::runtime_error("Entry point compilation failed. Error code: " + std::to_string(result) + 
			". Entry point index " + std::to_string(entryPointIndex) + 
			" or target index " + std::to_string(targetIndex) + " may not exist.");
	}

	if (!targetCode)
	{
		throw std::runtime_error("No target code generated for entry point " + std::to_string(entryPointIndex) + 
			", target " + std::to_string(targetIndex));
	}

	// Set output values
	*output = targetCode->getBufferPointer();
	*outputSize = static_cast<int>(targetCode->getBufferSize());

	return result;
}

slang::ProgramLayout* Native::ProgramCLI::GetLayout(int targetIndex)
{
	if (!m_parent->getProgramComponent())
		throw std::runtime_error("Program is not initialized");

	Slang::ComPtr<slang::IBlob> diagnosticsBlob;
	slang::ProgramLayout* result = m_parent->getProgramComponent()->getLayout(targetIndex, diagnosticsBlob.writeRef());

	// Handle diagnostics
	if (diagnosticsBlob && diagnosticsBlob->getBufferSize() > 0)
	{
		std::string diagnosticsText = std::string((const char*)diagnosticsBlob->getBufferPointer());
		
		if (!result)
		{
			throw std::runtime_error("Failed to get program layout: " + diagnosticsText);
		}
		else
		{
			std::cout << "Program layout diagnostics: " << diagnosticsText << std::endl;
		}
	}
	else if (!result)
	{
		throw std::runtime_error("Failed to get program layout for target index " + std::to_string(targetIndex) + 
			". Target may not exist.");
	}

	return result;
}

slang::IComponentType* Native::ProgramCLI::getNative()
{
	return m_parent->getProgramComponent();
}

Native::ModuleCLI* Native::ProgramCLI::getParent()
{
	return m_parent;
}

// Helper method - now obsolete but kept for compatibility
slang::IComponentType** Native::ProgramCLI::getProgramComponents()
{
	if (!m_parent)
		return nullptr;

	unsigned int entryPointCount = m_parent->getEntryPointCount();
	unsigned int componentCount = entryPointCount + 1;

	// Note: This method has potential memory management issues
	// Consider redesigning to use smart pointers or RAII containers
	slang::IComponentType** programComponents = new slang::IComponentType*[componentCount];
	slang::IEntryPoint** entryPoints = new slang::IEntryPoint*[entryPointCount];

	try
	{
		// Fill entry points
		for (unsigned int i = 0; i < entryPointCount; i++)
		{
			entryPoints[i] = m_parent->getEntryPointByIndex(i)->getNative();
		}

		// Fill program components
		for (unsigned int i = 0; i < entryPointCount; i++)
		{
			if (i < componentCount)
				programComponents[i] = entryPoints[i];
		}

		// Add the native module as the last component
		programComponents[componentCount - 1] = m_parent->getNative();

		delete[] entryPoints;
		return programComponents;
	}
	catch (...)
	{
		delete[] entryPoints;
		delete[] programComponents;
		throw;
	}
}