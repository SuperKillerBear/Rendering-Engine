using RenderingEngine.Meshes;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.GameObjects
{
    public class Cube : DynamicObject
    {
        public Cube(GL gl)
            : base(new CubeMesh(gl))
        {
            //Any Additional Things here
            this.name = "Cube";
        }
    }
}
