using RenderingEngine.Rendering;
using Silk.NET.OpenGL;

namespace RenderingEngine.Meshs
{
    public class TriangleMesh : Mesh
    {
        public TriangleMesh(GL gl)
            : base(gl,
                vertices: new float[]
                {
                    0.0f,  0.5f, 0.0f,      1.0f, 0.0f, 0.0f, // Top Centre, Red
                   -0.5f, -0.5f, 0.0f,      0.0f, 1.0f, 0.0f, // Left Bottom, Green
                    0.5f, -0.5f, 0.0f,      0.0f, 0.0f, 1.0f  // Right Bottom, Blue
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
