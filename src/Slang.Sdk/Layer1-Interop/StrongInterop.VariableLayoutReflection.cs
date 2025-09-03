using System.Runtime.InteropServices.Marshalling;

namespace Slang.Sdk.Interop
{
    internal unsafe partial class StrongInterop
    {
        internal class VariableLayoutReflection
        {
            /// <summary>
            /// Releases a variable layout reflection with strongly-typed handle.
            /// </summary>
            internal static void Release(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                SlangNativeInterop.VariableLayoutReflection_Release(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
            }

            /// <summary>
            /// Gets the variable.
            /// </summary>
            internal static VariableReflectionHandle GetVariable(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.VariableLayoutReflection_GetVariable(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new VariableReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the variable layout name.
            /// </summary>
            internal static string GetName(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                char* pName = SlangNativeInterop.VariableLayoutReflection_GetName(variableLayoutReflection, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                string result = Utf8StringMarshaller.ConvertToManaged((byte*)pName);

                SlangNativeInterop.FreeChar(&pError);

                return result;
            }

            /// <summary>
            /// Finds a modifier.
            /// </summary>
            internal static ModifierReflectionHandle FindModifier(
                VariableLayoutReflectionHandle variableLayoutReflection,
                int modifierId,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.VariableLayoutReflection_FindModifier(variableLayoutReflection, modifierId, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new ModifierReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the type layout.
            /// </summary>
            internal static TypeLayoutReflectionHandle GetTypeLayout(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.VariableLayoutReflection_GetTypeLayout(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new TypeLayoutReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the category.
            /// </summary>
            internal static int GetCategory(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.VariableLayoutReflection_GetCategory(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the category count.
            /// </summary>
            internal static uint GetCategoryCount(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.VariableLayoutReflection_GetCategoryCount(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets a category by index.
            /// </summary>
            internal static int GetCategoryByIndex(
                VariableLayoutReflectionHandle variableLayoutReflection,
                uint index,
                out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.VariableLayoutReflection_GetCategoryByIndex(variableLayoutReflection, index, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the offset for a specific category.
            /// </summary>
            internal static nuint GetOffset(
                VariableLayoutReflectionHandle variableLayoutReflection,
                int category,
                out string? error)
            {
                char* pError = null;
                nuint result = SlangNativeInterop.VariableLayoutReflection_GetOffset(variableLayoutReflection, category, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the type.
            /// </summary>
            internal static TypeReflectionHandle GetType(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.VariableLayoutReflection_GetType(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new TypeReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the binding index.
            /// </summary>
            internal static uint GetBindingIndex(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.VariableLayoutReflection_GetBindingIndex(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the binding space.
            /// </summary>
            internal static uint GetBindingSpace(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.VariableLayoutReflection_GetBindingSpace(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the space for a specific category.
            /// </summary>
            internal static VariableLayoutReflectionHandle GetSpace(
                VariableLayoutReflectionHandle variableLayoutReflection,
                int category,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.VariableLayoutReflection_GetSpace(variableLayoutReflection, category, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new VariableLayoutReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the image format.
            /// </summary>
            internal static int GetImageFormat(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.VariableLayoutReflection_GetImageFormat(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the semantic name.
            /// </summary>
            internal static string GetSemanticName(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                char* pName = SlangNativeInterop.VariableLayoutReflection_GetSemanticName(variableLayoutReflection, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                string result = Utf8StringMarshaller.ConvertToManaged((byte*)pName);

                SlangNativeInterop.FreeChar(&pError);

                return result;
            }

            /// <summary>
            /// Gets the semantic index.
            /// </summary>
            internal static uint GetSemanticIndex(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.VariableLayoutReflection_GetSemanticIndex(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the stage.
            /// </summary>
            internal static Stage GetStage(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.VariableLayoutReflection_GetStage(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return (Stage)result;
            }

            /// <summary>
            /// Gets the pending data layout.
            /// </summary>
            internal static VariableLayoutReflectionHandle GetPendingDataLayout(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.VariableLayoutReflection_GetPendingDataLayout(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new VariableLayoutReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the native variable layout reflection handle.
            /// </summary>
            internal static nint GetNative(VariableLayoutReflectionHandle variableLayoutReflection, out string? error)
            {
                char* pError = null;
                nint result = SlangNativeInterop.VariableLayoutReflection_GetNative(variableLayoutReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }
        }
    }
}