using SDL2;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Rendering_Engine
{
    class Program
    {
        //FPS Counter Vars
        private static int frameCount = 0;
        private static Stopwatch frameStopwatch = new Stopwatch();
        private static bool running = true;

        //SDL2 Surface Vars
        private IntPtr window;
        private IntPtr glContext;
        private GL gl;
        private int width = 800;
        private int height = 600;

        private string vertexShaderSource;
        private string fragmentShaderSource;
        private uint shaderProgram;


        static void Main(string[] args)
        {
            var app = new Program();

            try
            {
                app.Initialize();

                while (running)
                {
                    // --- Handle events ---
                    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
                    {
                        if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        {
                            running = false;
                        }
                    }

                    frameStopwatch.Start();

                    app.RenderFrame();

                    frameCount++;

                    //Display Frames Rendered in a Second
                    if (frameStopwatch.ElapsedMilliseconds >= 1000)
                    {
                        Console.WriteLine($"FPS: {frameCount}");
                        frameCount = 0;
                        frameStopwatch.Restart();
                    }
                }
            }
            finally
            {
                if (app.window != IntPtr.Zero)
                {
                    // Cleanup
                    SDL.SDL_GL_DeleteContext(app.glContext);
                    SDL.SDL_DestroyWindow(app.window);
                    SDL.SDL_Quit();
                }
                    
                
            }
            

        }

        private void Initialize()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            // Set SDL OpenGL attributes (version, profile, etc.)
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,
                (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

            window = SDL.SDL_CreateWindow(
                "3D Renderer",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width, height,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );

            //Create GL Context
            glContext = SDL.SDL_GL_CreateContext(window);

            // Disable vsync
            SDL.SDL_GL_SetSwapInterval(0);

            // Load Silk.NET OpenGL bindings using SDL's loader
            gl = GL.GetApi(procName => SDL.SDL_GL_GetProcAddress(procName));

            // Set up OpenGL state
            gl.Viewport(0, 0, 800, 600);
            gl.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            vertexShaderSource = LoadShaderSource("Shaders/simple.vert");
            fragmentShaderSource = LoadShaderSource("Shaders/simple.frag");

            // Generate & bind a Vertex Array Object
            uint vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // Generate & bind a Vertex Buffer Object
            uint vbo = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ArrayBuffer, vbo);

            unsafe
            {
                fixed (float* v = &vertices[0])
                {
                    gl.BufferData(GLEnum.ArrayBuffer,
                                  (nuint)(vertices.Length * sizeof(float)),
                                  v,
                                  GLEnum.StaticDraw);
                }
            
            
            // Tell OpenGL how to interpret the vertex data
            //Assigning Position Atrib
            gl.VertexAttribPointer(
                0,                          // matches layout(location = 0)
                3,                          // 3 floats per vertex
                GLEnum.Float,               // data type
                false,                      // don’t normalize
                6 * sizeof(float),          // stride (bytes per vertex)
                (void*)0                    // offset
            );

            //Assigning Colour Atrib
            gl.VertexAttribPointer(
                1,                          // matches layout(location = 1)
                3,                          // 3 floats per vertex
                GLEnum.Float,               // data type
                false,                      // don’t normalize
                6 * sizeof(float),          // stride (bytes per vertex)
                (void*)(3 * sizeof(float))                    // offset
            );

            }

            //STRIDE HOLDS DATA FOR EXAMPLE, FIRST 3 IS ASSIGNED IN SHADER
            //AS POS THEN NEXT 3 AS COLOUR AKA LOCATION 1,2

            gl.EnableVertexAttribArray(0); //Enable Pos Attrib
            gl.EnableVertexAttribArray(1); //Enable Colour Attrib






            // Compile vertex shader
            uint vertexShader = gl.CreateShader(GLEnum.VertexShader);
            gl.ShaderSource(vertexShader, vertexShaderSource);
            gl.CompileShader(vertexShader);
            CheckShaderCompileStatus(vertexShader);

            // Compile fragment shader
            uint fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentShaderSource);
            gl.CompileShader(fragmentShader);
            CheckShaderCompileStatus(fragmentShader);

            // Link into shader program
            uint shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);
            CheckProgramLinkStatus(shaderProgram);

            // Clean up shaders (they’re linked now)
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            // Store program for later
            this.shaderProgram = shaderProgram;





        }

        float[] vertices =
        {
             0.0f,  0.5f, 0.0f,     1.0f, 0.0f, 0.0f,  // top, red
            -0.5f, -0.5f, 0.0f,     0.0f, 1.0f, 0.0f,  // bottom left, green
             0.5f, -0.5f, 0.0f,     0.0f, 0.0f, 1.0f   // bottom right, blue
        };


        private void RenderFrame()
        {

            // Clear the screen
            gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            //DO COOL RENDER STUFF...

            gl.UseProgram(shaderProgram);

            //Draw Triangle
            gl.DrawArrays(PrimitiveType.Triangles, 0, 3);


            // Swap buffers
            SDL.SDL_GL_SwapWindow(window);

        }

        static string LoadShaderSource(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        private void CheckShaderCompileStatus(uint shader)
        {
            string infoLog = gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
                Console.WriteLine($"Shader compile log: {infoLog}");
        }

        private void CheckProgramLinkStatus(uint program)
        {
            string infoLog = gl.GetProgramInfoLog(program);
            if (!string.IsNullOrWhiteSpace(infoLog))
                Console.WriteLine($"Program link log: {infoLog}");
        }


    }
}
