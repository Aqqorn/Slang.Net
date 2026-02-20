using Slang.Sdk;

namespace ReflectionAPI
{
    /// <summary>
    /// Comprehensive showcase of Slang.Net Reflection API features.
    /// Demonstrates basic to advanced reflection usage on a real-world shader.
    /// </summary>
    public class ReflectionApiShowcase
    {
        public static void Run()
        {
            Console.WriteLine("SLANG.NET REFLECTION API SHOWCASE");
            Console.WriteLine("====================================\n");

            // Load the shader module
            var session = new Session.Builder()
                .AddSearchPath(AppDomain.CurrentDomain.BaseDirectory)
                .AddTarget(Targets.SpirV.vulkan_1_2)
                .Create();
            var module = session.LoadModule("Shaders/ReflectionSamples.slang");
            var program = module.Program;
            var reflection = program.Targets[Targets.SpirV.vulkan_1_2].GetReflection();

            // Basic: List entry points and their stages
            Console.WriteLine("Entry Points:");
            foreach (var entry in reflection.EntryPoints)
            {
                Console.WriteLine($"- {entry.Name} (Stage: {entry.Stage})");
            }
            Console.WriteLine();

            // Intermediate: List global parameters (constant buffers, textures, samplers)
            Console.WriteLine("Global Parameters:");
            foreach (var param in reflection.Parameters)
            {
                Console.WriteLine($"- {param.Name} (Kind: {param.Type.Kind}, Binding Space: {param.BindingSpace}, Binding Index: {param.BindingIndex})");
            }
            Console.WriteLine();

            // Advanced: Inspect type layouts and attributes
            Console.WriteLine("Type Layouts for Constant Buffers:");
            foreach (var param in reflection.Parameters.Where(p => p.Type.Kind == Slang.Sdk.Interop.TypeKind.ConstantBuffer))
            {
                var layout = param.TypeLayout;
                var size = layout.GetSize(Slang.Sdk.Interop.ParameterCategory.Uniform);
                if (size == 0)
                    size = layout.GetSize(Slang.Sdk.Interop.ParameterCategory.ConstantBuffer);

                var alignment = layout.GetAlignment(Slang.Sdk.Interop.ParameterCategory.Uniform);
                if (alignment == 0)
                    alignment = layout.GetAlignment(Slang.Sdk.Interop.ParameterCategory.ConstantBuffer);

                Console.WriteLine($"- {param.Name}: Size={size}, Alignment={alignment}");
                foreach (var field in layout.Fields)
                {
                    Console.WriteLine($"    Field: {field.Name}, Type: {field.Type.Name}");
                }
            }
            Console.WriteLine();

            // Advanced: Show attributes and semantics on entry points
            Console.WriteLine("Entry Point Attributes and Semantics:");
            foreach (var entry in reflection.EntryPoints)
            {
                Console.WriteLine($"- {entry.Name}");
                foreach (var attr in entry.Function.UserAttributes)
                {
                    Console.WriteLine($"    Attribute: {attr.Name}");
                }
            }
            Console.WriteLine();

            // Expert: Export full reflection data as JSON
            Console.WriteLine("Full Reflection JSON:");
            Console.WriteLine(reflection.ToJson());
        }
    }
}
