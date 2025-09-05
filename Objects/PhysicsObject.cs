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
        public float Mass = 1;
        private float MassInv;
        public Vector3D<float> Velocity;
        public Vector3D<float> Acceleration;
        public Vector3D<float> Forces;

        private int bufferID;

        public PhysicsObject(Mesh mesh)
            : base(mesh) 
        {
            bufferID = PhysicsObjectsHandler.AddObj(this);
            MassInv = 1 / Mass;
        }


        public void TickPhysics(double DeltaTime)
        {
            float dt = (float) DeltaTime;
            Acceleration += Forces * MassInv;

            Velocity += Acceleration * dt;

            Position += Velocity * dt;
        }

        public void Destroy()
        {
            PhysicsObjectsHandler.RemoveObj(bufferID);
        }
    }
}
