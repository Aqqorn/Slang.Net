
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Slang.Sdk
{
    // This is unnecessary for now since glslang.dll is included with the Slang SDK
    //public static class SpirvTools
    //{
    //    public static bool IsInstalled => CLI_Utilities.IsToolAvailable("spirv-opt");
    //
    //    public static void CopyTools()
    //    {
    //        if (!IsInstalled)
    //            throw new Exception("The SpirV tools is not installed. It must be installed in order to add a SpirV target. You can get it from the Vulkan SDK. Download it here: https://www.lunarg.com/vulkan-sdk/");
    //
    //        var optPath = CLI_Utilities.LocateExecutable("spirv-opt.exe");
    //        var valPath = CLI_Utilities.LocateExecutable("spirv-val.exe");
    //        var disPath = CLI_Utilities.LocateExecutable("spirv-dis.exe");
    //        var asPath = CLI_Utilities.LocateExecutable("spirv-as.exe");
    //
    //        if (optPath == null || valPath == null || disPath == null || asPath == null)
    //            throw new Exception("SpirV tools are not found in the PATH. Please ensure that the Vulkan SDK is installed and the tools are available in the system PATH.");
    //
    //        File.Copy(optPath, Path.Combine(Runtime.SlangNative_Directory, Path.GetFileName(optPath)), true);
    //        File.Copy(valPath, Path.Combine(Runtime.SlangNative_Directory, Path.GetFileName(valPath)), true);
    //        File.Copy(disPath, Path.Combine(Runtime.SlangNative_Directory, Path.GetFileName(disPath)), true);
    //        File.Copy(asPath, Path.Combine(Runtime.SlangNative_Directory, Path.GetFileName(asPath)), true);
    //    }
    //}
    public static class DXC_Tools
    {
        public static bool IsLoaded
        {
            get
            {
                // Check if the directory exists and contains the necessary files
                return Directory.Exists(AppDomain.CurrentDomain.BaseDirectory) &&
                       File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dxc.exe")) &&
                       File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dxcompiler.dll")) &&
                       File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dxil.dll")) &&
                       File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dxv.exe"));
            }
        }

        public static bool TryLoad()
        {
            if (IsLoaded)
                return true;
            if (CLI_Utilities.TryGetTool("dxc", out string toolPath))
            {
                var result = CLI_Utilities.CopyAllFiles(Path.GetDirectoryName(toolPath)!, AppDomain.CurrentDomain.BaseDirectory);
                return result && IsLoaded;
            }
            return false;
        }
    }

    internal static class CLI_Utilities
    {
        internal static bool TryGetTool(string toolName, out string toolPath)
        {
            // append exe if not already present
            if (!toolName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                toolName += ".exe";
            }

            toolPath = LocateExecutable(toolName) ?? string.Empty;

            if (toolPath == null)
            {
                return false;
            }

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = toolPath,
                        Arguments = "--version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        internal static string? LocateExecutable(string environmentVariable)
        {
            if (string.IsNullOrWhiteSpace(environmentVariable))
                throw new ArgumentException("Executable name must not be null or whitespace.", nameof(environmentVariable));

            string? pathEnv = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (pathEnv == null)
                return null;

            var result = pathEnv.Split(Path.PathSeparator).Select(p => Path.Combine(p, environmentVariable)).FirstOrDefault(File.Exists);
            return result;
        }

        public static bool CopyAllFiles(string sourceDir, string destDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                Console.Error.WriteLine("Source directory does not exist.");
                return false;
            }

            if (!Directory.Exists(destDir))
            {
                Console.Error.WriteLine("Destination directory does not exist.");
                return false;
            }

            try
            {
                foreach (var filePath in Directory.GetFiles(sourceDir))
                {
                    var fileName = Path.GetFileName(filePath);
                    var destPath = Path.Combine(destDir, fileName);
                    File.Copy(filePath, destPath, overwrite: true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error copying files: {ex.Message}");
                return false;
            }
        }
    }
}