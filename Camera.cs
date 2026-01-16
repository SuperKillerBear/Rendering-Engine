using RenderingEngine.Utilities;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class Camera
    {
        public static int FOV = 80;        
        public static Vector3D<float> MoveDirection = Vector3D<float>.Zero;
        
        public static Vector3D<float> Position = new Vector3D<float>(0f, 0f, 3f);
        public static Vector3D<float> Rotation = Vector3D<float>.Zero;
        
        private static float moveSpeed = 25f;
        private static float sprintMulti = 2;

        public static Vector3D<float> Forward;
        public static Vector3D<float> Right;
        public static Vector3D<float> Up = new Vector3D<float>(0, 1, 0);

        public static float Sensitivity = 0.001f;

        public static bool enableGUI = false;

        public static void CalcLookVector(float relPitch, float relYaw)
        {
            Rotation.X -= relPitch * Sensitivity;
            Rotation.Y += relYaw * Sensitivity;

            Rotation.X = Math.Clamp(Rotation.X, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

            Forward = UMath.Normalize( new Vector3D<float>(
                MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y),
                MathF.Sin(Rotation.X),
                MathF.Cos(Rotation.X) * MathF.Sin(Rotation.Y)
                ));

            Right = UMath.Cross(
                Up,
                Forward                
                );
        }

        public static void CalcMoveVector(bool? x, bool? y, bool? z)
        {
            MoveDirection = Vector3D<float>.Zero;

            if (x == true) MoveDirection += Right;
            if (x == false) MoveDirection -= Right;
            
            if (y == true) MoveDirection += Up;
            if (y == false) MoveDirection -= Up;

            if (z == true) MoveDirection += Forward;
            if (z == false) MoveDirection -= Forward;

            //Console.WriteLine($"Move Direction: {MoveDirection.ToString()}");
        }

        public static void Move(double deltaTime, bool sprint)
        {
            //Create multiplier based on sprinting or not
            float moveMulti = sprint ? sprintMulti : 1f;
            
            //NOTE: Applies to all directions!

            //Apply Movement Speed
            Position += UMath.ScaleVec( MoveDirection, (float) deltaTime * moveSpeed * moveMulti);
        }
    }

    
}
