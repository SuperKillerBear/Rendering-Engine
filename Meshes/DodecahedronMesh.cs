using RenderingEngine.Rendering;
using Silk.NET.OpenGL;

namespace RenderingEngine.Meshes
{
    public class DodecahedronMesh : Mesh
    {
        public DodecahedronMesh(GL gl)
            : base(gl,
                vertices: new float[]
                {
                    // Straight up just fucking wrong ._.
                    0.0f,  0.5f,  0.0f,   1f, 0f, 0f,
                   -0.5f,  0.0f,  0.5f,   1f, 0f, 0f,
                    0.5f,  0.0f,  0.5f,   1f, 0f, 0f,
                    0.5f,  0.0f, -0.5f,   1f, 0f, 0f,
                   -0.5f,  0.0f, -0.5f,   1f, 0f, 0f,
                    0.0f, -0.5f,  0.0f,   1f, 0f, 0f,

                    // Face 2 (Green)
                    0.0f,  0.5f,  0.0f,   0f, 1f, 0f,
                    0.5f,  0.0f,  0.5f,   0f, 1f, 0f,
                    0.5f,  0.0f, -0.5f,   0f, 1f, 0f,
                    0.0f, -0.5f,  0.0f,   0f, 1f, 0f,
                   -0.5f,  0.0f, -0.5f,   0f, 1f, 0f,
                   -0.5f,  0.0f,  0.5f,   0f, 1f, 0f,

                    // Face 3 (Blue)
                    -0.5f,  0.0f,  0.5f,  0f, 0f, 1f,
                    0.0f,  0.5f,  0.0f,   0f, 0f, 1f,
                    0.5f,  0.0f,  0.5f,   0f, 0f, 1f,
                    0.0f, -0.5f,  0.0f,   0f, 0f, 1f,
                    -0.5f,  0.0f, -0.5f,  0f, 0f, 1f,

                    // Face 4 (Yellow)
                    0.5f,  0.0f, -0.5f,   1f, 1f, 0f,
                    0.0f,  0.5f,  0.0f,   1f, 1f, 0f,
                    0.0f, -0.5f,  0.0f,   1f, 1f, 0f,
                    -0.5f,  0.0f, -0.5f,  1f, 1f, 0f,
                    0.5f,  0.0f,  0.5f,   1f, 1f, 0f,

                    // Face 5 (Cyan)
                    -0.5f,  0.0f,  0.5f,  0f, 1f, 1f,
                    0.0f,  0.5f,  0.0f,   0f, 1f, 1f,
                    0.0f, -0.5f,  0.0f,   0f, 1f, 1f,
                    0.5f,  0.0f,  0.5f,   0f, 1f, 1f,
                    0.5f,  0.0f, -0.5f,   0f, 1f, 1f,

                    // Face 6 (Magenta)
                    0.0f,  0.5f,  0.0f,   1f, 0f, 1f,
                    0.5f,  0.0f,  0.5f,   1f, 0f, 1f,
                    0.5f,  0.0f, -0.5f,   1f, 0f, 1f,
                    0.0f, -0.5f,  0.0f,   1f, 0f, 1f,
                    -0.5f,  0.0f, -0.5f,  1f, 0f, 1f,

                    // Face 7 (Orange)
                    -0.5f,  0.0f,  0.5f,  1f, 0.5f, 0f,
                    0.0f,  0.5f,  0.0f,   1f, 0.5f, 0f,
                    0.5f,  0.0f,  0.5f,   1f, 0.5f, 0f,
                    0.0f, -0.5f,  0.0f,   1f, 0.5f, 0f,
                    -0.5f,  0.0f, -0.5f,  1f, 0.5f, 0f,

                    // Face 8 (Pink)
                    0.0f,  0.5f,  0.0f,   1f, 0.75f, 0.8f,
                    0.5f,  0.0f, -0.5f,   1f, 0.75f, 0.8f,
                    0.0f, -0.5f,  0.0f,   1f, 0.75f, 0.8f,
                    -0.5f,  0.0f, -0.5f,  1f, 0.75f, 0.8f,
                    -0.5f,  0.0f,  0.5f,  1f, 0.75f, 0.8f,

                    // Face 9 (LightGreen)
                    0.0f,  0.5f,  0.0f,   0.5f, 1f, 0.5f,
                    0.5f,  0.0f,  0.5f,   0.5f, 1f, 0.5f,
                    0.5f,  0.0f, -0.5f,   0.5f, 1f, 0.5f,
                    0.0f, -0.5f,  0.0f,   0.5f, 1f, 0.5f,
                    -0.5f,  0.0f, -0.5f,  0.5f, 1f, 0.5f,

                    // Face 10 (LightBlue)
                    -0.5f,  0.0f,  0.5f,  0.5f, 0.5f, 1f,
                    0.0f,  0.5f,  0.0f,   0.5f, 0.5f, 1f,
                    0.5f,  0.0f,  0.5f,   0.5f, 0.5f, 1f,
                    0.0f, -0.5f,  0.0f,   0.5f, 0.5f, 1f,
                    -0.5f,  0.0f, -0.5f,  0.5f, 0.5f, 1f,

                    // Face 11 (Brown)
                    0.0f,  0.5f,  0.0f,   0.6f, 0.3f, 0.1f,
                    0.5f,  0.0f, -0.5f,   0.6f, 0.3f, 0.1f,
                    0.0f, -0.5f,  0.0f,   0.6f, 0.3f, 0.1f,
                    -0.5f,  0.0f, -0.5f,  0.6f, 0.3f, 0.1f,
                    -0.5f,  0.0f,  0.5f,  0.6f, 0.3f, 0.1f,

                    // Face 12 (Gray)
                    0.5f,  0.0f,  0.5f,   0.5f, 0.5f, 0.5f,
                    0.0f,  0.5f,  0.0f,   0.5f, 0.5f, 0.5f,
                    0.0f, -0.5f,  0.0f,   0.5f, 0.5f, 0.5f,
                    0.5f,  0.0f, -0.5f,   0.5f, 0.5f, 0.5f,
                    -0.5f,  0.0f, -0.5f,  0.5f, 0.5f, 0.5f,
                },
                indices: new uint[]
                {
                    // Each pentagonal face split into 3 triangles
                    0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5,   // Face 1
                    6, 7, 8, 6, 8, 9, 6, 9, 10, 6, 10, 11, // Face 2
                    12, 13, 14, 12, 14, 15, 12, 15, 16,     // Face 3
                    17, 18, 19, 17, 19, 20, 17, 20, 21,     // Face 4
                    22, 23, 24, 22, 24, 25, 22, 25, 26,     // Face 5
                    27, 28, 29, 27, 29, 30, 27, 30, 31,     // Face 6
                    32, 33, 34, 32, 34, 35, 32, 35, 36,     // Face 7
                    37, 38, 39, 37, 39, 40, 37, 40, 41,     // Face 8
                    42, 43, 44, 42, 44, 45, 42, 45, 46,     // Face 9
                    47, 48, 49, 47, 49, 50, 47, 50, 51,     // Face 10
                    52, 53, 54, 52, 54, 55, 52, 55, 56,     // Face 11
                    57, 58, 59, 57, 59, 60, 57, 60, 61      // Face 12
                })
        {
            // Optional extra initialization
        }
    }
}
