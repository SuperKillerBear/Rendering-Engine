using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace RenderingEngine.Meshes
{
    public class TriangleMesh : Mesh
    {
        public TriangleMesh()
            : base(
                vertices: new float[]
                {
                    // Top Centre, Red
                    0.0f,  0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   0f,0f,
                    // Left Bottom, Green
                    -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,   0f,0f,
                    // Right Bottom, Blue
                    0.5f, -0.5f, 0.0f,  0.0f, 0.0f, 1.0f,   0f,0f
                },
                indices: new uint[]
                {
                    2, 1, 0
                })
        {
            // Optional: any extra initialization
        }
    }
}
