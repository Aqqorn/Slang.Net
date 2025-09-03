#include "ModuleCLI.h"
#include "SessionCLI.h"
#include "CompileRequest.h"
#include "EntryPointCLI.h"
#include "ProgramCLI.h"
#include <iostream>
#include <memory>
#include <vector>

Native::ModuleCLI::ModuleCLI(SessionCLI* parent, CompileRequestCLI* compileRequest)
{
	if (!parent)
		throw std::invalid_argument("Parent session cannot be null");
	if (!compileRequest)
		throw std::invalid_argument("Compile request cannot be null");

	m_parent = parent->getNative();
	unsigned int moduleIndex = parent->getModuleCount();
	
	initializeFromCompileRequest(parent, compileRequest, moduleIndex);
	compileRequest->getNative()->getProgramWithEntryPoints(m_programComponent.writeRef());
}

Native::ModuleCLI::ModuleCLI(SessionCLI* parent, const char* moduleName, const char* modulePath, const char* shaderSource)
{
	if (!parent)
		throw std::invalid_argument("Parent session cannot be null");
	if (!moduleName)
		throw std::invalid_argument("Module name cannot be null");
	if (!modulePath)
		throw std::invalid_argument("Module path cannot be null");
	if (!shaderSource)
		throw std::invalid_argument("Shader source cannot be null");

	m_parent = parent->getNative();
	unsigned int moduleIndex = parent->getModuleCount();

	// Create compile request
	auto compileRequest = CompileRequestCLI(parent);
	
	// Add the shader file
	compileRequest.addTranslationUnit(SLANG_SOURCE_LANGUAGE_SLANG, moduleName);
	compileRequest.addTranslationUnitSourceFile(moduleIndex, modulePath);

	initializeFromCompileRequest(parent, &compileRequest, moduleIndex);
	compileRequest.getNative()->getProgramWithEntryPoints(m_programComponent.writeRef());
}

Native::ModuleCLI::ModuleCLI(SessionCLI* parent, const char* moduleName)
{
	if (!parent)
		throw std::invalid_argument("Parent session cannot be null");
	if (!moduleName)
		throw std::invalid_argument("Module name cannot be null");

	m_parent = parent->getNative();
	
	Slang::ComPtr<slang::IBlob> diagnosticsBlob;
	m_slangModule = m_parent->loadModule(moduleName, diagnosticsBlob.writeRef());

	// Handle diagnostics
	if (diagnosticsBlob && diagnosticsBlob->getBufferSize() > 0)
	{
		std::string diagnosticsText = std::string((const char*)diagnosticsBlob->getBufferPointer());
		std::string errorMessage = "Issues loading module '" + std::string(moduleName) + "': " + diagnosticsText;

		if (!m_slangModule)
			throw std::runtime_error(errorMessage);
		else
			std::cout << diagnosticsText << std::endl;
	}
	else if (!m_slangModule)
	{
		throw std::runtime_error("Failed to load module '" + std::string(moduleName) + "'. No diagnostics available.");
	}
	// TODO: programComponent
}

Native::ModuleCLI::ModuleCLI(SessionCLI* parent, slang::IModule* nativeModule)
{
	if (!parent)
		throw std::invalid_argument("Parent session cannot be null");
	if (!nativeModule)
		throw std::invalid_argument("Native module cannot be null");

	m_parent = parent->getNative();
	m_slangModule = nativeModule;
	// TODO: programComponent
}

Native::ModuleCLI::ModuleCLI(const ModuleCLI& other)
{
	m_parent = other.m_parent;
	m_slangModule = other.m_slangModule;
	
	// Don't copy the compile request - it's not needed for copied modules
	// Don't copy cached objects - they'll be recreated on demand
}

Native::ModuleCLI::~ModuleCLI()
{
	// ComPtr will automatically release Slang interfaces
}

void Native::ModuleCLI::initializeFromCompileRequest(SessionCLI* parent, CompileRequestCLI* compileRequest, unsigned int moduleIndex)
{
	// Compile the module
	if (compileRequest->getNative()->compile() != SLANG_OK)
	{
		auto diagnostics = compileRequest->getNative()->getDiagnosticOutput();
		throw std::runtime_error(std::string("Slang compile error:\n") + (diagnostics ? diagnostics : "Unknown error"));
	}

	// Get the compiled module
	Slang::ComPtr<slang::IModule> slangModule;
	SlangResult result = compileRequest->getNative()->getModule(moduleIndex, slangModule.writeRef());
	
	if (SLANG_FAILED(result) || !slangModule)
	{
		auto diagnostics = compileRequest->getNative()->getDiagnosticOutput();
		throw std::runtime_error("Failed to retrieve compiled module: " + std::string(diagnostics ? diagnostics : "Unknown error"));
	}

	// Handle any diagnostics
	auto diagnostics = compileRequest->getNative()->getDiagnosticOutput();
	if (diagnostics && strlen(diagnostics) > 0)
	{
		std::cout << "Module compilation diagnostics:\n" << diagnostics << std::endl;
	}

	m_slangModule = slangModule;
}

const char* Native::ModuleCLI::getName()
{
	if (!m_slangModule)
		return nullptr;
	return m_slangModule->getName();
}

slang::IComponentType* Native::ModuleCLI::getProgramComponent()
{
	return m_programComponent;
}

unsigned int Native::ModuleCLI::getEntryPointCount()
{
	if (!m_slangModule)
		return 0;
	return m_slangModule->getDefinedEntryPointCount();
}

Native::EntryPointCLI* Native::ModuleCLI::getEntryPointByIndex(unsigned index)
{
	return new EntryPointCLI(this, index);
}

Native::EntryPointCLI* Native::ModuleCLI::findEntryPointByName(const char* name)
{
	return new EntryPointCLI(this, name);
}

Slang::ComPtr<slang::ISession> Native::ModuleCLI::getParent()
{
	return m_parent;
}

Slang::ComPtr<slang::IModule> Native::ModuleCLI::getNative()
{
	return m_slangModule;
}

std::unique_ptr<Native::ProgramCLI> Native::ModuleCLI::getProgram()
{
	return std::unique_ptr<ProgramCLI>(new ProgramCLI(this)); // Return a copy
}