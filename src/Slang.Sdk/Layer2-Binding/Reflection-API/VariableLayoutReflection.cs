using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slang.Sdk.Interop;
using static Slang.Sdk.Interop.Utilities;

namespace Slang.Sdk.Binding
{
    internal unsafe class VariableLayoutReflection : Reflection, IEquatable<VariableLayoutReflection>
    {
        internal override Reflection? Parent { get; }
        internal override VariableLayoutReflectionHandle Handle { get; }
        internal override VariableLayoutReflectionHandle NativeHandle => new(StrongInterop.VariableLayoutReflection.GetNative(Handle, out var _));


        internal VariableLayoutReflection(Reflection parent, VariableLayoutReflectionHandle handle)
        {
            Parent = parent;
            Handle = handle;
        }

        internal VariableReflection GetVariable()
        {
            string? error = null;
            return new VariableReflection(this, Call(() => StrongInterop.VariableLayoutReflection.GetVariable(Handle, out error), () => error));
        }

        internal string GetName()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetName(Handle, out error), () => error);
        }

        internal ModifierReflection FindModifier(int modifierId)
        {
            string? error = null;
            return new ModifierReflection(this, Call(() => StrongInterop.VariableLayoutReflection.FindModifier(Handle, modifierId, out error), () => error));
        }

        internal TypeLayoutReflection GetTypeLayout()
        {
            string? error = null;
            return new TypeLayoutReflection(this, Call(() => StrongInterop.VariableLayoutReflection.GetTypeLayout(Handle, out error), () => error));
        }

        internal ParameterCategory GetCategory()
        {
            string? error = null;
            return (ParameterCategory)Call(() => StrongInterop.VariableLayoutReflection.GetCategory(Handle, out error), () => error);
        }

        internal uint GetCategoryCount()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetCategoryCount(Handle, out error), () => error);
        }

        internal ParameterCategoryStruct GetCategoryByIndex(uint index)
        {
            string? error = null;
            return (ParameterCategory)Call(() => StrongInterop.VariableLayoutReflection.GetCategoryByIndex(Handle, index, out error), () => error);
        }

        internal nuint GetOffset(ParameterCategory category)
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetOffset(Handle, (int)category, out error), () => error);
        }

        internal new TypeReflection GetType()
        {
            string? error = null;
            return new TypeReflection(this, Call(() => StrongInterop.VariableLayoutReflection.GetType(Handle, out error), () => error));
        }

        internal uint GetBindingIndex()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetBindingIndex(Handle, out error), () => error);
        }

        internal uint GetBindingSpace()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetBindingSpace(Handle, out error), () => error);
        }

        internal VariableLayoutReflection GetSpace(ParameterCategory category)
        {
            string? error = null;
            return new VariableLayoutReflection(this, Call(() => StrongInterop.VariableLayoutReflection.GetSpace(Handle, (int)category, out error), () => error));
        }

        internal int GetImageFormat()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetImageFormat(Handle, out error), () => error);
        }

        internal string GetSemanticName()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetSemanticName(Handle, out error), () => error);
        }

        internal uint GetSemanticIndex()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetSemanticIndex(Handle, out error), () => error);
        }

        internal Stage GetStage()
        {
            string? error = null;
            return Call(() => StrongInterop.VariableLayoutReflection.GetStage(Handle, out error), () => error);
        }

        internal VariableLayoutReflection GetPendingDataLayout()
        {
            string? error = null;
            return new VariableLayoutReflection(this, Call(() => StrongInterop.VariableLayoutReflection.GetPendingDataLayout(Handle, out error), () => error));
        }

        public bool Equals(VariableLayoutReflection? other)
        {
            return this == other;
        }
    }
}
