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
        private static bool running = true;
        private static int FPSFrameCount = 0;
        private static Stopwatch frameStopwatch = new Stopwatch();

        private IntPtr window;
        private IntPtr glContext;
        private GL gl;

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

            // Create Object to Render
            

            while (running)
            {
                // Handle SDL events
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        running = false;
                }

                frameStopwatch.Start();

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
                800, 600,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );

            glContext = SDL.SDL_GL_CreateContext(window);
            SDL.SDL_GL_SetSwapInterval(0);

            gl = GL.GetApi(procName => SDL.SDL_GL_GetProcAddress(procName));
            gl.Viewport(0, 0, 800, 600);
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
