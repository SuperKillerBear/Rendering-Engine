using ImGuiNET;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System.ComponentModel;

namespace RenderingEngine.Components
{
    public class TransformComponent : Component, ISerializable
    {
        public override string ComponentName => "Transform Component";

        private ColliderComponent? collider; //TODO: Issue => if collider is added after transform, it will be null
        private bool isDirty = true;
        private bool dirtyScale = true;
        
        public Vector3D<float> worldScale = Vector3D<float>.One;

        public Vector3D<float> position { get; private set; }

        public Vector3D<float> scale { get; private set; } = Vector3D<float>.One;

        private Vector3D<float> _rotation;
        public Vector3D<float> rotation
        {
            get => _rotation;
            private set
            {
                _rotation = value;
                if (collider != null)
                    collider.CalcAABBMaxMins();
            }
        }


        private Vector3D<float> lastRotation;


        public Matrix4X4<float> ModelMatrix { get; set; }


        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner

            collider = Owner?.GetComponent<ColliderComponent>();
        }

        public void DirtyScale()
        {
            dirtyScale = true;
            foreach (var child in Owner.children)
            {
                child.Transform.DirtyScale();
            }
        }

        public Vector3D<float> GetWorldScale()
        {
            // Always recalculate if dirty
            if (dirtyScale)
            {
                var parent = Owner.parent;
                if (parent != null)
                {
                    // Make sure parent's worldScale is up to date before using it
                    var parentScale = parent.Transform.GetWorldScale();
                    worldScale = scale * parentScale;
                }
                else
                {
                    worldScale = scale; // root scale = local scale
                }

                dirtyScale = false;
            }

            return worldScale;
        }



        public Matrix4X4<float> GetModelMatrix()
        {
            if (isDirty) CalcModelMatrix();                        
            return ModelMatrix;
        }

        public void CalcModelMatrix()
        {
            if (!isDirty) return; //Maybe remove this if the FPS drain is too much

            this.ModelMatrix =
                Matrix4X4.CreateScale(GetWorldScale()) *
                Matrix4X4.CreateRotationX(rotation.X) *
                Matrix4X4.CreateRotationY(rotation.Y) *
                Matrix4X4.CreateRotationZ(rotation.Z) *
                Matrix4X4.CreateTranslation(position);
            isDirty = false;
        }

        public void SetScale(Vector3D<float> mag)
        {
            this.scale = mag;
            isDirty = true;
            DirtyScale();
        }

        public void SetRotation(Vector3D<float> mag)
        {
            this.rotation = mag;
            isDirty = true;
        }

        public void SetPositon(Vector3D<float> mag)
        {
            this.position = mag;
            isDirty = true;
        }

        public void Scale(Vector3D<float> mag)
        {
            this.scale *= mag;
            isDirty = true;
            DirtyScale();            
        }

        public void Rotate(Vector3D<float> mag)
        {
            this.rotation += mag;
            isDirty = true;
        }

        public void Translate(Vector3D<float> mag)
        {
            this.position += mag;
            isDirty = true;
        }

        public override void OnInspectorGUI()
        {
            ImGuiNET.ImGui.Text("Transform Component");
            
            ImGui.Text($"Position: {position}");
            ImGui.Text($"Rotation: {_rotation}");
            ImGui.Text($"Scale: {scale}");
            if (ImGui.Button("Scale*1.1")) { Scale(new Vector3D<float>(1.1f)); }
            if (ImGui.Button("Pos?")) { Console.WriteLine($"Position: {position.ToString()}"); }

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(position.Z);

            writer.Write(rotation.X);
            writer.Write(rotation.Y);
            writer.Write(rotation.Z);

            writer.Write(scale.X);
            writer.Write(scale.Y);
            writer.Write(scale.Z);
        }

        public void Deserialize(BinaryReader reader)
        {
            var loadedPos = new Vector3D<float>();
            loadedPos.X = reader.ReadSingle();
            loadedPos.Y = reader.ReadSingle();
            loadedPos.Z = reader.ReadSingle();

            Translate(loadedPos);

            _rotation.X = reader.ReadSingle();
            _rotation.Y = reader.ReadSingle();
            _rotation.Z = reader.ReadSingle();

            var loadedScale = new Vector3D<float>();
            loadedScale.X = reader.ReadSingle();
            loadedScale.Y = reader.ReadSingle();
            loadedScale.Z = reader.ReadSingle();

            Scale(loadedScale);
        }
    }
}

