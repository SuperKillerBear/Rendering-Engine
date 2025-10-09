using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
using RenderingEngine.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RenderingEngine.Components
{
    public class RigidBodyComponent : ColliderComponent
    {
        private TransformComponent? transform;

        private float _mass = 10;
        public float mass
        {
            get => _mass;
            set
            {
                massInv = 1 / value;
                _mass = value;
            }
        }
        public ref float Rmass => ref _mass;

        public float massInv = 1 / 10;

        public float restitution = 1f;
        private float g = 9.81f;

        public bool applyGravity = true;
        public bool tickPhysics = false;

        public Vector3D<float> ForceAccum = Vector3D<float>.Zero;
        public Vector3D<float> Acceleration = Vector3D<float>.Zero;
        public Vector3D<float> Velocity = Vector3D<float>.Zero;

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner

            //Owner will not be null here
            this.transform = owner.GetComponent<TransformComponent>();
            

            PhysicsObjectsHandler.AddObj(this);
        }

        public void TickPhysics(double deltaTime)
        {
            if (!tickPhysics || transform == null) return;


            float dt = (float)deltaTime * Program.tickRate;

            //Apply Gravity
            if (applyGravity) ForceAccum.Y -= g * mass;

            //TODO: Collision Checks, etc
            if (transform.Position.Y <= 0.5 * transform.Scale.Y && Velocity.Y < 0)
            {
                Velocity.Y *= (float)-restitution;
            }


            //Apply Force to Velocity   F = MA
            Velocity += ForceAccum * massInv * dt;

            //Update Position Accordingly
            transform.Position += Velocity * dt;


            //Clear Forces at the end
            ForceAccum = Vector3D<float>.Zero;

            this.CheckCollisions();
        }


        //TODO UPDATE FOR ECS
        public void CheckCollisions()
        {
            this.CalcChunks();

            //Check Collisions with other Physics Objects in same chunks

            //Flawed Logic as they may not have calced now physics chuncks
            foreach (RendererComponent obj in Renderer.RenderingObjects)
            {
                for (int c = 0; c < chunks.Count; c++)
                {
                    var collider = obj.owner.GetComponent<ColliderComponent>();

                    if (collider == null) continue;

                    if (collider.chunks.Contains(chunks[c]) && collider as RigidBodyComponent != this)
                    {
                        var resultant = this.CalcCollisions(collider);

                        if (resultant != Vector3D<float>.Zero && (UMath.Dot(new Vector3D<float>(0, 1, 0), Velocity) < 0))
                        {
                            //Simple Collision Response
                            Velocity *= -restitution;
                        }
                    }
                }
            }




        }

    }
}
