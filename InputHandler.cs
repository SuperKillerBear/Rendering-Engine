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
using ImGuiNET;

namespace RenderingEngine
{
    public static class InputHandler
    {
        private static Vector2 lastPos = Vector2.Zero;
        public static float accumMouseRelX, accumMouseRelY;

        // persistent key state
        static bool w, a, s, d, up, down, sprint;

        private static IKeyboard? keyboard;
        private static IMouse? mouse;
        private static IView? window;

        private static ImGuiNET.ImGuiIOPtr io;

        public static void RegisterDevices(IInputContext inputContext, IView Window)
        {
            window = Window;
            keyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
            mouse = inputContext.Mice.Count > 0 ? inputContext.Mice[0] : null;
            mouse.Cursor.CursorMode = Camera.enableGUI ? CursorMode.Normal : CursorMode.Raw;

            io = ImGui.GetIO();

            Console.WriteLine(
                $"Mouse:{mouse.Position} | Framebuffer:{window.FramebufferSize} | Window:{window.Size} | Scale:{io.DisplayFramebufferScale}"
            );


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
            //Console.WriteLine($"Mouse Pos: ({vector.X},{vector.Y})");
            //Console.WriteLine($"{io.DisplayFramebufferScale.ToString()}");
            var pos = m.Position;
            pos.Y = window.Size.Y - pos.Y;
            //pos.X *= io.DisplayFramebufferScale.X;
            //pos.Y *= io.DisplayFramebufferScale.Y;
            io.AddMousePosEvent(pos.X, pos.Y); //Pass Mouse Movement to ImGui

            var delta = lastPos - vector;

            // accumulate this frame’s motion
            accumMouseRelX += delta.X;
            accumMouseRelY += delta.Y;

            lastPos = vector;
        }

        private static void OnKeyDown(IKeyboard kb, Key key, int code)
        {
            var imguiKey = ToImGuiKey(key);
            if (imguiKey != ImGuiKey.None) io.AddKeyEvent(imguiKey, true); //Pass Key Down to ImGUI
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
            var imguiKey = ToImGuiKey(key);
            if (imguiKey != ImGuiKey.None) io.AddKeyEvent(imguiKey, false); //Pass Key Down to ImGUI
            switch (key)
            {
                case Key.W: w = false; break;
                case Key.A: a = false; break;
                case Key.S: s = false; break;
                case Key.D: d = false; break;
                case Key.Space: up = false; break;
                case Key.ControlLeft: down = false; break;
                case Key.ShiftLeft: sprint = false; break;
                case Key.Tab: 
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

        private static ImGuiKey ToImGuiKey(Key key)
        {
            return key switch
            {
                Key.Tab => ImGuiKey.Tab,
                Key.Left => ImGuiKey.LeftArrow,
                Key.Right => ImGuiKey.RightArrow,
                Key.Up => ImGuiKey.UpArrow,
                Key.Down => ImGuiKey.DownArrow,
                Key.Delete => ImGuiKey.Delete,
                Key.Backspace => ImGuiKey.Backspace,
                Key.Space => ImGuiKey.Space,
                Key.Enter => ImGuiKey.Enter,
                Key.Escape => ImGuiKey.Escape,
                Key.A => ImGuiKey.A,
                Key.B => ImGuiKey.B,
                Key.C => ImGuiKey.C,
                Key.D => ImGuiKey.D,
                Key.E => ImGuiKey.E,
                Key.F => ImGuiKey.F,
                Key.G => ImGuiKey.G,
                Key.H => ImGuiKey.H,
                Key.I => ImGuiKey.I,
                Key.J => ImGuiKey.J,
                Key.K => ImGuiKey.K,
                Key.L => ImGuiKey.L,
                Key.M => ImGuiKey.M,
                Key.N => ImGuiKey.N,
                Key.O => ImGuiKey.O,
                Key.P => ImGuiKey.P,
                Key.Q => ImGuiKey.Q,
                Key.R => ImGuiKey.R,
                Key.S => ImGuiKey.S,
                Key.T => ImGuiKey.T,
                Key.U => ImGuiKey.U,
                Key.V => ImGuiKey.V,
                Key.W => ImGuiKey.W,
                Key.X => ImGuiKey.X,
                Key.Y => ImGuiKey.Y,
                Key.Z => ImGuiKey.Z,
                _ => ImGuiKey.None,
            };
        }

    }
}
