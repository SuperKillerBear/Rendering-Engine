using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderingEngine.GameObjects;
using ImGuiNET;

namespace RenderingEngine.Components
{
    public class TransformComponent : Component, ISerializable
    {
        public override string ComponentName => "Transform Component";

        private ColliderComponent? collider; //TODO: Issue => if collider is added after transform, it will be null

        public Vector3D<float> Position;

        public Vector3D<float> Scale = Vector3D<float>.One;

        private Vector3D<float> rotation;
        public Vector3D<float> Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (collider != null)
                    collider.CalcAABBMaxMins();
            }
        }

        public ref Vector3D<float> RotationRef => ref rotation;

        private Vector3D<float> lastRotation;


        public Matrix4X4<float> ModelMatrix { get; set; }


        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner

            collider = owner?.GetComponent<ColliderComponent>();
        }


        public void UpdateModelMatrix()
        {
            this.ModelMatrix =
                Matrix4X4.CreateScale(Scale) *
                Matrix4X4.CreateRotationX(Rotation.X) *
                Matrix4X4.CreateRotationY(Rotation.Y) *
                Matrix4X4.CreateRotationZ(Rotation.Z) *
                Matrix4X4.CreateTranslation(Position);
        }


        public override void OnInspectorGUI()
        {
            ImGuiNET.ImGui.Text("Transform Component");

            InputVector3D("Position", ref Position);
            InputVector3D("Rotation", ref RotationRef);
            InputVector3D("Scale", ref Scale);

            if (ImGui.Button("Pos?")) { Console.WriteLine($"Position: {Position.ToString()}"); }

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);

            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);

            writer.Write(Scale.X);
            writer.Write(Scale.Y);
            writer.Write(Scale.Z);
        }

        public void Deserialize(BinaryReader reader)
        {
            Position.X = reader.ReadSingle();
            Position.Y = reader.ReadSingle();
            Position.Z = reader.ReadSingle();

            rotation.X = reader.ReadSingle();
            rotation.Y = reader.ReadSingle();
            rotation.Z = reader.ReadSingle();

            Scale.X = reader.ReadSingle();
            Scale.Y = reader.ReadSingle();
            Scale.Z = reader.ReadSingle();
        }
    }
}
