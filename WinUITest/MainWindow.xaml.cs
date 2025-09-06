using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Slang.Sdk;
using Slang.Sdk.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUITest
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            SimpleCompileTest();
        }

        public static void SimpleCompileTest()
        {
            try
            {
                // Add diagnostic information
                var processArch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
                var osArch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
                var runtimeDir = Slang.Runtime.SlangNative_Directory;

                System.Diagnostics.Debug.WriteLine($"ProcessArchitecture: {processArch}");
                System.Diagnostics.Debug.WriteLine($"OSArchitecture: {osArch}");
                System.Diagnostics.Debug.WriteLine($"SlangNative_Directory: {runtimeDir}");
                System.Diagnostics.Debug.WriteLine($"SlangNative.dll exists: {System.IO.File.Exists(System.IO.Path.Combine(runtimeDir, "SlangNative.dll"))}");

                // Verify all Slang dependencies are present and can be loaded
                var dependencies = new[] { "SlangNative.dll", "slang.dll", "slang-glslang.dll", "slang-glsl-module.dll", "slang-llvm.dll", "slang-rt.dll", "gfx.dll" };
                foreach (var dep in dependencies)
                {
                    var depPath = System.IO.Path.Combine(runtimeDir, dep);
                    var exists = System.IO.File.Exists(depPath);
                    System.Diagnostics.Debug.WriteLine($"Dependency {dep}: exists={exists}, path={depPath}");

                    if (exists)
                    {
                        var fileInfo = new System.IO.FileInfo(depPath);
                        System.Diagnostics.Debug.WriteLine($"  Size: {fileInfo.Length} bytes");
                    }
                }

                // Create a session with compiler options and search paths
                Session.Builder builder = new Session.Builder()
                    .AddCompilerOption(CompilerOption.Name.WarningsAsErrors, new CompilerOption.Value(CompilerOption.Value.Kind.Int, 0, 0, "all", null))
                    .AddCompilerOption(CompilerOption.Name.Obfuscate, new CompilerOption.Value(CompilerOption.Value.Kind.Int, 1, 0, null, null))
                    .AddPreprocessorMacro("LIGHTING_SCALER", "12")
                    .AddTarget(Targets.Custom(Target.CompileTarget.Dxil, "sm_6_0"))
                    .AddSearchPath($@"{AppDomain.CurrentDomain.BaseDirectory}Tests\Shaders\"); // Fixed: AverageColor.slang is copied to output directory root

                System.Diagnostics.Debug.WriteLine("About to create Slang session...");

                // Create the session
                Session session = builder.Create();

                System.Diagnostics.Debug.WriteLine("Slang session created successfully!");

                // Load the module from the specified file
                Module module = session.LoadModule("AverageColor", $@"{AppDomain.CurrentDomain.BaseDirectory}Tests\Shaders\AverageColor.slang");

                // Get the shader program from the module
                Slang.Sdk.Program program = module.Program;

                // Access the shader program from the module
                ShaderReflection reflection = module.Program.Targets[Targets.Custom(Target.CompileTarget.Dxil, "sm_6_0")].GetReflection();

                // Compile the shader program
                var compileResult = program.Targets[Targets.Custom(Target.CompileTarget.Dxil, "sm_6_0")].Compile();

                // Print the generated source code length
                switch (compileResult.CompileOutputType)
                {
                    case Target.CompileOutputType.SourceCode:
                        Console.WriteLine($"Source code length: {compileResult.SourceCode!.Length}");
                        System.Diagnostics.Debug.WriteLine($"Source code length: {compileResult.SourceCode!.Length}");
                        break;
                    case Target.CompileOutputType.ByteCode:
                        Console.WriteLine($"Bytecode length: {compileResult.ByteCode!.Length}");
                        Console.WriteLine($"Bytecode Preview: {BitConverter.ToString(compileResult.ByteCode.Take(32).ToArray()).Replace("-", " ")}");
                        System.Diagnostics.Debug.WriteLine($"Bytecode length: {compileResult.ByteCode!.Length}");
                        break;
                }

                System.Diagnostics.Debug.WriteLine("Slang compilation completed successfully!");
            }
            catch (BadImageFormatException ex)
            {
                System.Diagnostics.Debug.WriteLine($"BadImageFormatException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"This indicates an architecture mismatch between ARM64 and x64 DLLs");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SimpleCompileTest failed: {ex}");
                throw;
            }
        }
    }
}
