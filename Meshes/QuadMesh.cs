using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderingEngine.Rendering;

namespace RenderingEngine.Meshes
{
    internal class QuadMesh : Mesh
    {
        public QuadMesh(GL gl)
            : base(gl,
                vertices: new float[]
                {
                     0.5f, -0.5f, 0.0f,     0.8f, 0.0f, 0.9f,   //Bottom Right
                    -0.5f, -0.5f, 0.0f,     0.1f, 0.7f, 0.9f,   //Bottom Left
                    -0.5f,  0.5f, 0.0f,     1.0f, 0.7f, 0.0f,   //Top Left
                     0.5f,  0.5f, 0.0f,     0.1f, 0.1f, 0.9f    //Top Right
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
