using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Slang.Sdk.Interop
{
    internal unsafe partial class StrongInterop
    {
        internal class EntryPoint
        {
            /// <summary>
            /// Creates an entry point with strongly-typed handle.
            /// </summary>
            internal static EntryPointHandle Create(
                ModuleHandle parentModule,
                uint entryPointIndex,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.EntryPoint_Create(parentModule, entryPointIndex, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new EntryPointHandle(handle);
            }

            /// <summary>
            /// Creates an entry point by name with strongly-typed handle.
            /// </summary>
            internal static EntryPointHandle CreateByName(
                ModuleHandle parentModule,
                string entryPointName,
                out string? error)
            {
                char* pError = null;
                var unmanagedName = Utf8StringMarshaller.ConvertToUnmanaged(entryPointName);
                var handle = SlangNativeInterop.EntryPoint_CreateByName(parentModule, (char*)unmanagedName, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);

                Utf8StringMarshaller.Free(unmanagedName);
                SlangNativeInterop.FreeChar(&pError);

                return new EntryPointHandle(handle);
            }

            /// <summary>
            /// Gets the entry point index.
            /// </summary>
            internal static int GetIndex(EntryPointHandle entryPoint, out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.EntryPoint_GetIndex(entryPoint, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the entry point name.
            /// </summary>
            internal static string? GetName(EntryPointHandle entryPoint, out string? error)
            {
                char* pError = null;
                char* pName = SlangNativeInterop.EntryPoint_GetName(entryPoint, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                string? result = Utf8StringMarshaller.ConvertToManaged((byte*)pName);

                SlangNativeInterop.FreeChar(&pError);

                return result;
            }

            /// <summary>
            /// Compiles an entry point.
            /// </summary>
            internal static SlangResult Compile(
                EntryPointHandle entryPoint,
                int targetIndex,
                out byte[]? output,
                out string? error)
            {
                char* pError = null;
                void* pOutput = null;
                int outputSize = -1;

                var result = SlangNativeInterop.EntryPoint_Compile(entryPoint, targetIndex, &pOutput, &outputSize, &pError);

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
            /// Gets the native entry point handle.
            /// </summary>
            internal static nint GetNative(EntryPointHandle entryPoint, out string? error)
            {
                char* pError = null;
                nint result = SlangNativeInterop.EntryPoint_GetNative(entryPoint, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Releases an entry point with strongly-typed handle.
            /// </summary>
            internal static void Release(EntryPointHandle entryPoint, out string? error)
            {
                char* pError = null;
                SlangNativeInterop.EntryPoint_Release(entryPoint, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
            }
        }
    }
}
