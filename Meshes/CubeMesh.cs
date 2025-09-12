using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;

namespace RenderingEngine.Meshes
{
    public class CubeMesh : Mesh
    {
        public CubeMesh(GL gl)
            : base(gl,
                vertices: new Vertex[]
                {
                    // Front face (z = +0.5)
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f,  0.5f), new Vector3D<float>(1.00f, 0.00f, 0.00f), new Vector2D<float>(0f,0f)), // BL - red
                    new Vertex(new Vector3D<float>( 0.5f, -0.5f,  0.5f), new Vector3D<float>(1.00f, 0.50f, 0.00f), new Vector2D<float>(0f,0f)), // BR - orange
                    new Vertex(new Vector3D<float>( 0.5f,  0.5f,  0.5f), new Vector3D<float>(1.00f, 1.00f, 0.00f), new Vector2D<float>(0f,0f)), // TR - yellow
                    new Vertex(new Vector3D<float>(-0.5f,  0.5f,  0.5f), new Vector3D<float>(1.00f, 0.20f, 0.60f), new Vector2D<float>(0f,0f)), // TL - pink

                    // Back face (z = -0.5)
                    new Vertex(new Vector3D<float>( 0.5f, -0.5f, -0.5f), new Vector3D<float>(0.00f, 1.00f, 0.00f), new Vector2D<float>(0f,0f)), // BL - green
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f, -0.5f), new Vector3D<float>(0.50f, 1.00f, 0.00f), new Vector2D<float>(0f,0f)), // BR - lime
                    new Vertex(new Vector3D<float>(-0.5f,  0.5f, -0.5f), new Vector3D<float>(0.20f, 0.60f, 0.10f), new Vector2D<float>(0f,0f)), // TR - olive
                    new Vertex(new Vector3D<float>( 0.5f,  0.5f, -0.5f), new Vector3D<float>(0.00f, 0.60f, 0.60f), new Vector2D<float>(0f,0f)), // TL - teal

                    // Left face
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f, -0.5f), new Vector3D<float>(0.00f, 0.00f, 1.00f), new Vector2D<float>(0f,0f)), // BL - blue
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f,  0.5f), new Vector3D<float>(0.00f, 0.00f, 0.50f), new Vector2D<float>(0f,0f)), // BR - navy
                    new Vertex(new Vector3D<float>(-0.5f,  0.5f,  0.5f), new Vector3D<float>(0.30f, 0.70f, 1.00f), new Vector2D<float>(0f,0f)), // TR - sky
                    new Vertex(new Vector3D<float>(-0.5f,  0.5f, -0.5f), new Vector3D<float>(0.00f, 1.00f, 1.00f), new Vector2D<float>(0f,0f)), // TL - cyan

                    // Right face
                    new Vertex(new Vector3D<float>( 0.5f, -0.5f,  0.5f), new Vector3D<float>(1.00f, 0.00f, 1.00f), new Vector2D<float>(0f,0f)), // BL - magenta
                    new Vertex(new Vector3D<float>( 0.5f, -0.5f, -0.5f), new Vector3D<float>(0.60f, 0.00f, 0.60f), new Vector2D<float>(0f,0f)), // BR - purple
                    new Vertex(new Vector3D<float>( 0.5f,  0.5f, -0.5f), new Vector3D<float>(0.70f, 0.30f, 1.00f), new Vector2D<float>(0f,0f)), // TR - violet
                    new Vertex(new Vector3D<float>( 0.5f,  0.5f,  0.5f), new Vector3D<float>(1.00f, 0.60f, 0.80f), new Vector2D<float>(0f,0f)), // TL - light pink

                    // Top face
                    new Vertex(new Vector3D<float>(-0.5f,  0.5f,  0.5f), new Vector3D<float>(1.00f, 0.84f, 0.00f), new Vector2D<float>(0f,0f)), // BL - gold
                    new Vertex(new Vector3D<float>( 0.5f,  0.5f,  0.5f), new Vector3D<float>(1.00f, 1.00f, 0.60f), new Vector2D<float>(0f,0f)), // BR - light yellow
                    new Vertex(new Vector3D<float>( 0.5f,  0.5f, -0.5f), new Vector3D<float>(1.00f, 0.40f, 0.40f), new Vector2D<float>(0f,0f)), // TR - salmon
                    new Vertex(new Vector3D<float>(-0.5f,  0.5f, -0.5f), new Vector3D<float>(1.00f, 0.50f, 0.40f), new Vector2D<float>(0f,0f)), // TL - coral

                    // Bottom face
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f, -0.5f), new Vector3D<float>(0f, 0f, 0f), new Vector2D<float>(0f,0f)),
                    new Vertex(new Vector3D<float>( 0.5f, -0.5f, -0.5f), new Vector3D<float>(0f, 0f, 0f), new Vector2D<float>(0f,0f)),
                    new Vertex(new Vector3D<float>( 0.5f, -0.5f,  0.5f), new Vector3D<float>(0f, 0f, 0f), new Vector2D<float>(0f,0f)),
                    new Vertex(new Vector3D<float>(-0.5f, -0.5f,  0.5f), new Vector3D<float>(0f, 0f, 0f), new Vector2D<float>(0f,0f))
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
