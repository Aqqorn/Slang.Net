using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;
using System.Numerics;

namespace SlangCube
{
    class Program
    {
        // Window, Input, and Camera
        private static IWindow window;
        private static Input input;
        private static Camera camera = new Camera();

        // Renderer
        private static SlangShader Shader;
        private static IRenderer Renderer;
        private static IMesh Mesh;

        // Graphics Backend
        private static GraphicsBackend SelectedBackend = GraphicsBackend.None;

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Slang.Net Demo!");
            Console.WriteLine("Please select a graphics backend:");
            Console.WriteLine("1. OpenGL");
            Console.WriteLine("2. DirectX11");

            // Gets the user's choice for the graphics backend
            while (SelectedBackend == GraphicsBackend.None)
            {
                Console.Write("Enter your choice (1 or 2): ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SelectedBackend = GraphicsBackend.OpenGL;
                        Console.WriteLine("Selected OpenGL backend.");
                        break;
                    case "2":
                        SelectedBackend = GraphicsBackend.DirectX11;
                        Console.WriteLine("Selected DirectX11 backend.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice, defaulting to OpenGL.");
                        SelectedBackend = GraphicsBackend.None;
                        break;
                }
            }

            // Define logic for showing the window based on the selected backend
            void ShowWindow(GraphicsBackend selectedBackend)
            {
                var options = WindowOptions.Default;
                options.Size = new Vector2D<int>(800, 600);
                options.Title = $"Slang.Net Demo - (Press Alt+F4 to exit)";
                options.API = selectedBackend switch
                {
                    GraphicsBackend.OpenGL => GraphicsAPI.Default,
                    _ => GraphicsAPI.None
                };

                window = Window.Create(options);

                window.Load += Load;
                window.Update += Update;
                window.Render += Render;
                window.FramebufferResize += OnFramebufferResize;
                window.Closing += OnClose;

                window.Run();

                window.Dispose();
            }

            // Show the window based on the selected backend
            ShowWindow(SelectedBackend);
        }

        private static void Load()
        {
            // Initialize Input
            input = new Input(window, () => camera, (newValue) => camera = newValue);
            input.Load();

            // Initialize based on selected backend
            Console.WriteLine($"Initializing {SelectedBackend} backend...");

            // Load the Slang shader from file
            var slangShader = new SlangShader("Shaders/shader.slang");

            // Shader target agnostic compilation
            var source = slangShader.CompileShaders(SelectedBackend);

            Renderer = SelectedBackend switch
            {
                GraphicsBackend.OpenGL => new OpenGLRenderer(window),
                GraphicsBackend.DirectX11 => new DirectX11Renderer(window),
                _ => throw new NotSupportedException($"The selected backend {SelectedBackend} is not supported."),
            };

            Mesh = Renderer.CreateCubeMesh(source.vertexShaderSource, source.fragmentShaderSource, texturePath: "Resources\\silk.png", modelPath: "Resources\\cube.model");
        }

        private static unsafe void Update(double deltaTime)
        {
            var moveSpeed = 2.5f * (float)deltaTime;

            if (input.primaryKeyboard.IsKeyPressed(Key.W))
            {
                //Move forwards
                camera.Position += moveSpeed * camera.Front;
            }
            if (input.primaryKeyboard.IsKeyPressed(Key.S))
            {
                //Move backwards
                camera.Position -= moveSpeed * camera.Front;
            }
            if (input.primaryKeyboard.IsKeyPressed(Key.A))
            {
                //Move left
                camera.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * moveSpeed;
            }
            if (input.primaryKeyboard.IsKeyPressed(Key.D))
            {
                //Move right
                camera.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * moveSpeed;
            }
        }

        private static unsafe void Render(double deltaTime)
        {
            // Graphics backend agnostic render call
            Renderer.Render(window, Mesh, camera);
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            (Renderer as OpenGLRenderer)?._gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            Mesh?.Dispose();
            Renderer?.Dispose();
            Shader?.Dispose();
        }
    }
}
