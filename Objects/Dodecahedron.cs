using RenderingEngine.Meshes;
using RenderingEngine.Objects;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Objects
{
    public class Dodecahedron : DynamicObject
    {
        public Dodecahedron(GL gl)
            : base(new DodecahedronMesh(gl))
        {
            //Any Additional Things here
        }

    }
}
