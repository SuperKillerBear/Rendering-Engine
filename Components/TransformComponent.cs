using ImGuiNET;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System.ComponentModel;
using System.Numerics;

namespace RenderingEngine.Components
{
    public class TransformComponent : Component, ISerializable
    {
        public override string ComponentName => "Transform Component";

        private ColliderComponent? collider; //TODO: Issue => if collider is added after transform, it will be null
        private bool isDirty = true;
        private bool dirtyScale = true;
        
        
        public Vector3D<float> position { get; private set; }
        public Vector3D<float> scale { get; private set; } = Vector3D<float>.One;

        private Vector3D<float> _rotation;
        public Vector3D<float> rotation
        {
            get => _rotation;
            private set
            {
                _rotation = value;
                //if (collider != null)
                 //   (collider as BoxColliderComponent).CalculateAABB();
            }
        }

        public Vector3D<float> worldScale = Vector3D<float>.One;
        public Matrix4X4<float> ModelMatrix { get; set; }
        


        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner
            collider = Owner?.GetComponent<ColliderComponent>();
            MarkDirtyRecursive();
        }
        
        private void MarkDirtyRecursive()
        {
            isDirty = true;
            dirtyScale = true;

            foreach (var child in Owner.children)
            {
                child.Transform.MarkDirtyRecursive();
            }
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
            if (!isDirty) return;
            
            
            Matrix4X4<float> local =
                Matrix4X4.CreateScale(scale) *
                Matrix4X4.CreateRotationX(rotation.X) *
                Matrix4X4.CreateRotationY(rotation.Y) *
                Matrix4X4.CreateRotationZ(rotation.Z) *
                Matrix4X4.CreateTranslation(position);
            
            if (Owner.parent != null)
            {
                Matrix4X4<float> parentWorld = Owner.parent.Transform.GetModelMatrix();
                ModelMatrix = parentWorld * local;
            }
            else
            {
                ModelMatrix = local;
            }
            
            
            Console.WriteLine($"pos={position}  M14/M24/M34={ModelMatrix.M14},{ModelMatrix.M24},{ModelMatrix.M34}  M41/M42/M43={ModelMatrix.M41},{ModelMatrix.M42},{ModelMatrix.M43}");

            
            isDirty = false;
        }

        public void SetScale(Vector3D<float> mag)
        {
            scale = mag;
            MarkDirtyRecursive();
        }

        public void SetRotation(Vector3D<float> mag)
        {
            rotation = mag;
            MarkDirtyRecursive();
        }

        public void SetPositon(Vector3D<float> mag)
        {
            this.position = mag;
            MarkDirtyRecursive();
        }

        public void Scale(Vector3D<float> mag)
        {
            this.scale *= mag;
            MarkDirtyRecursive();
        }

        public void Rotate(Vector3D<float> mag)
        {
            this.rotation += mag;
            MarkDirtyRecursive();
        }

        public void Translate(Vector3D<float> mag)
        {
            this.position += mag;
            MarkDirtyRecursive();
        }

        public override void OnInspectorGUI()
        {
            ImGuiNET.ImGui.Text("Transform Component");
            
            Vector3 pos = new(position.X, position.Y, position.Z);
            Vector3 rot = new(rotation.X, rotation.Y, rotation.Z);
            Vector3 scl = new(scale.X, scale.Y, scale.Z);
            
            var wm = GetModelMatrix();
            var worldPos = new Vector3D<float>(wm.M41, wm.M42, wm.M43);
            ImGui.Text($"World Position: {worldPos}");
            
            if (ImGui.InputFloat3("Position", ref pos))
            {
                SetPositon(new Vector3D<float>(pos.X, pos.Y, pos.Z));
            }
            if (ImGui.InputFloat3("Rotation", ref rot))
            {
                SetRotation(new Vector3D<float>(rot.X, rot.Y, rot.Z));
            }
            if (ImGui.InputFloat3("Scale", ref scl))
            {
                SetScale(new Vector3D<float>(scl.X, scl.Y, scl.Z));
            }
            if (ImGui.Button("Up 2y"))
            {
                Translate(new Vector3D<float>(0f, 2f, 0f));
            }
            //ImGui.Text($"Position: {position}");
            //ImGui.Text($"Rotation: {_rotation}");
            //ImGui.Text($"Scale: {scale}");
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
            SetPositon(loadedPos);
            
            var loadedRot = new Vector3D<float>();
            loadedRot.X = reader.ReadSingle();
            loadedRot.Y = reader.ReadSingle();
            loadedRot.Z = reader.ReadSingle();
            SetRotation(loadedRot);

            var loadedScale = new Vector3D<float>();
            loadedScale.X = reader.ReadSingle();
            loadedScale.Y = reader.ReadSingle();
            loadedScale.Z = reader.ReadSingle();
            SetScale(loadedScale);
        }
    }
}

