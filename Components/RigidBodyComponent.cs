using ImGuiNET;
using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
using RenderingEngine.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RenderingEngine.Components
{
    public class RigidBodyComponent : Component, ISerializable
    {
        public override string ComponentName => "RigidBody Component";

        private float _mass = 10;
        public float mass
        {
            get => _mass;
            set
            {
                _mass = value;
                massInv = (float)1 / _mass;
            }
        }
        public ref float Rmass => ref _mass;

        public float massInv = (float) 1 / 10;

        public float restitution = 1f;
        private float g = 9.81f;

        public bool applyGravity = true;
        public bool tickPhysics = false;


        public Vector3D<float> ForceAccum = Vector3D<float>.Zero;
        public Vector3D<float> Acceleration = Vector3D<float>.Zero;
        public Vector3D<float> Velocity = Vector3D<float>.Zero;

        public ColliderComponent? Collider {get; private set; }


        private Vector3D<float> Resultant = Vector3D<float>.Zero;

        private int physObjListIndex = -1;

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner

            physObjListIndex = PhysicsObjectsHandler.AddObj(this);

            Collider = Owner.GetComponent<ColliderComponent>();
            if (Collider == null)
                Collider = Owner.AddComponent<BoxColliderComponent>();
        }

        public void TickPhysics(double deltaTime)
        {
            if (!tickPhysics) return;

            float dt = (float)deltaTime * Program.tickRate;

            //Apply Gravity
            if (applyGravity) this.ForceAccum.Y -= g * mass;

            //TODO: Collision Checks, etc
            if (Owner.Transform.position.Y <= 0.5 * Owner.Transform.scale.Y && this.Velocity.Y < 0)
            {
                this.Velocity.Y *= (float)-restitution;
            }


            //Apply Force to Velocity   F = MA
            //Velocity += ForceAccum * massInv * dt;
            Velocity += ForceAccum * 0.1f * dt;

            //Update Position Accordingly NOT WORKING
            Owner.Transform.Translate(Velocity * dt);


            //Clear Forces at the end
            ForceAccum = Vector3D<float>.Zero;

            this.CheckCollisions();
        }


        
        public void CheckCollisions()
        {
            return; //Fix Later
            Collider.CalcChunks();

            //Check Collisions with other Physics Objects in same chunks

            //Flawed Logic as they may not have calced now physics chuncks
            foreach (RendererComponent obj in Renderer.RenderingObjects)
            {
                for (int c = 0; c < Collider.chunks.Count; c++)
                {
                    var collider2 = obj.Owner.GetComponent<ColliderComponent>() as BoxColliderComponent;

                    if (collider2 == null) continue;

                    if (Collider.chunks.Contains(Collider.chunks[c]) && collider2 != Collider)
                    {
                        //Resultant = (collider as BoxColliderComponent).CollideAABBs(collider2); //Wont Be Null

                        //if (Resultant != Vector3D<float>.Zero && (UMath.Dot(new Vector3D<float>(0, 1, 0), Velocity) < 0))
                        //{
                            //Simple Collision Response
                        //    Velocity *= -restitution;
                        //}


                    }
                }
            }




        }


        public override void OnInspectorGUI()
        {
            ImGui.Text("RigidBody Component");
            ImGui.DragFloat("Mass", ref _mass, 0.1f, 0.1f, 1000f);

            ImGui.Text($"Acceleration: {Acceleration}");
            ImGui.Text($"Velocity: {Velocity}");

            ImGui.DragFloat("Restitution", ref restitution, 0.01f, 0f, 1f);
            ImGui.DragFloat("Gravity", ref g, 0.1f, 0f, 100f);
            
            ImGui.Text($"Resultant: {Resultant}");

            ImGui.Text($"Chunks: {string.Join(", ", Collider.chunks.Select(c => $"({c.X}, {c.Y})"))}");

            ImGui.Checkbox("Apply Gravity", ref applyGravity);
            ImGui.Checkbox("Tick Physics", ref tickPhysics);

            if (ImGui.Button("Update Calcs"))
            {
                CheckCollisions();
            }

        }
    
        
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(mass);
            writer.Write(restitution);
            writer.Write(g);
            writer.Write(applyGravity);
            writer.Write(tickPhysics);
        }
        
        public void Deserialize(BinaryReader reader)
        {
            mass = reader.ReadSingle();
            restitution = reader.ReadSingle();
            g = reader.ReadSingle();
            applyGravity = reader.ReadBoolean();
            tickPhysics = reader.ReadBoolean();
        }

        public override void Dispose()
        {
            PhysicsObjectsHandler.RemoveObj(this, physObjListIndex);
        }

    }
}
