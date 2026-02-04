using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Utilities
{
    public static class UMath
    {    
    
        public static void WriteSilkVec3(BinaryWriter writer, Vector3D<float> vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }
        
        public static Vector3D<float> ReadSilkVec3(BinaryReader reader)
        {
            var result = new Vector3D<float>();
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            result.Z = reader.ReadSingle();
            
            return result;
        }
        
        
        public static float Dot(this Vector3D<float> a, Vector3D<float> b)
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector3D<float> Cross(this Vector3D<float> a, Vector3D<float> b)
            => new Vector3D<float>(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );

        public static float LengthSquared(this Vector3D<float> v)
            => v.X * v.X + v.Y * v.Y + v.Z * v.Z;

        public static float Length(this Vector3D<float> v)
            => MathF.Sqrt(v.LengthSquared());

        public static Vector3D<float> Normalize(this Vector3D<float> v)
        {
            float len = v.Length();
            if (len <= 1e-9f) return new Vector3D<float>(0f, 0f, 0f);
            return new Vector3D<float>(v.X / len, v.Y / len, v.Z / len);
        }

        public static Vector3D<float> ScaleVec(Vector3D<float> vec, float factor)
        {
            return new Vector3D<float>(
                vec.X * factor,
                vec.Y * factor,
                vec.Z * factor
                );
        }

        public static int Sign(float v)
        {
            return v < 0 ? -1 : 1;
        }

        public static void RotateAABB(
            Vector3D<float> min, 
            Vector3D<float> max, 
            Vector3D<float> rotation, 
            out Vector3D<float> newMin, 
            out Vector3D<float> newMax)
        {
            var centre = (min + max) / 2;
            var extent = (max - min) / 2;

            float cX = (float)Math.Cos(rotation.X);
            float sX = (float)Math.Sin(rotation.X);

            float cY = (float)Math.Cos(rotation.Y);
            float sY = (float)Math.Sin(rotation.Y);

            float cZ = (float)Math.Cos(rotation.Z);
            float sZ = (float)Math.Sin(rotation.Z);

            //Roation Matrix
            float r11 = cY * cZ;
            float r12 = cY * sZ;
            float r13 = -sY;

            float r21 = sX * sY * cZ - cX * sZ;
            float r22 = sX * sY * sZ + cX * cZ;
            float r23 = sX * cY;

            float r31 = cX * sY * cZ + sX * sZ;
            float r32 = cX * sY * sZ - sX * cZ;
            float r33 = cX * cY;

            Vector3D<float> newExtent = new Vector3D<float>(
                MathF.Abs(r11) * extent.X + MathF.Abs(r12) * extent.Y + MathF.Abs(r13) * extent.Z,
                MathF.Abs(r21) * extent.X + MathF.Abs(r22) * extent.Y + MathF.Abs(r23) * extent.Z,
                MathF.Abs(r31) * extent.X + MathF.Abs(r32) * extent.Y + MathF.Abs(r33) * extent.Z
                );

            newMin = centre - newExtent;
            newMax = centre + newExtent;
        }

    }
}
