using Slang.Sdk.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Slang.Sdk.Interop
{
    internal partial class StrongInterop
    {
        internal unsafe class GlobalSession
        {
            /// <summary>
            /// Creates a module with strongly-typed handle.
            /// </summary>
            internal static void EnableGlsl(out string? error)
            {
                char* pError = null;

                SlangNativeInterop.GlobalSession_SetEnableGlsl(true, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);

                SlangNativeInterop.FreeChar(&pError);
            }

            internal static int FindProfile(string name, out string? error)
            {
                char* pError = null;
                char* pName = (char*)Utf8StringMarshaller.ConvertToUnmanaged(name);

                try
                {
                    int result = SlangNativeInterop.GlobalSession_FindProfile(pName, &pError);
                    error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                    return result;
                }
                finally
                {
                    Utf8StringMarshaller.Free((byte*)pName);
                    SlangNativeInterop.FreeChar(&pError);
                }
            }

            internal static int FindCapability(string name, out string? error)
            {
                char* pError = null;
                char* pName = (char*)Utf8StringMarshaller.ConvertToUnmanaged(name);

                try
                {
                    int result = SlangNativeInterop.GlobalSession_FindCapability(pName, &pError);
                    error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                    return result;
                }
                finally
                {
                    Utf8StringMarshaller.Free((byte*)pName);
                    SlangNativeInterop.FreeChar(&pError);
                }
            }

            internal static SlangResult CheckCompileTargetSupport(int target, out string? error)
            {
                char* pError = null;
                SlangResult result = SlangNativeInterop.GlobalSession_CheckCompileTargetSupport(target, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            internal static SlangResult CheckPassThroughSupport(int passThrough, out string? error)
            {
                char* pError = null;
                SlangResult result = SlangNativeInterop.GlobalSession_CheckPassThroughSupport(passThrough, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }
        }
    }
}
