using RenderingEngine.Meshes;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Objects
{
    public class Quad : DynamicObject
    {
        public Quad(GL gl)
            : base(new QuadMesh(gl))
        {
            this.name = "Quad";
            //Any Additional Things here
        }


    }
}
