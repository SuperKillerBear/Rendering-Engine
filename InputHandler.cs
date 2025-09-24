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
using System.Numerics;

namespace RenderingEngine
{
    public static class InputHandler
    {
        private static Vector2 lastPos = Vector2.Zero;
        static float accumMouseRelX, accumMouseRelY;

        // persistent key state
        static bool w, a, s, d, up, down, sprint;

        private static IKeyboard? keyboard;
        private static IMouse? mouse;

        public static void RegisterDevices(IInputContext inputContext, IView window)
        {
            keyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
            mouse = inputContext.Mice.Count > 0 ? inputContext.Mice[0] : null;
            mouse.Cursor.CursorMode = Camera.enableGUI ? CursorMode.Normal : CursorMode.Raw;

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


        private static void OnMouseMove(IMouse m, Vector2 vector)
        {
            var delta = lastPos - vector;

            // accumulate this frame’s motion
            accumMouseRelX += delta.X;
            accumMouseRelY += delta.Y;

            lastPos = vector;
        }

        private static void OnKeyDown(IKeyboard kb, Key key, int code)
        {
            if (Camera.enableGUI) return;
            switch (key)
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

        private static void OnKeyUp(IKeyboard kb, Key key, int code)
        {
            switch (key)
            {
                case Key.W: w = false; break;
                case Key.A: a = false; break;
                case Key.S: s = false; break;
                case Key.D: d = false; break;
                case Key.Space: up = false; break;
                case Key.ControlLeft: down = false; break;
                case Key.ShiftLeft: sprint = false; break;
                case Key.E: 
                    Camera.enableGUI = !Camera.enableGUI;
                    mouse.Cursor.CursorMode = Camera.enableGUI ? CursorMode.Normal : CursorMode.Raw;
                    break;
            }
        }


        // Call once per frame *after* HandleEvents
        public static void UpdateCamera(double deltaTime)
        {
            if (!Camera.enableGUI) Camera.CalcLookVector(relPitch: -accumMouseRelY, relYaw: -accumMouseRelX);
            

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
