using Slang.Sdk.Collections;
using Slang.Sdk.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Slang.Sdk.Interop.Target;

namespace Slang.Sdk
{
    public partial class Session
    {
        public class Builder
        {
            public List<CompilerOption> CompilerOptions { get; } = new();
            public List<PreprocessorMacro> PreprocessorMacros { get; } = new();
            public List<Target> Targets { get; } = new();
            public List<string> SearchPaths { get; } = new();

            public Builder AddCompilerOption(CompilerOption.Name name, CompilerOption.Value value)
            {
                CompilerOptions.Add(new CompilerOption(name, value));
                return this;
            }

            public Builder AddPreprocessorMacro(string name, string value)
            {
                PreprocessorMacros.Add(new PreprocessorMacro(name, value));
                return this;
            }

            public Builder AddTarget(Target target)
            {
                // Not neccessary, can be safely deleted
                //if (target.target == Target.CompileTarget.SpirV)
                //    CheckAndPrepareSpirv(target);

                Targets.Add(target);
                return this;
            }

            public Builder AddSearchPath(string absolutePath)
            {
                SearchPaths.Add(absolutePath);
                return this;
            }

            public Session Create()
            {
                return new Session(CompilerOptions.ToArray(), PreprocessorMacros.ToArray(), Targets.ToArray(), SearchPaths.ToArray());
            }
        }
    }
}
