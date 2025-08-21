using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Objects
{
    public  class LoadedDynamicObject : DynamicObject
    {
        private readonly GL _gl;

        public LoadedDynamicObject(GL gl, Mesh mesh)
            : base(mesh)   // calls the base constructor that accepts Mesh
        {
            _gl = gl;
            // any extra initialization
        }
    }
}

