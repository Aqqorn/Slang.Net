using Slang.Sdk.Interop;

namespace Slang.Sdk.Binding
{
    internal unsafe class TypeReflection : Reflection, IEquatable<TypeReflection>
    {
        internal override Reflection? Parent { get; }
        internal override TypeReflectionHandle Handle { get; }
        internal override TypeReflectionHandle NativeHandle => new(StrongInterop.TypeReflection.GetNative(Handle, out var _));


        internal TypeReflection(Reflection parent, TypeReflectionHandle handle)
        {
            Parent = parent;
            Handle = handle;
        }

        internal TypeKind GetKind()
        {
            string? error = null;
            return (TypeKind)Call(() => StrongInterop.TypeReflection.GetKind(Handle, out error), () => error);
        }

        internal uint GetFieldCount()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetFieldCount(Handle, out error), () => error);
        }

        internal VariableReflection GetFieldByIndex(uint index)
        {
            string? error = null;
            return new VariableReflection(this, Call(() => StrongInterop.TypeReflection.GetFieldByIndex(Handle, index, out error), () => error));
        }

        internal bool IsArray()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.IsArray(Handle, out error), () => error);
        }

        internal TypeReflection UnwrapArray()
        {
            string? error = null;
            return new TypeReflection(this, Call(() => StrongInterop.TypeReflection.UnwrapArray(Handle, out error), () => error));
        }

        internal nuint GetElementCount()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetElementCount(Handle, out error), () => error);
        }

        internal TypeReflection GetElementType()
        {
            string? error = null;
            return new TypeReflection(this, Call(() => StrongInterop.TypeReflection.GetElementType(Handle, out error), () => error));
        }

        internal uint GetRowCount()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetRowCount(Handle, out error), () => error);
        }

        internal uint GetColumnCount()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetColumnCount(Handle, out error), () => error);
        }

        internal ScalarType GetScalarType()
        {
            string? error = null;
            return (ScalarType)Call(() => StrongInterop.TypeReflection.GetScalarType(Handle, out error), () => error);
        }

        internal TypeReflection GetResourceResultType()
        {
            string? error = null;
            return new TypeReflection(this, Call(() => StrongInterop.TypeReflection.GetResourceResultType(Handle, out error), () => error));
        }

        internal ResourceShape GetResourceShape()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetResourceShape(Handle, out error), () => error);
        }

        internal int GetResourceAccess()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetResourceAccess(Handle, out error), () => error);
        }

        internal string GetName()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetName(Handle, out error), () => error);
        }

        internal uint GetUserAttributeCount()
        {
            string? error = null;
            return Call(() => StrongInterop.TypeReflection.GetUserAttributeCount(Handle, out error), () => error);
        }

        internal AttributeReflection GetUserAttributeByIndex(uint index)
        {
            string? error = null;
            return new AttributeReflection(this, Call(() => StrongInterop.TypeReflection.GetUserAttributeByIndex(Handle, index, out error), () => error));
        }

        internal AttributeReflection FindAttributeByName(string name)
        {
            string? error = null;
            return new AttributeReflection(
                this,
                Call(() => StrongInterop.TypeReflection.FindAttributeByName(Handle, name, out error), () => error));
        }

        internal TypeReflection ApplySpecializations(GenericReflection genRef)
        {
            string? error = null;
            return new TypeReflection(this, Call(() => StrongInterop.TypeReflection.ApplySpecializations(Handle, genRef.Handle, out error), () => error));
        }

        internal GenericReflection GetGenericContainer()
        {
            string? error = null;
            return new GenericReflection(this, Call(() => StrongInterop.TypeReflection.GetGenericContainer(Handle, out error), () => error));
        }

        public bool Equals(TypeReflection? other)
        {
            return this == other;
        }
    }
}
