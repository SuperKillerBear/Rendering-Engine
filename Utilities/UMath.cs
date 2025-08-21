using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Utilities
{
    public static class UMath
    {
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
    }
}
