using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slang.Sdk.Collections;
using Slang.Sdk.Interop;

namespace Slang.Sdk
{
    public class Type : Reflection, IEquatable<Type>,
        IComposition<Variable>,
        IComposition<Attribute>,
        INamedComposition<Attribute>
    {
        #region Definition
        public override Reflection? Parent { get; }
        internal override Binding.TypeReflection Binding { get; }

        internal Type(Reflection parent, Binding.TypeReflection binding)
        {
            Parent = parent;
            Binding = binding;
        }
        #endregion

        #region IComposition<Variable>

        uint IComposition<Variable>.Count => Binding.GetFieldCount();
        Variable IComposition<Variable>.GetByIndex(uint index) =>
            new Variable(this, Binding.GetFieldByIndex(index));

        #endregion

        #region IComposition<Attribute>

        uint IComposition<Attribute>.Count => Binding.GetUserAttributeCount();
        Attribute IComposition<Attribute>.GetByIndex(uint index) =>
            new Attribute(this, Binding.GetUserAttributeByIndex(index));

        #endregion

        #region INamedComposition<Attribute>

        Attribute? INamedComposition<Attribute>.FindByName(string name) =>
            Binding.FindAttributeByName(name) is { } attr ? new Attribute(this, attr) : null;

        #endregion

        #region Collections

        SlangCollection<Variable>? _Fields;
        public SlangCollection<Variable> Fields => 
            _Fields ??= new SlangCollection<Variable>(this);

        SlangNamedCollectionary<Attribute>? _UserAttributes;
        public SlangNamedCollectionary<Attribute> UserAttributes => 
            _UserAttributes ??= new SlangNamedCollectionary<Attribute>(this, this);

        #endregion

        #region Pretty
        public string? Name => Binding.GetName();
        public TypeKind Kind => Binding.GetKind();
        public bool IsArray => Binding.IsArray();
        public nuint ElementCount => Binding.GetElementCount();
        public uint RowCount => Binding.GetRowCount();
        public uint ColumnCount => Binding.GetColumnCount();
        public ScalarType ScalarType => Binding.GetScalarType();
        public ResourceShape ResourceShape => Binding.GetResourceShape();
        public int ResourceAccess => Binding.GetResourceAccess();

        public Type? ElementType => 
            Binding.GetElementType() is { } element ? new Type(this, element) : null;

        public Type? UnwrapArray() =>
            Binding.UnwrapArray() is { } unwrapped ? new Type(this, unwrapped) : null;

        public Type? ResourceResultType =>
            Binding.GetResourceResultType() is { } result ? new Type(this, result) : null;

        public Generic? GenericContainer =>
            Binding.GetGenericContainer() is { } container ? new Generic(this, container) : null;
        public Type? ApplySpecializations(Generic? genRef) =>
            genRef?.Binding is { } genBinding && Binding.ApplySpecializations(genBinding) is { } specialized ? 
                new Type(this, specialized) : null;

        #endregion

        #region Equality
        public bool Equals(Type? other)
        {
            return this == other;
        }
        #endregion

    }
}
