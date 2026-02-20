using Slang.Sdk.Binding;
using Slang.Sdk.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slang.Sdk
{
    public partial class Module : IEquatable<Module>
    {
        #region Definition
        public Session Parent { get; }
        internal Binding.Module Binding { get; }

        internal Module(Session parent, Module.Builder builder)
        {
            Parent = parent;
            Binding = new Binding.Module(Parent.Binding, builder.Binding);
            Name = null;
        }

        internal Module(Session parent, string moduleName, string modulePath, string moduleSource)
        {
            Parent = parent;
            Binding = new Binding.Module(Parent.Binding, moduleName, modulePath, moduleSource);
            Name = moduleName;
        }

        internal Module(Session parent, string moduleName)
        {
            Parent = parent;
            Binding = new Binding.Module(Parent.Binding, moduleName);
            Name = moduleName;
        }

        internal Module(Session parent, Binding.Module binding, string? moduleName = null)
        {
            Parent = parent;
            Binding = binding;
            Name = moduleName;
        }
        #endregion

        #region Pretty
        Program? _Program;
        public Program Program => _Program ??= new Program(this);
        public string? Name { get; }
        #endregion

        #region Equality
        public static bool operator ==(Module? left, Module? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            if (left.Binding == right.Binding) return true;
            return false;
        }

        public static bool operator !=(Module? left, Module? right)
        {
            if (ReferenceEquals(left, right)) return false;
            if (left is null || right is null) return true;
            if (left.Binding == right.Binding) return false;
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Module entryPoint) return Equals(entryPoint);
            return false;
        }

        public bool Equals(Module? other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Binding.GetHashCode();
        }
        #endregion

        #region Disposable
        private bool _disposed = false; // To detect redundant calls

        ~Module()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevent Finalize from being called
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                }

                // Release unmanaged resources here
                Binding?.Dispose();

                _disposed = true;
            }
        }
        #endregion
    }
}
