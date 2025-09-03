using Slang.Sdk.Interop;
using static Slang.Sdk.Interop.StrongInterop;
using static Slang.Sdk.Interop.Utilities;

namespace Slang.Sdk.Binding;

/// <summary>
/// Represents a Slang compilation session that manages modules and compilation.
/// </summary>
internal unsafe sealed class Session : CompilationBinding, IDisposable
{
    static object _lock = new object();

    internal override SessionHandle Handle { get; }
    internal override SessionHandle NativeHandle => new(StrongInterop.Session.GetNative(Handle, out var _));


    internal IReadOnlyCollection<Target> Targets { get; }
    internal string[] SearchPaths { get; }

    /// <summary>
    /// Creates a new Slang session with the specified configuration.
    /// </summary>
    /// <param name="configuration">The session configuration.</param>
    /// <exception cref="SlangException">Thrown if session creation fails.</exception>
    internal Session(CompilerOption[] options, PreprocessorMacro[] macros, Target[] models, string[] searchPaths)
    {
        // Checks is there any DXIL target, if so, find and load dxil.dll
        if (models.Where(item => item.target == Target.CompileTarget.Dxil).Any())
        {
            lock(_lock)
            {
                if (!DXC_Tools.TryLoad())
                    throw new SlangException(
                        SlangResult.Fail,
                         "DirectXShaderCompiler is not installed: \n" +
                                "1. Please download and install it from here: https://github.com/microsoft/DirectXShaderCompiler/releases\n" +
                                "2. Make sure the installation path (...\\dxc<version>.zip\\bin\\<architecture>\\) is added to your system's PATH environment variable.");
            }
        }

        Targets = models;
        SearchPaths = searchPaths;

        fixed (CompilerOption* optionsPtr = options)
        fixed (PreprocessorMacro* macrosPtr = macros)
        fixed (Target* modelsPtr = models)
        {
            Handle = StrongInterop.Session.Create(
                optionsPtr, options.Length,
                macrosPtr, macros.Length,
                modelsPtr, models.Length,
                searchPaths, searchPaths.Length,
                out var error);

            if (Handle.IsInvalid)
                throw new SlangException(SlangResult.Fail, $"Failed to create Slang session: {error ?? "<No error was returned from Slang>"}");
        }
    }

    internal uint GetModuleCount()
    {
        string? error = null;
        return Call(() => StrongInterop.Session.GetModuleCount(Handle, out error), () => error);
    }

    internal Module GetModuleByIndex(uint index)
    {
        string? error = null;
        return new Module(this, Call(() => StrongInterop.Session.GetModuleByIndex(Handle, index, out error), () => error));
    }

    internal static void EnableGlsl()
    {
        string? error = null;
        StrongInterop.GlobalSession.EnableGlsl(out error);
        if (error != null)
            throw new SlangException(SlangResult.Fail, $"Failed to enable GLSL support: {error}");
    }

    #region Disposable
    private bool _disposed = false; // To detect redundant calls

    ~Session()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Prevent Finalize from being called
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Release managed resources here
            }

            // Release unmanaged resources here
            Handle?.Dispose();

            _disposed = true;
        }
    }
    #endregion
}