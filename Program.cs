using ImGuiNET;
using RenderingEngine.Gui;
using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
using SDL2;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;
using System.Numerics;
using static Silk.NET.Core.Native.WinString;

namespace RenderingEngine
{
    class Program
    {
        public static bool running = true;
        private static int FPSFrameCount = 0;
        public static int lastFPS = 0;
        private static Stopwatch frameStopwatch = new Stopwatch();
        
        
        private GL gl;

        public static int ScreenWidth = 1200, ScreenHeight = 1200;
        public static bool fullscreen = false;
        public static float aspectRatio;

        // Create Shader Program
        public static IWindow window;
        private RenderingEngine.Rendering.Shader shader;
        private static Renderer renderer;
        public static ImGuiController _imgui;
        public static IInputContext input;

        // Create Gui Panels
        private HierarchyPanel hierarchyPanel;
        private InspectorPanel inspectorPanel;
        private SettingsPanel settingsPanel;


        // Create Chunk Size
        public static int chunkSize = 8;
        public static float tickRate = 1f;


        static void Main(string[] args)
        {
            var app = new Program();
            app.Run();
        }

        private void Run()
        {
            Initialize();
            window.Run();
            Cleanup();
        }

        private void Initialize()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(ScreenWidth, ScreenHeight);
            options.Title = "Doomy";
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 6)); //4, 6
            options.FramesPerSecond = 0;
            options.UpdatesPerSecond = 0;
            options.VSync = false;
            options.WindowBorder = fullscreen ? WindowBorder.Hidden : WindowBorder.Resizable;
            options.WindowState = fullscreen ? WindowState.Fullscreen : WindowState.Normal;

            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;            
            window.Resize += OnResize;
            
        }

        private void OnResize(Vector2D<int> size)
        {
            gl.Viewport(0, 0, (uint)size.X, (uint) size.Y);
            aspectRatio = (float)size.X / size.Y;
            ImGui.GetIO().DisplaySize = new Vector2(size.X, size.Y);
        }

        private void OnLoad()
        {
            gl = GL.GetApi(window);
            
            gl.Viewport(0, 0, (uint)ScreenWidth, (uint)ScreenHeight);
            gl.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            Console.WriteLine($"OpenGL Version: {gl.GetStringS(GLEnum.Version)}");
            Console.WriteLine($"Vendor: {gl.GetStringS(GLEnum.Vendor)}");
            Console.WriteLine($"Renderer: {gl.GetStringS(GLEnum.Renderer)}");
            // Create Shader Program
            shader = new RenderingEngine.Rendering.Shader(gl, "Shaders/simple.vert", "Shaders/simple.frag");
            // Create Renderer
            
            
            renderer = new Renderer(gl, shader.ProgramID);
            input = window.CreateInput();

            _imgui = new ImGuiController(gl, window, input);

            InputHandler.RegisterDevices(input, window);

            SetupGui();

            aspectRatio = (float)ScreenWidth / ScreenHeight;

            if (!hasExtension(gl, "GL_ARB_bindless_texture"))
            {
                Console.WriteLine("Bindless Textures Not Supported...");
                Cleanup();
            }
            else Console.WriteLine("Bindless Textures ARE Supported...");


            frameStopwatch.Start();
        }

        private void OnUpdate(double deltaTime)
        {
            InputHandler.UpdateCamera(deltaTime);

            PhysicsObjectsHandler.TickObjs(deltaTime);

            FPSFrameCount++;
            if (frameStopwatch.ElapsedMilliseconds >= 1000)
            {
                //Console.WriteLine($"FPS: {FPSFrameCount}");
                lastFPS = FPSFrameCount;
                FPSFrameCount = 0;
                frameStopwatch.Reset();
                frameStopwatch.Start();
            }
        }

        private void SetupGui()
        {            
            inspectorPanel = new InspectorPanel();
            hierarchyPanel = new HierarchyPanel(inspectorPanel);
            settingsPanel = new SettingsPanel();
        }

        private void OnRender(double deltaTime)
        {
            _imgui.Update((float)deltaTime);

            gl.Clear((uint)(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit));
            
            //DO RENDERING
            renderer.Clear();
            renderer.Draw();

            //Show Functions of ImGUI
            if (Camera.enableGUI)
            {
                
                hierarchyPanel.Draw();

                inspectorPanel.Draw();

                settingsPanel.Draw();
                
            }
            

            _imgui.Render();
        }

        

        public static void Cleanup()
        {
            //_imgui.Dispose(); //Throws Error, Not Neccisary?
            window.Close();
        }

        private bool hasExtension(GL gl, string name)
        {
            gl.GetInteger(GLEnum.NumExtensions, out int numExt);

            unsafe
            {
                for (uint i = 0; i < numExt; i++)
                {
                    string? ext = SilkMarshal.PtrToString((nint)gl.GetString(GLEnum.Extensions, i));                    
                    if (ext != null) 
                    { 
                        if (ext == name)
                        {
                            Console.WriteLine($"Checked Has Extension: {name}");
                            return true;
                        }
                    }

                }
                Console.WriteLine($"Does NOT have Extension: {name}");
                return false;
            }
        }
    }
}
