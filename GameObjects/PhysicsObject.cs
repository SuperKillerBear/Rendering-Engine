using RenderingEngine.Rendering;
using RenderingEngine.Utilities;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.GameObjects
{
    public class PhysicsObject : DynamicObject
    {
        public float mass = 10;
        public float massInv;

        public float restitution = 1f;
        private float g = 9.81f;

        public bool applyGravity = true;
        public bool tickPhysics = false;

        public Vector3D<float> ForceAccum = Vector3D<float>.Zero;
        public Vector3D<float> Acceleration = Vector3D<float>.Zero;
        public Vector3D<float> Velocity = Vector3D<float>.Zero;

        public PhysicsObject(Mesh mesh) :
            base(mesh)
        {
            this.name = "Physics Cube";

            massInv = 1 / mass;
            Acceleration.Y = -g;

            //PhysicsObjectsHandler.AddObj(this);
        }

        /*
        public void TickPhysics(double deltaTime)
        {
            if (!tickPhysics) return;

            float dt = (float) deltaTime * Program.tickRate;

            //Apply Gravity
            if (applyGravity) ForceAccum.Y -= g * mass;

            //TODO: Collision Checks, etc
            if (Position.Y <= 0.5 * Scale.Y && Velocity.Y < 0)
        {
                Velocity.Y *= (float) -restitution;
            }
            

            //Apply Force to Velocity   F = MA
            Velocity += ForceAccum * massInv * dt;

            //Update Position Accordingly
            Position += Velocity * dt;            
            
            
            //Clear Forces at the end
            ForceAccum = Vector3D<float>.Zero;

            this.CheckCollisions();
        }



        public void CheckCollisions()
        {
            this.CalcChunks();

            //Check Collisions with other Physics Objects in same chunks

            //Flawed Logic as they may not have calced now physics chuncks
            for (int i = 0; i < Renderer.dynObjs.Length; i++)
            {
                for (int c = 0; c < chunks.Count; c++)
                {
                    if (Renderer.dynObjs[i] != null && Renderer.dynObjs[i].chunks.Contains(chunks[c]) && Renderer.dynObjs[i] != this)
                    {
                        var resultant = this.CalcCollisions(Renderer.dynObjs[i]);

                        if (resultant != Vector3D<float>.Zero && (UMath.Dot(new Vector3D<float>(0, 1, 0), Velocity) < 0))
                        {
                            //Simple Collision Response
                            Velocity *= -restitution;
                        }
                    }
                }
            }




        }
    */
    }
}
