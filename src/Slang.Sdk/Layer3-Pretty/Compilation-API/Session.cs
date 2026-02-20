using Slang.Sdk.Collections;
using Slang.Sdk.Interop;
using System.Reflection.Metadata;

namespace Slang.Sdk
{
    public partial class Session
        : IComposition<Target>,
        IComposition<Module>,
        INamedComposition<Module>,
        IDisposable
    {
        #region Definition
        internal Binding.Session Binding { get; }

        internal Session(CompilerOption[] options, PreprocessorMacro[] macros, Target[] models, string[] searchPaths)
        {
            Binding = new Binding.Session(options, macros, models, searchPaths);
        }
        #endregion

        // Maybe make this its own class (temp)
        #region Global
        public static bool GlslEnabled
        {
            set
            {
                if (value)
                    global::Slang.Sdk.Binding.Session.EnableGlsl();
                else
                    throw new NotImplementedException("Disabling GLSL is not currently supported.");
            }
        }

        public static int FindProfile(string profileName)
            => global::Slang.Sdk.Binding.Session.FindProfile(profileName);

        public static int FindCapability(string capabilityName)
            => global::Slang.Sdk.Binding.Session.FindCapability(capabilityName);

        public static SlangResult CheckCompileTargetSupport(Target.CompileTarget target)
            => global::Slang.Sdk.Binding.Session.CheckCompileTargetSupport(target);

        public static SlangResult CheckPassThroughSupport(PassThrough passThrough)
            => global::Slang.Sdk.Binding.Session.CheckPassThroughSupport(passThrough);
        #endregion

        #region Pretty
        public Module ImportModule(string moduleName)
        {
            // Allows the users to either include or exclude the .slang extension
            moduleName = Path.GetFileNameWithoutExtension(moduleName);

            return new Module(this, moduleName);
        }
        public Module FindModule(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
                throw new ArgumentException("Module name cannot be null or empty.", nameof(moduleName));

            if (!Binding.SearchPaths.Any())
                throw new InvalidOperationException("No search paths configured for this session. Add at least one path with Session.Builder.AddSearchPath(...).");

            // Allows the users to either include or exclude the .slang extension
            moduleName = Path.GetFileNameWithoutExtension(moduleName);

            string? modulePath = null;
            string? moduleSource = null;
            foreach (var searchPath in Binding.SearchPaths)
            {
                var filePath = Path.Combine(searchPath, $"{moduleName}.slang");
                if (File.Exists(filePath))
                {
                    modulePath = filePath;
                    moduleSource = File.ReadAllText(filePath);
                    break;
                }
            }
            if (modulePath is null || moduleSource is null)
                throw new FileNotFoundException($"{moduleName} could not be found in any of the parent session's search paths.");

            return new Module(this, moduleName, modulePath, moduleSource);
        }

        public Module LoadModule(string modulePath)
        {
            if (string.IsNullOrWhiteSpace(modulePath))
                throw new ArgumentException("Module name cannot be null or empty.", nameof(modulePath));

            string? moduleName = Path.GetFileName(modulePath);
            string? moduleSource = File.Exists(modulePath) ? File.ReadAllText(modulePath) : null;

            if (moduleName is null)
                throw new FormatException($"The specified path is not correctly formatted: {modulePath}");
            if (moduleSource is null)
                throw new FileNotFoundException($"The specified slang file was not found: {modulePath}", modulePath);

            var result = new Module(this, moduleName, modulePath, moduleSource);
            _ModulesList.Add(result);
            return result;
        }

        public Module LoadModule(string moduleName, string modulePath)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
                throw new ArgumentException("Module name cannot be null or empty.", nameof(moduleName));

            if (string.IsNullOrWhiteSpace(modulePath))
                throw new ArgumentException("Module path cannot be null or empty.", nameof(modulePath));

            if (!Binding.SearchPaths.Any())
                throw new InvalidOperationException("No search paths configured for this session. Add at least one path with Session.Builder.AddSearchPath(...).");

            // Allows the users to either include or exclude the .slang extension
            moduleName = Path.GetFileNameWithoutExtension(moduleName);

            string? moduleSource = null;
            if (File.Exists(modulePath))
                moduleSource = File.ReadAllText(modulePath);

            if (moduleSource is null)
                throw new FileNotFoundException($"The specified slang file was not found: {modulePath}", modulePath);

            var result = new Module(this, moduleName, modulePath, moduleSource);
            _ModulesList.Add(result);
            return result;
        }

        public Module LoadModuleFromSourceString(string moduleName, string sourceText, string? modulePath = null)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
                throw new ArgumentException("Module name cannot be null or empty.", nameof(moduleName));
            if (string.IsNullOrWhiteSpace(sourceText))
                throw new ArgumentException("Source text cannot be null or empty.", nameof(sourceText));

            // Keep extension-less import semantics while still feeding a stable virtual path to Slang diagnostics.
            moduleName = Path.GetFileNameWithoutExtension(moduleName);
            modulePath ??= $"{moduleName}.slang";

            var bindingModule = Binding.LoadModuleFromSourceString(moduleName, modulePath, sourceText);
            var result = new Module(this, bindingModule, moduleName);
            _ModulesList.Add(result);
            return result;
        }

        SlangCollection<Target>? _Targets;
        public SlangCollection<Target> Targets => _Targets ??= new SlangCollection<Target>(this);

        SlangNamedCollectionary<Module>? _Module;
        public SlangNamedCollectionary<Module> Modules => _Module ??= new (this, this);
        #endregion

        #region Composition

        // Targets
        Target IComposition<Target>.GetByIndex(uint index)
        {
            return Binding.Targets.ElementAt((int)index);
        }
        uint IComposition<Target>.Count => (uint)Binding.Targets.Count;

        // Modules
        List<Module> _ModulesList = new();
        uint IComposition<Module>.Count => (uint)_ModulesList.Count;
        Module IComposition<Module>.GetByIndex(uint index)
        {
            return _ModulesList[(int)index];
        }

        Module? INamedComposition<Module>.FindByName(string name)
        {
            return FindModule(name);
        }

        #endregion

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
                Binding?.Dispose();

                _disposed = true;
            }
        }
        #endregion

    }
}
