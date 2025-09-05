using RenderingEngine.Objects;
using RenderingEngine.Rendering;
using SDL2;
using Silk.NET.OpenGL;
using System;
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

        private IntPtr window;
        private IntPtr glContext;
        private GL gl;

        public static int ScreenWidth = 800, ScreenHeight = 600;

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

                PhysicsObjectsHandler.TickObjects(deltaTime);

                //DO RENDERING
                renderer.Clear();
                renderer.Draw();


                SDL.SDL_GL_SwapWindow(window);

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
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,
                (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

            window = SDL.SDL_CreateWindow(
                "3D Renderer",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                ScreenWidth, ScreenHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );

            glContext = SDL.SDL_GL_CreateContext(window);
            SDL.SDL_GL_SetSwapInterval(0);

            gl = GL.GetApi(procName => SDL.SDL_GL_GetProcAddress(procName));
            gl.Viewport(0, 0, (uint)ScreenWidth, (uint) ScreenHeight);            
            gl.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
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
    }
}
