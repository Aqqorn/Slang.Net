using System.Runtime.InteropServices;

namespace Slang
{
    public static class Runtime
    {
        public static string CLI_Directory
        {
            get
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                return ResolveRuntimeDirectory(appDirectory, GetRuntimeArchitecture(true));
            }
        }

        public static string SlangNative_Directory
        {
            get
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                return ResolveRuntimeDirectory(appDirectory, GetRuntimeArchitecture(false));
            }
        }

        public static string AsString()
        {
            return GetRuntime();
        }

        public static Architecture GetRuntimeArchitecture(bool isCLI)
        {
            return isCLI ? RuntimeInformation.OSArchitecture : RuntimeInformation.ProcessArchitecture;
        }

        private static string ResolveRuntimeDirectory(string appDirectory, Architecture preferredArchitecture)
        {
            string preferredFolder = GetRuntimeFolderName(preferredArchitecture);
            string preferredPath = Path.Combine(appDirectory, "runtimes", preferredFolder, "native");
            if (Directory.Exists(preferredPath))
                return preferredPath;

            // Fallback order: try process architecture, then common Windows defaults
            Architecture processArch = RuntimeInformation.ProcessArchitecture;
            string processFolder = GetRuntimeFolderName(processArch);
            string processPath = Path.Combine(appDirectory, "runtimes", processFolder, "native");
            if (Directory.Exists(processPath))
                return processPath;

            string[] fallbacks = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new[] { "win-x64", "win-arm64", "win-x86" }
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? new[] { "linux-x64", "linux-arm64" }
                    : new[] { "osx-x64", "osx-arm64" };

            foreach (var folder in fallbacks)
            {
                string candidate = Path.Combine(appDirectory, "runtimes", folder, "native");
                if (Directory.Exists(candidate))
                    return candidate;
            }

            // Project/local build layout may place native files directly in app base directory.
            if (File.Exists(Path.Combine(appDirectory, "slangc.exe")) || File.Exists(Path.Combine(appDirectory, "SlangNative.dll")))
                return appDirectory;

            return preferredPath;
        }

        private static string GetRuntimeFolderName(Architecture architecture)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return architecture switch
                {
                    Architecture.X64 => "win-x64",
                    Architecture.Arm64 => "win-arm64",
                    Architecture.X86 => "win-x86",
                    _ => "win-x64"
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return architecture switch
                {
                    Architecture.X64 => "linux-x64",
                    Architecture.Arm64 => "linux-arm64",
                    _ => "linux-x64"
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return architecture switch
                {
                    Architecture.X64 => "osx-x64",
                    Architecture.Arm64 => "osx-arm64",
                    _ => "osx-x64"
                };
            }

            return "win-x64";
        }

        private static string GetRuntime()
        {
            var architecture = RuntimeInformation.OSArchitecture;

            return architecture switch
            {
                Architecture.X64 => "x64",
                Architecture.Arm64 => "ARM64",
                Architecture.X86 => "x86",
                _ => "x64" // Default fallback
            };
        }

        // Delete this
        //private static Architecture GetEffectiveArchitecture()
        //{
        //    var reportedArchitecture = RuntimeInformation.ProcessArchitecture;

        //    // On Windows ARM64 systems, .NET may report ARM64 even when running x64 emulation
        //    // We need to check if the appropriate runtime files exist
        //    if (reportedArchitecture == Architecture.Arm64 && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //    {
        //        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        //        // Check if ARM64 runtime folder exists and contains required files
        //        string arm64RuntimePath = Path.Combine(appDirectory, "runtimes", "win-arm64", "native");
        //        string x64RuntimePath = Path.Combine(appDirectory, "runtimes", "win-x64", "native");

        //        bool arm64RuntimeExists = System.IO.Directory.Exists(arm64RuntimePath) && 
        //                                 File.Exists(Path.Combine(arm64RuntimePath, "slangc.exe"));
        //        bool x64RuntimeExists = System.IO.Directory.Exists(x64RuntimePath) && 
        //                               File.Exists(Path.Combine(x64RuntimePath, "slangc.exe"));

        //        // If ARM64 runtime doesn't exist but x64 does, prefer x64 (emulation scenario)
        //        if (!arm64RuntimeExists && x64RuntimeExists)
        //        {
        //            return Architecture.X64;
        //        }

        //        // Check if this is an emulated x64 process on ARM64 by examining environment
        //        // WOW64 processes will have PROCESSOR_ARCHITEW6432 set to ARM64
        //        string? processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
        //        if (processorArchitecture == "ARM64")
        //        {
        //            // This is likely an x64 process running under emulation
        //            // Prefer x64 runtime if available
        //            if (x64RuntimeExists)
        //            {
        //                return Architecture.X64;
        //            }
        //        }
        //    }

        //    return reportedArchitecture;
        //}
    }
}
