using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;

namespace RenderingEngine.Meshes
{
    public class CubeMesh : Mesh
    {
        public CubeMesh()
            : base(
                vertices: new float[]
                {
                    -0.5f, -0.5f,  0.5f, 1.00f, 0.00f, 0.00f, 0f, 0f,
                     0.5f, -0.5f,  0.5f, 1.00f, 0.50f, 0.00f, 0f, 0f,
                     0.5f,  0.5f,  0.5f, 1.00f, 1.00f, 0.00f, 0f, 0f,
                    -0.5f,  0.5f,  0.5f, 1.00f, 0.20f, 0.60f, 0f, 0f,

                     0.5f, -0.5f, -0.5f, 0.00f, 1.00f, 0.00f, 0f, 0f,
                    -0.5f, -0.5f, -0.5f, 0.50f, 1.00f, 0.00f, 0f, 0f,
                    -0.5f,  0.5f, -0.5f, 0.20f, 0.60f, 0.10f, 0f, 0f,
                     0.5f,  0.5f, -0.5f, 0.00f, 0.60f, 0.60f, 0f, 0f,

                    -0.5f, -0.5f, -0.5f, 0.00f, 0.00f, 1.00f, 0f, 0f,
                    -0.5f, -0.5f,  0.5f, 0.00f, 0.00f, 0.50f, 0f, 0f,
                    -0.5f,  0.5f,  0.5f, 0.30f, 0.70f, 1.00f, 0f, 0f,
                    -0.5f,  0.5f, -0.5f, 0.00f, 1.00f, 1.00f, 0f, 0f,

                     0.5f, -0.5f,  0.5f, 1.00f, 0.00f, 1.00f, 0f, 0f,
                     0.5f, -0.5f, -0.5f, 0.60f, 0.00f, 0.60f, 0f, 0f,
                     0.5f,  0.5f, -0.5f, 0.70f, 0.30f, 1.00f, 0f, 0f,
                     0.5f,  0.5f,  0.5f, 1.00f, 0.60f, 0.80f, 0f, 0f,

                    -0.5f,  0.5f,  0.5f, 1.00f, 0.84f, 0.00f, 0f, 0f,
                     0.5f,  0.5f,  0.5f, 1.00f, 1.00f, 0.60f, 0f, 0f,
                     0.5f,  0.5f, -0.5f, 1.00f, 0.40f, 0.40f, 0f, 0f,
                    -0.5f,  0.5f, -0.5f, 1.00f, 0.50f, 0.40f, 0f, 0f,

                    -0.5f, -0.5f, -0.5f, 0f, 0f, 0f, 0f, 0f,
                     0.5f, -0.5f, -0.5f, 0f, 0f, 0f, 0f, 0f,
                     0.5f, -0.5f,  0.5f, 0f, 0f, 0f, 0f, 0f,
                    -0.5f, -0.5f,  0.5f, 0f, 0f, 0f, 0f, 0f
                },

                indices: new uint[]
                {
                    // For each face: two triangles (BL,BR,TR) (BL,TR,TL)

                    // Front face (base index 0)
                    0, 1, 2,  0, 2, 3,

                    // Back face (base index 4)
                    4, 5, 6,  4, 6, 7,

                    // Left face (base index 8)
                    8, 9,10,  8,10,11,

                    // Right face (base index 12)
                   12,13,14, 12,14,15,

                    // Top face (base index 16)
                   16,17,18, 16,18,19,

                    // Bottom face (base index 20)
                   20,21,22, 20,22,23
                })
        {
            // Optional: any extra initialization
        }
    }
}
