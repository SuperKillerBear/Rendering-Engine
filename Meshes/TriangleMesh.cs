using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace RenderingEngine.Meshes
{
    public class TriangleMesh : Mesh
    {
        public TriangleMesh(GL gl)
            : base(gl,
                vertices: new Vertex[]
                {
                    // Top Centre, Red
                    new Vertex(new Vector3D<float>(0.0f,  0.5f, 0.0f), new Vector3D<float>(1.0f, 0.0f, 0.0f), new Vector2D<float>(0f,0f)),
                    // Left Bottom, Green
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f, 0.0f), new Vector3D<float>(0.0f, 1.0f, 0.0f), new Vector2D<float>(0f,0f)),
                    // Right Bottom, Blue
                    new Vertex(new Vector3D<float>(0.5f, -0.5f, 0.0f), new Vector3D<float>(0.0f, 0.0f, 1.0f), new Vector2D<float>(0f,0f))
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
