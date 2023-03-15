using System;
using System.Reflection;
using System.Runtime.Loader;

namespace OTelHttpRepro
{
    public class CustomLoadContext : AssemblyLoadContext
    {
        public CustomLoadContext(string assemblyPath)
        {
            AssemblyDependencyResolver = new AssemblyDependencyResolver(assemblyPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = AssemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);

            if (string.IsNullOrWhiteSpace(assemblyPath))
            {
                // fallback to loading with default context
                return null;
            }

            return LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libPath = AssemblyDependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (string.IsNullOrWhiteSpace(libPath))
            {
                // fallback to loading with default context
                return IntPtr.Zero;
            }

            return LoadUnmanagedDllFromPath(libPath);
        }

        private AssemblyDependencyResolver AssemblyDependencyResolver { get; }
    }
}