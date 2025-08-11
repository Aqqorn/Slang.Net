#include "SessionCLI.h"
#include "ModuleCLI.h"
#include <vector>
#include <iostream>
#include <stdexcept>

// Static member definitions
Slang::ComPtr<slang::IGlobalSession> Native::SessionCLI::s_context = nullptr;
bool Native::SessionCLI::s_isEnableGlsl = false;

// Constructor with parameters implementation
Native::SessionCLI::SessionCLI(
    CompilerOptionCLI* options, int optionsLength,
    PreprocessorMacroDescCLI* macros, int macrosLength, 
    TargetCLI* models, int modelsLength,
    char** searchPaths, int searchPathsLength)
{
    slang::SessionDesc sessionDesc = {};

    // Set targets
    std::vector<slang::TargetDesc> targetDescs;
    targetDescs.reserve(modelsLength);

    for (int i = 0; i < modelsLength; i++)
    {
        TargetCLI model = models[i];

        slang::TargetDesc targetDesc = {};
        targetDesc.format = (SlangCompileTarget)model.getTarget();
        targetDesc.profile = GetGlobalSession()->findProfile(model.getProfile());
        
        // Add to our collection
        targetDescs.push_back(targetDesc);
    }

	// Sets the target descriptions if we have any
    if (!targetDescs.empty())
    {
        sessionDesc.targets = targetDescs.data();
        sessionDesc.targetCount = static_cast<int>(targetDescs.size());
    }

	// Set compiler options
    std::vector<slang::CompilerOptionEntry> compilerOptions;
    compilerOptions.reserve(optionsLength);
    
    for (int i = 0; i < optionsLength; i++)
    {
        CompilerOptionCLI option = options[i];
		CompilerOptionValueCLI value = option.getValue();
    
        slang::CompilerOptionEntry entry = 
        {
                (slang::CompilerOptionName)option.getName(),
                {(slang::CompilerOptionValueKind)value.kind, value.intValue0, value.intValue1, value.stringValue0, value.stringValue1 }
        };
    
        // Add to our collection
        compilerOptions.push_back(entry);
    }
    
	// Sets the compiler options if we have any
    if (!compilerOptions.empty())
    {
        sessionDesc.compilerOptionEntries = compilerOptions.data();
        sessionDesc.compilerOptionEntryCount = compilerOptions.size();
    }
    
    // Set PreprocessorMacroDesc
    std::vector<slang::PreprocessorMacroDesc> preprocessorMacroDesc;
    preprocessorMacroDesc.reserve(macrosLength);
    
    for (int i = 0; i < macrosLength; i++)
    {
        slang::PreprocessorMacroDesc macroDesc = { macros[i].getName(), macros[i].getValue() };
    
        // Add to our collection
        preprocessorMacroDesc.push_back(macroDesc);
    }
    
	// Set the preprocessor macros if we have any
    if (!preprocessorMacroDesc.empty())
    {
        sessionDesc.preprocessorMacros = preprocessorMacroDesc.data();
        sessionDesc.preprocessorMacroCount = preprocessorMacroDesc.size();
    }

	// Set the search paths if we have any
    if (searchPathsLength != 0)
    {
        sessionDesc.searchPaths = searchPaths;
        sessionDesc.searchPathCount = searchPathsLength;
    }

    SlangResult createResult = GetGlobalSession()->createSession(sessionDesc, m_session.writeRef());
    
    if (SLANG_FAILED(createResult))
        throw std::runtime_error("Failed to create Slang session. Error code: " + std::to_string(createResult) + 
                                ". Check compiler options, targets, and search paths.");
}

// Destructor implementation
Native::SessionCLI::~SessionCLI()
{   
    // ComPtr will automatically release the session
}

// This is only called when a session is created, GLSL has to be enabled before the first session is created.
Slang::ComPtr<slang::IGlobalSession> Native::SessionCLI::GetGlobalSession()
{
    if (s_context == nullptr)
    {
        SlangResult result;
        if (s_isEnableGlsl)
        {
            SlangGlobalSessionDesc desc = {};
            desc.enableGLSL = true;
            desc.apiVersion = SLANG_API_VERSION;
            result = slang_createGlobalSession2(&desc, s_context.writeRef());
		}
        else
        {
            result = slang_createGlobalSession(SLANG_API_VERSION, s_context.writeRef());
		}
        
        if (SLANG_FAILED(result))
            throw std::runtime_error("Failed to create Slang global session. Error code: " + std::to_string(result));
    }

    return s_context;
}

unsigned int Native::SessionCLI::getModuleCount()
{
    if (!m_session)
        return 0;
    return m_session->getLoadedModuleCount();
}

Native::ModuleCLI* Native::SessionCLI::getModuleByIndex(unsigned index)
{
    return new Native::ModuleCLI(this, m_session->getLoadedModule(index));
}

Slang::ComPtr<slang::ISession> Native::SessionCLI::getNative()
{
    return m_session;
}