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

        public LoadedDynamicObject(GL gl, Mesh mesh, Vector3D<float> scale)
            : base(mesh)   // calls the base constructor that accepts Mesh
        {
            _gl = gl;

            Scale = scale;
            // any extra initialization
        }
    }
}

