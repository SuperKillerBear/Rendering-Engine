using RenderingEngine.Rendering;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Objects
{
    public class PhysicsObject : DynamicObject
    {
        public Vector3D<float> Velocity;
        public Vector3D<float> Acceleration;


        public PhysicsObject(Mesh mesh)
            : base(mesh) 
        {
            
        }
    }
}
