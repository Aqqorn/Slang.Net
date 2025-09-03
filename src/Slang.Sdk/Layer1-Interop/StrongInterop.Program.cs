using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Slang.Sdk.Interop
{
    internal unsafe partial class StrongInterop
    {
        internal class Program
        {
            /// <summary>
            /// Creates a program with strongly-typed handle.
            /// </summary>
            internal static ProgramHandle Create(ModuleHandle parentModule, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.Program_Create(parentModule, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new ProgramHandle(handle);
            }

            /// <summary>
            /// Releases a program with strongly-typed handle.
            /// </summary>
            internal static void Release(ProgramHandle program, out string? error)
            {
                char* pError = null;
                SlangNativeInterop.Program_Release(program.Handle, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
            }

            /// <summary>
            /// Compiles a program using the specified target and returns the result with strongly-typed input.
            /// </summary>
            internal static SlangResult CompileTarget(
                ProgramHandle program,
                uint targetIndex,
                out byte[]? output,
                out string? error)
            {
                char* pError = null;
                void* pOutput = null;
                int outputSize = -1;

                var result = SlangNativeInterop.Program_CompileTarget(program.Handle, targetIndex, &pOutput, &outputSize, &pError);
                
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);

                if (outputSize != -1)
                {
                    output = new byte[outputSize];
                    Marshal.Copy((nint)pOutput, output, 0, outputSize);
                    // Free the native buffer allocated by the native layer
                    SlangNativeInterop.FreePointer(&pOutput);
                }
                else
                    output = null;

                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Compiles a program using the specified entry point and returns the result with strongly-typed input.
            /// </summary>
            internal static SlangResult CompileEntryPoint(
                ProgramHandle program,
                uint entryPointIndex,
                uint targetIndex,
                out byte[]? output,
                out string? error)
            {
                char* pError = null;
                void* pOutput = null;
                int outputSize = -1;

                var result = SlangNativeInterop.Program_CompileEntryPoint(program.Handle, entryPointIndex, targetIndex, &pOutput, &outputSize, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);

                if (outputSize != -1)
                {
                    output = new byte[outputSize];
                    Marshal.Copy((nint)pOutput, output, 0, outputSize);
                    // Free the native buffer allocated by the native layer
                    SlangNativeInterop.FreePointer(&pOutput);
                }
                else
                    output = null;

                SlangNativeInterop.FreeChar(&pError);

                return result;
            }

            /// <summary>
            /// Gets program reflection with strongly-typed handle.
            /// </summary>
            internal static ShaderReflectionHandle GetProgramReflection(
                ProgramHandle program,
                uint targetIndex,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.Program_GetProgramReflection(program.Handle, targetIndex, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new ShaderReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the native program handle.
            /// </summary>
            internal static nint GetNative(ProgramHandle program, out string? error)
            {
                char* pError = null;
                nint result = SlangNativeInterop.Program_GetNative(program.Handle, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }
        }
    }
}
