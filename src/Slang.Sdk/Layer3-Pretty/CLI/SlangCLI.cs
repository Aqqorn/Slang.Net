using System.Diagnostics;
using System.Text;

namespace Slang.Sdk
{
    public static class CLI
    {
        private static string _WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string WorkingDirectory 
        {
            get => _WorkingDirectory;
            set => _WorkingDirectory = Directory.Exists(value) ? value : throw new DirectoryNotFoundException($"Working directory does not exist: {value}");
        }

        public static CLI_Results slangc(
            string? target = null,
            string? profile = null,
            string? entry = null,
            string? stage = null,
            string? outputPath = null,
            string? moduleName = null,
            string[]? includePaths = null,
            Dictionary<string, string>? defines = null,
            string? std = null,
            bool warningsAsErrors = false,
            string[]? disableWarnings = null,
            string? optimizationLevel = null,
            bool emitIR = false,
            bool debugInfo = false,
            string? reflectionJsonPath = null,
            string[]? inputFiles = null)
        {
            SlangC_Options args = new SlangC_Options()
            {
                Target = target,
                Profile = profile,
                Entry = entry,
                Stage = stage,
                OutputPath = outputPath,
                ModuleName = moduleName,
                IncludePaths = includePaths is null ? new() : includePaths.ToList(),
                Defines = defines,
                Std = std,
                WarningsAsErrors = warningsAsErrors,
                DisableWarnings = disableWarnings is null ? new() : disableWarnings.ToList(),
                OptimizationLevel = optimizationLevel,
                EmitIR = emitIR,
                DebugInfo = debugInfo,
                ReflectionJsonPath = reflectionJsonPath,
                InputFiles = inputFiles is null ? new() : inputFiles.ToList(),
            };

            return slangc(args);
        }

        public static CLI_Results slangc(SlangC_Options args)
        {
            return InvokeSlangc(args.ToString());
        }

        public static CLI_Results slangc(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                throw new ArgumentException("Arguments cannot be null or empty.", nameof(args));
            return InvokeSlangc(args);
        }

        private static CLI_Results InvokeSlangc(string args)
        {
            var slangcPath = Path.Combine(Runtime.CLI_Directory, "slangc.exe");
            
            // Check if slangc.exe exists before trying to run it
            if (!File.Exists(slangcPath))
            {
                return new CLI_Results
                {
                    ExitCode = -1,
                    StdOut = "",
                    StdErr = $"slangc.exe not found at expected path: {slangcPath}\n" +
                            $"Runtime directory: {Runtime.CLI_Directory}\n" +
                            $"Directory exists: {Directory.Exists(Runtime.CLI_Directory)}\n"
                };
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = slangcPath,
                Arguments = args,
                WorkingDirectory = WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
            };

            using var process = new Process { StartInfo = startInfo };
            var result = new CLI_Results();

            var stdoutBuilder = new StringBuilder();
            var stderrBuilder = new StringBuilder();

            process.OutputDataReceived += (_, e) => { if (e.Data != null) stdoutBuilder.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) stderrBuilder.AppendLine(e.Data); };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                result.ExitCode = process.ExitCode;
                result.StdOut = stdoutBuilder.ToString();
                result.StdErr = stderrBuilder.ToString();
            }
            catch (Exception ex)
            {
                result.ExitCode = -1;
                result.StdOut = "";
                result.StdErr = $"Failed to start slangc.exe: {ex.Message}\n" +
                               $"Path: {slangcPath}\n" +
                               $"Working Directory: {WorkingDirectory}";
            }

            return result;
        }

        // TODO: Implement Async feature in later version
    }
}
