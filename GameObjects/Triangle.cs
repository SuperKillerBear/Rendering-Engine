using RenderingEngine.Rendering;
using RenderingEngine.Meshes;
using Silk.NET.OpenGL;

namespace RenderingEngine.GameObjects
{
    public class Triangle : DynamicObject
    {
        public Triangle(GL gl) 
            : base(new TriangleMesh(gl))
        {
            //Any Additional Things here
        }

    }
}
