using System.Runtime.InteropServices.Marshalling;

namespace Slang.Sdk.Interop
{
    internal unsafe partial class StrongInterop
    {
        internal class TypeReflection
        {
            /// <summary>
            /// Releases a type reflection with strongly-typed handle.
            /// </summary>
            internal static void Release(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                SlangNativeInterop.TypeReflection_Release(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
            }

            /// <summary>
            /// Gets the type kind.
            /// </summary>
            internal static int GetKind(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.TypeReflection_GetKind(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the field count.
            /// </summary>
            internal static uint GetFieldCount(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.TypeReflection_GetFieldCount(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets a field by index.
            /// </summary>
            internal static VariableReflectionHandle GetFieldByIndex(
                TypeReflectionHandle typeReflection,
                uint index,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_GetFieldByIndex(typeReflection, index, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new VariableReflectionHandle(handle);
            }

            /// <summary>
            /// Checks if the type is an array.
            /// </summary>
            internal static bool IsArray(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                bool result = SlangNativeInterop.TypeReflection_IsArray(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Unwraps an array type.
            /// </summary>
            internal static TypeReflectionHandle UnwrapArray(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_UnwrapArray(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new TypeReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the element count.
            /// </summary>
            internal static nuint GetElementCount(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                nuint result = SlangNativeInterop.TypeReflection_GetElementCount(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the element type.
            /// </summary>
            internal static TypeReflectionHandle GetElementType(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_GetElementType(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new TypeReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the row count.
            /// </summary>
            internal static uint GetRowCount(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.TypeReflection_GetRowCount(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the column count.
            /// </summary>
            internal static uint GetColumnCount(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.TypeReflection_GetColumnCount(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the scalar type.
            /// </summary>
            internal static int GetScalarType(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.TypeReflection_GetScalarType(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the resource result type.
            /// </summary>
            internal static TypeReflectionHandle GetResourceResultType(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_GetResourceResultType(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new TypeReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the resource shape.
            /// </summary>
            internal static ResourceShape GetResourceShape(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                ResourceShape result = (ResourceShape)SlangNativeInterop.TypeReflection_GetResourceShape(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the resource access.
            /// </summary>
            internal static int GetResourceAccess(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                int result = SlangNativeInterop.TypeReflection_GetResourceAccess(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets the type name.
            /// </summary>
            internal static string GetName(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                char* pName = SlangNativeInterop.TypeReflection_GetName(typeReflection, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                string result = Utf8StringMarshaller.ConvertToManaged((byte*)pName);

                SlangNativeInterop.FreeChar(&pError);

                return result;
            }

            /// <summary>
            /// Gets the user attribute count.
            /// </summary>
            internal static uint GetUserAttributeCount(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                uint result = SlangNativeInterop.TypeReflection_GetUserAttributeCount(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }

            /// <summary>
            /// Gets a user attribute by index.
            /// </summary>
            internal static AttributeReflectionHandle GetUserAttributeByIndex(
                TypeReflectionHandle typeReflection,
                uint index,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_GetUserAttributeByIndex(typeReflection, index, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new AttributeReflectionHandle(handle);
            }

            /// <summary>
            /// Finds an attribute by name.
            /// </summary>
            internal static AttributeReflectionHandle FindAttributeByName(
                TypeReflectionHandle typeReflection,
                string name,
                out string? error)
            {
                char* pError = null;
                var unmanagedName = Utf8StringMarshaller.ConvertToUnmanaged(name);
                var handle = SlangNativeInterop.TypeReflection_FindAttributeByName(typeReflection, (char*)unmanagedName, &pError);

                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);

                Utf8StringMarshaller.Free(unmanagedName);
                SlangNativeInterop.FreeChar(&pError);

                return new AttributeReflectionHandle(handle);
            }

            /// <summary>
            /// Applies specializations to the type.
            /// </summary>
            internal static TypeReflectionHandle ApplySpecializations(
                TypeReflectionHandle typeReflection,
                GenericReflectionHandle genRef,
                out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_ApplySpecializations(typeReflection, genRef, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new TypeReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the generic container.
            /// </summary>
            internal static GenericReflectionHandle GetGenericContainer(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                var handle = SlangNativeInterop.TypeReflection_GetGenericContainer(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return new GenericReflectionHandle(handle);
            }

            /// <summary>
            /// Gets the native type reflection handle.
            /// </summary>
            internal static nint GetNative(TypeReflectionHandle typeReflection, out string? error)
            {
                char* pError = null;
                nint result = SlangNativeInterop.TypeReflection_GetNative(typeReflection, &pError);
                error = Utf8StringMarshaller.ConvertToManaged((byte*)pError);
                SlangNativeInterop.FreeChar(&pError);
                return result;
            }
        }
    }
}