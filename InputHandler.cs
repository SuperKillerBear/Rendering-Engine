using RenderingEngine;
using SDL2;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class InputHandler
    {
        public static int FOV = 80;
        public static Vector3D<float> Position = Vector3D<float>.Zero;
        public static Vector3D<float> Rotation = Vector3D<float>.Zero;
        private const float moveSpeed = 0.05f;

        public static void HandleEvents()
        {
            
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    RenderingEngine.Program.running = false;

                if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    switch (e.key.keysym.sym)
                    {
                        case SDL.SDL_Keycode.SDLK_w:
                            Console.WriteLine($"W pressed, Pos: {Position}");
                            Position.Z -= moveSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_a:
                            Console.WriteLine($"A pressed, Pos: {Position}");
                            Position.X += moveSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_s:
                            Console.WriteLine($"S pressed, Pos: {Position}");
                            Position.Z += moveSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_d:
                            Console.WriteLine($"D pressed, Pos: {Position}");
                            Position.X -= moveSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_ESCAPE:
                            Program.running = false;
                            break;
                    }
                }
            }
        }
    }
}
