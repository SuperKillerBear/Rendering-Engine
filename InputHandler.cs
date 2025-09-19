using RenderingEngine;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Windowing;
using Silk.NET.Input;

namespace RenderingEngine
{
    public static class InputHandler
    {
        //static IInputContext input = Program.window.CreateInput();

        static int accumMouseRelX, accumMouseRelY;

        // persistent key state
        static bool w, a, s, d, up, down, sprint;

        private static IKeyboard? keyboard;
        private static IMouse? mouse;

        public static void RegisterDevices(IInputContext inputContext, IView window)
        {
            keyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
            mouse = inputContext.Mice.Count > 0 ? inputContext.Mice[0] : null;

            if (keyboard != null)
            {
                keyboard.KeyDown += OnKeyDown;
                keyboard.KeyUp += OnKeyUp;
            }

            if (mouse != null)
            {
                mouse.MouseMove += OnMouseMove;
            }
        }


        private void OnKeyDown(IKeyboard kb, Key key, int code)
        {
            switch(key)
            {
                case Key.W: w = true; break;
                case Key.A: a = true; break;
                case Key.S: s = true; break;
                case Key.D: d = true; break;
                case Key.Space: up = true; break;
                case Key.ControlLeft: down = true; break;
                case Key.ShiftLeft: sprint = true; break;
                case Key.Escape: Program.Cleanup(); break;
            }
        }


        public static void HandleEvents()
        {


            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    RenderingEngine.Program.running = false;
                
                if (e.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
                {
                    // accumulate this frame’s motion
                    accumMouseRelX += e.motion.xrel;
                    accumMouseRelY += e.motion.yrel;
                }
                
                if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    switch (e.key.keysym.sym)
                    {
                        case SDL.SDL_Keycode.SDLK_w: w = true; break;
                        case SDL.SDL_Keycode.SDLK_a: a = true; break;
                        case SDL.SDL_Keycode.SDLK_s: s = true; break;
                        case SDL.SDL_Keycode.SDLK_d: d = true; break;
                        case SDL.SDL_Keycode.SDLK_SPACE: up = true; break;
                        case SDL.SDL_Keycode.SDLK_LCTRL: down = true; break;
                        case SDL.SDL_Keycode.SDLK_LSHIFT: sprint = true; break;
                        case SDL.SDL_Keycode.SDLK_ESCAPE:
                            Program.running = false;
                            break;
                    }
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    switch (e.key.keysym.sym)
                    {
                        case SDL.SDL_Keycode.SDLK_w: w = false; break;
                        case SDL.SDL_Keycode.SDLK_a: a = false; break;
                        case SDL.SDL_Keycode.SDLK_s: s = false; break;
                        case SDL.SDL_Keycode.SDLK_d: d = false; break;
                        case SDL.SDL_Keycode.SDLK_SPACE: up = false; break;
                        case SDL.SDL_Keycode.SDLK_LCTRL: down = false; break;
                        case SDL.SDL_Keycode.SDLK_LSHIFT: sprint = false; break;
                    }
                }

            }
        }

        // Call once per frame *after* HandleEvents
        public static void UpdateCamera(double deltaTime)
        {
            // mouse: yaw = +xrel, pitch = -yrel (typical FPS controls)
            Camera.CalcLookVector(relPitch: accumMouseRelY, relYaw: accumMouseRelX);

            // reset deltas for next frame
            accumMouseRelX = 0;
            accumMouseRelY = 0;

            // build per-axis intentions (nullable as Camera Class expects)
            bool? x = a == d ? (bool?)null : (d ? false : true);
            bool? y = down == up ? (bool?)null : (up ? true : false);
            bool? z = s == w ? (bool?)null : (w ? true : false);

            Camera.CalcMoveVector(x, y, z);
            Camera.Move(deltaTime, sprint);
        }

    }
}
