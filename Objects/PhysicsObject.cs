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
    public class PhysicsObject : DynamicObject
    {
        public float mass = 10;
        public float massInv;

        public float restitution = 1f;
        private float g = 9.81f;

        public Vector3D<float> ForceAccum = Vector3D<float>.Zero;
        public Vector3D<float> Acceleration = Vector3D<float>.Zero;
        public Vector3D<float> Velocity = Vector3D<float>.Zero;

        public PhysicsObject(Mesh mesh) :
            base(mesh)
        {
            massInv = 1 / mass;
            Acceleration.Y = -g;
        }


        public void TickPhysics(double deltaTime)
        {
            float dt = (float) deltaTime;

            //Apply Gravity
            ForceAccum.Y -= g * mass;

            //TODO: Collision Checks, etc
            if (Position.Y <= 0 && Velocity.Y < 0)
            {
                Velocity.Y *= (float) -restitution;
            }
            

            //Apply Force to Velocity   F = MA
            Velocity += ForceAccum * massInv * dt;

            //Update Position Accordingly
            Position += Velocity * dt;            
            

            //Clear Forces at the end
            ForceAccum = Vector3D<float>.Zero;
        }
    }
}
