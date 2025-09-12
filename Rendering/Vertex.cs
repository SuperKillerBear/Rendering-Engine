using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3D<float> pos;
        public Vector3D<float> colour;
        public Vector2D<float> uv;

        
        public Vertex(Vector3D<float> xyz, Vector3D<float> rgb, Vector2D<float> UV)
        {
            pos = xyz;
            colour = rgb;
            uv = UV;
        }

    }
}
