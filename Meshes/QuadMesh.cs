using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Meshes
{
    internal class QuadMesh : Mesh
    {
        public QuadMesh()
            : base(
                vertices: new float[]
                {
                     0.5f, -0.5f, 0.0f,     0.8f, 0.0f, 0.9f,   0f,0f,
                    -0.5f, -0.5f, 0.0f,     0.1f, 0.7f, 0.9f,   0f,0f,
                    -0.5f,  0.5f, 0.0f,     1.0f, 0.7f, 0.0f,   0f,0f,
                     0.5f,  0.5f, 0.0f,     0.1f, 0.1f, 0.9f,   0f,0f
                },
                indices: new uint[]
                {
                    0, 1, 2, //Bottom Left Triangle
                    0, 2, 3  //Top Right Triangle
                })
        {
            // Optional: any extra initialization
        }
    }
}
