using RenderingEngine.Rendering;
using Silk.NET.OpenGL;

namespace RenderingEngine.Meshes
{
    public class CubeMesh : Mesh
    {
        public CubeMesh(GL gl)
            : base(gl,
                vertices: new float[]
                {
                    // Front face (z = +0.5)  - vertices order: BL, BR, TR, TL
                    -0.5f, -0.5f,  0.5f,   1.00f, 0.00f, 0.00f, // BL - red
                     0.5f, -0.5f,  0.5f,   1.00f, 0.50f, 0.00f, // BR - orange
                     0.5f,  0.5f,  0.5f,   1.00f, 1.00f, 0.00f, // TR - yellow
                    -0.5f,  0.5f,  0.5f,   1.00f, 0.20f, 0.60f, // TL - pink

                    // Back face (z = -0.5)  - BL, BR, TR, TL  (viewed from outside -z)
                     0.5f, -0.5f, -0.5f,   0.00f, 1.00f, 0.00f, // BL - green
                    -0.5f, -0.5f, -0.5f,   0.50f, 1.00f, 0.00f, // BR - lime
                    -0.5f,  0.5f, -0.5f,   0.20f, 0.60f, 0.10f, // TR - olive
                     0.5f,  0.5f, -0.5f,   0.00f, 0.60f, 0.60f, // TL - teal

                    // Left face (x = -0.5) - BL, BR, TR, TL
                    -0.5f, -0.5f, -0.5f,   0.00f, 0.00f, 1.00f, // BL - blue
                    -0.5f, -0.5f,  0.5f,   0.00f, 0.00f, 0.50f, // BR - navy
                    -0.5f,  0.5f,  0.5f,   0.30f, 0.70f, 1.00f, // TR - sky
                    -0.5f,  0.5f, -0.5f,   0.00f, 1.00f, 1.00f, // TL - cyan

                    // Right face (x = +0.5) - BL, BR, TR, TL
                     0.5f, -0.5f,  0.5f,   1.00f, 0.00f, 1.00f, // BL - magenta
                     0.5f, -0.5f, -0.5f,   0.60f, 0.00f, 0.60f, // BR - purple
                     0.5f,  0.5f, -0.5f,   0.70f, 0.30f, 1.00f, // TR - violet
                     0.5f,  0.5f,  0.5f,   1.00f, 0.60f, 0.80f, // TL - light pink

                    // Top face (y = +0.5) - BL, BR, TR, TL
                    -0.5f,  0.5f,  0.5f,   1.00f, 0.84f, 0.00f, // BL - gold
                     0.5f,  0.5f,  0.5f,   1.00f, 1.00f, 0.60f, // BR - light yellow
                     0.5f,  0.5f, -0.5f,   1.00f, 0.40f, 0.40f, // TR - salmon
                    -0.5f,  0.5f, -0.5f,   1.00f, 0.50f, 0.40f, // TL - coral

                    // Bottom face (y = -0.5) - BL, BR, TR, TL
                    -0.5f, -0.5f, -0.5f,   0.60f, 0.30f, 0.10f, // BL - brown
                     0.5f, -0.5f, -0.5f,   0.80f, 0.60f, 0.40f, // BR - tan
                     0.5f, -0.5f,  0.5f,   0.96f, 0.96f, 0.86f, // TR - beige
                    -0.5f, -0.5f,  0.5f,   0.50f, 0.50f, 0.50f  // TL - gray
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
            // Optional: any extra initialization (none required)
        }
    }
}
