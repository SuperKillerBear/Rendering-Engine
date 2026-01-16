using ImGuiNET;
using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using RenderingEngine.Gui;
using RenderingEngine.Meshes;
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
        
        public static GL gl;

        public static int ScreenWidth = 1200, ScreenHeight = 3000;
        public static bool fullscreen = true;
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


        public static List<GameObject> SceneObjects = new List<GameObject>();

        public static bool RenderingEnabled = true;
        public static bool PhysicsEnabled = true;

        public static float ResolutionScale = 1.0f;


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
            //window.Resize += OnResize;
            
        }

        private void OnResize(Vector2D<int> size)
        {
            gl.Viewport(0, 0, (uint)(size.X), (uint)(size.Y));
            aspectRatio = (float)((size.X * ResolutionScale) / (size.Y * ResolutionScale));
            ImGui.GetIO().DisplaySize = new Vector2(size.X, size.Y);
        }

        public static void UpdateResolution()
        {
            var size = window.FramebufferSize;
            int width = size.X;
            int height = size.Y;
            aspectRatio = (float)(width * ResolutionScale) / (height * ResolutionScale);
        }

        private void OnLoad()
        {
            gl = GL.GetApi(window);
            var fbSize = window.FramebufferSize;

            // Set OpenGL viewport to match framebuffer
            gl.Viewport(0, 0, (uint)fbSize.X, (uint)fbSize.Y);
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
            
            
            Console.WriteLine("ImGui Name: ", _imgui.GetType().AssemblyQualifiedName);
            Console.WriteLine("ImGui Location:", _imgui.GetType().Assembly.Location);

            // Update ImGui display size
            //ImGui.GetIO().DisplaySize = new Vector2(fbSize.X, fbSize.Y);

            InputHandler.RegisterDevices(input, window);

            SetupGui();

            MaterialHandler.Init();
            MeshHandler.Init();
            FileHandler.Init(); //Load Default Game Settings


            aspectRatio = (float)ScreenWidth / ScreenHeight;

            if (!hasExtension(gl, "GL_ARB_bindless_texture"))
            {
                Console.WriteLine("Bindless Textures Not Supported...");
                Cleanup();
            }
            else Console.WriteLine("Bindless Textures ARE Supported...");

            
            
            var Hospital = new GameObject();
            Hospital.name = "Hospital";
            Hospital.AddComponent<RendererComponent>().SetMesh("SilentHill");
            //Hospital.AddComponent<RendererComponent>().SetMeshID(0);
            Hospital.AddComponent<BoxColliderComponent>();
            
            /*
            var PhysicsCube = new GameObject();
            PhysicsCube.name = "Physics Cube";
            PhysicsCube.Transform.Translate(new Vector3D<float>(0, 5, 0));
            PhysicsCube.AddComponent<RendererComponent>().SetMeshID(0);
            PhysicsCube.AddComponent<RigidBodyComponent>();
            */

            /*
            var KelleyRoad = new GameObject();
            KelleyRoad.name = "Kelley Road";
            KelleyRoad.AddComponent<RendererComponent>().SetMesh("kelley-road");
            */
            //var ren = cube.GetComponent<RendererComponent>();
            //ren.material = mat2;

            frameStopwatch.Start();
        }

        private void OnUpdate(double deltaTime)
        {
            InputHandler.UpdateCamera(deltaTime);

            if (PhysicsEnabled) PhysicsObjectsHandler.TickObjs(deltaTime);

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
            ImGui.LoadIniSettingsFromMemory(""); 
            inspectorPanel = new InspectorPanel();
            hierarchyPanel = new HierarchyPanel(inspectorPanel);
            settingsPanel = new SettingsPanel();
        }

        private void OnRender(double deltaTime)
        {
            // Clear Colour and Depth Buffer for Next Frame
            gl.Clear((uint)(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit));

            // Call ImGui next Frame code
            _imgui.Update((float)deltaTime);
            
            //DO RENDERING
            renderer.Clear();
            if (RenderingEnabled) renderer.Draw();

            // Build UI here
            if (Camera.enableGUI)
            {

                hierarchyPanel.Draw();

                inspectorPanel.Draw();

                settingsPanel.Draw();
                
            }
            

            // Force Fixing of ImGui 
            gl.BindFramebuffer(GLEnum.Framebuffer, 0);
            var fb = window.FramebufferSize;
            gl.Viewport(0, 0, (uint)fb.X, (uint)fb.Y);
            
            //gl.Disable(GLEnum.ScissorTest);
            
            // Force correct scale for this frame
            var io = ImGui.GetIO();
            io.DisplayFramebufferScale = new Vector2(
                window.FramebufferSize.X / (float)window.Size.X,
                window.FramebufferSize.Y / (float)window.Size.Y
            );
            
            // Console Debug for ImGui scaling issues, (used integer division before)
            //Console.WriteLine($"view.Size={window.Size} fb={window.FramebufferSize} scale={io.DisplayFramebufferScale} display={io.DisplaySize}");

            _imgui.Render();
        }

        public static void ClearScene()
        {
            PhysicsEnabled = false;
            RenderingEnabled = false;

            Renderer.RenderingObjects.Clear();
            PhysicsObjectsHandler.ClearAll();

            MaterialHandler.UnloadTextures();
            MeshHandler.UnloadAll();


            //TODO: Clear Textures from Handler

            foreach (GameObject obj in SceneObjects)
                obj.Dispose();
            SceneObjects.Clear();

            //Force Memory Cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();


        }

        public static void Cleanup()
        {
            //_imgui.Dispose(); //Throws Error, Not Neccisary?
            FileHandler.SaveGameSettings();
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
