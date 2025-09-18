using RenderingEngine.Objects;
using RenderingEngine.Rendering;
using SDL2;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;

namespace RenderingEngine
{
    class Program
    {
        public static bool running = true;
        private static int FPSFrameCount = 0;
        private static Stopwatch frameStopwatch = new Stopwatch();
        private static Stopwatch deltaTimeStopwatch = new Stopwatch();

        private IWindow window;
        private IInputContext input;
        private GL gl;

        public static int ScreenWidth = 1200, ScreenHeight = 1200;
        public static bool fullscreen = true;
        public static float aspectRatio;



        static void Main(string[] args)
        {
            var app = new Program();
            app.Run();
        }

        private void Run()
        {
            Initialize();

            // Create Shader Program
            RenderingEngine.Rendering.Shader shader = new RenderingEngine.Rendering.Shader(gl, "Shaders/simple.vert", "Shaders/simple.frag");
            
            
            // Create Renderer
            Renderer renderer = new Renderer(gl, shader.ProgramID);

            double lastTime = deltaTimeStopwatch.Elapsed.TotalSeconds;

            window.Run();

            while (running)
            {                
                

                frameStopwatch.Start();
                deltaTimeStopwatch.Start();

                double currentTime = deltaTimeStopwatch.Elapsed.TotalSeconds;
                double deltaTime = currentTime - lastTime;
                lastTime = currentTime;

                //Handle SDL Events
                InputHandler.HandleEvents();
                InputHandler.UpdateCamera(deltaTime);
                
                PhysicsObjectsHandler.TickObjs(deltaTime);

                //DO RENDERING
                renderer.Clear();
                renderer.Draw();

                

                //SDL.SDL_GL_SwapWindow(window);

                FPSFrameCount++;
                if (frameStopwatch.ElapsedMilliseconds >= 1000)
                {
                    Console.WriteLine($"FPS: {FPSFrameCount}");
                    FPSFrameCount = 0;
                    frameStopwatch.Restart();
                }

            }

            Cleanup();
        }

        private void Initialize()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(ScreenWidth, ScreenHeight);
            options.Title = "Doomy";
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 6));
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

            aspectRatio = (float)ScreenWidth / ScreenHeight;

            /*
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3); //3
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3); //3
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,
                (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

            SDL.SDL_DisplayMode displayMode = new SDL.SDL_DisplayMode();
            SDL.SDL_GetCurrentDisplayMode(0, out displayMode);            
            displayMode.format = SDL.SDL_PIXELFORMAT_RGBA8888;
            displayMode.driverdata = IntPtr.Zero;

            if (fullscreen) 
            {
                ScreenWidth = displayMode.w;
                ScreenHeight = displayMode.h;

                window = SDL.SDL_CreateWindow(
                "3D Renderer",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                ScreenWidth, ScreenHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN
            );
            }
            else
            {
                window = SDL.SDL_CreateWindow(
                "3D Renderer",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                ScreenWidth, ScreenHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );
            }
            
            


            glContext = SDL.SDL_GL_CreateContext(window);
            
            SDL.SDL_GL_SetSwapInterval(0);


            */
            gl = GL.GetApi(procName => SDL.SDL_GL_GetProcAddress(procName));
            gl.Viewport(0, 0, (uint)ScreenWidth, (uint) ScreenHeight);            
            gl.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            Console.WriteLine($"OpenGL Version: {gl.GetStringS(GLEnum.Version)}");

            if (!hasExtension(gl, "GL_ARB_bindless_texture")) {
                Cleanup();
            }
            

        }

        private void OnLoad()
        {
            gl = GL.GetApi(window);

            
        }
        private void Cleanup()
        {
            if (window != IntPtr.Zero)
            {
                SDL.SDL_GL_DeleteContext(glContext);
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
            }
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
