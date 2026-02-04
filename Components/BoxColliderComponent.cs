using ImGuiNET;
using RenderingEngine.GameObjects;
using RenderingEngine.Utilities;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class BoxColliderComponent : ColliderComponent, ISerializable
    {//TODO => WRITE CODE FOR THIS COMPONENET
        public override string ComponentName => "Box Collider Component";

        public Vector3D<float> Size = new Vector3D<float>(1f, 1f, 1f);
        public Vector3D<float> Centre = new Vector3D<float>(0f, 0f, 0f);

        public Vector3D<float> WorldMin { get; private set; }
		public Vector3D<float> WorldMax { get; private set; }
		public Vector3D<float> WorldCenter { get; private set; }
		public Vector3D<float> WorldHalfExtents { get; private set; }

        private readonly Vector3D<float>[] _localCorners = new Vector3D<float>[8];
		private readonly Vector3D<float>[] _worldCorners = new Vector3D<float>[8];

		private bool _dirty = true;
		private Matrix4X4<float> _lastModel;

        public bool IsColliding = false;


        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner
            RebuildLocalCorners();
            RecalculateWorldAabb();
            _lastModel = Owner.Transform.GetModelMatrix();

            BoxColliderHandler.AddObj(this);
            // Temp while testing all colliders, this makes all Box Colliders be checked ticked every frame
            // which is redundent if they are not moving!
        }
        
        public void TickCollider()
		{
            IsColliding = false;

			var model = Owner.Transform.GetModelMatrix();

			if (_dirty || !MatrixEqualsApprox(model, _lastModel))
			{
				RecalculateWorldAabb();
				_lastModel = model;
				_dirty = false;
			}
		}

        private void RebuildLocalCorners()
		{
			var half = Size * 0.5f;

			float minX = Centre.X - half.X;
			float minY = Centre.Y - half.Y;
			float minZ = Centre.Z - half.Z;

			float maxX = Centre.X + half.X;
			float maxY = Centre.Y + half.Y;
			float maxZ = Centre.Z + half.Z;

			_localCorners[0] = new(minX, minY, minZ);
			_localCorners[1] = new(maxX, minY, minZ);
			_localCorners[2] = new(maxX, maxY, minZ);
			_localCorners[3] = new(minX, maxY, minZ);

			_localCorners[4] = new(minX, minY, maxZ);
			_localCorners[5] = new(maxX, minY, maxZ);
			_localCorners[6] = new(maxX, maxY, maxZ);
			_localCorners[7] = new(minX, maxY, maxZ);
		}

        private void RecalculateWorldAabb()
		{
			var model = Owner.Transform.GetModelMatrix();

			for (int i = 0; i < 8; i++)
				_worldCorners[i] = Vector3D.Transform(_localCorners[i], model);

			float minX = _worldCorners.Min(c => c.X);
			float minY = _worldCorners.Min(c => c.Y);
			float minZ = _worldCorners.Min(c => c.Z);

			float maxX = _worldCorners.Max(c => c.X);
			float maxY = _worldCorners.Max(c => c.Y);
			float maxZ = _worldCorners.Max(c => c.Z);

			WorldMin = new(minX, minY, minZ);
			WorldMax = new(maxX, maxY, maxZ);

			WorldCenter = (WorldMin + WorldMax) * 0.5f;
			WorldHalfExtents = (WorldMax - WorldMin) * 0.5f;
		}

        private bool MatrixEqualsApprox(in Matrix4X4<float> a, in Matrix4X4<float> b)
		{
			// Cheap approx check. Good enough to detect movement/rotation/scale changes.
			const float eps = 0.00001f;

			return
				MathF.Abs(a.M11 - b.M11) < eps && MathF.Abs(a.M12 - b.M12) < eps && MathF.Abs(a.M13 - b.M13) < eps && MathF.Abs(a.M14 - b.M14) < eps &&
				MathF.Abs(a.M21 - b.M21) < eps && MathF.Abs(a.M22 - b.M22) < eps && MathF.Abs(a.M23 - b.M23) < eps && MathF.Abs(a.M24 - b.M24) < eps &&
				MathF.Abs(a.M31 - b.M31) < eps && MathF.Abs(a.M32 - b.M32) < eps && MathF.Abs(a.M33 - b.M33) < eps && MathF.Abs(a.M34 - b.M34) < eps &&
				MathF.Abs(a.M41 - b.M41) < eps && MathF.Abs(a.M42 - b.M42) < eps && MathF.Abs(a.M43 - b.M43) < eps && MathF.Abs(a.M44 - b.M44) < eps;
		}

        public override void OnInspectorGUI()
		{
			if (InputVector3D("Size", ref Size))
            {
                _dirty = true;
				RebuildLocalCorners();
				RecalculateWorldAabb();
				_lastModel = Owner.Transform.GetModelMatrix();
				_dirty = false;
            }
			if (InputVector3D("Centre", ref Centre))
            {
                _dirty = true;
				RebuildLocalCorners();
				RecalculateWorldAabb();
				_lastModel = Owner.Transform.GetModelMatrix();
				_dirty = false;
            }
            

			if (ImGui.Button("Recalculate AABB"))
			{
				_dirty = true;
				RebuildLocalCorners();
				RecalculateWorldAabb();
				_lastModel = Owner.Transform.GetModelMatrix();
				_dirty = false;
			}

			ImGui.Text($"WorldMin: {WorldMin}");
			ImGui.Text($"WorldMax: {WorldMax}");
			ImGui.Text($"WorldCenter: {WorldCenter}");
			ImGui.Text($"WorldHalfExtents: {WorldHalfExtents}");

		}

        public void Serialize(BinaryWriter writer)
        {
            UMath.WriteSilkVec3(writer, Size);
            UMath.WriteSilkVec3(writer, Centre);
        }

        public void Deserialize(BinaryReader reader)
        {
            Size = UMath.ReadSilkVec3(reader);
            Centre = UMath.ReadSilkVec3(reader);
            
            //May be Redundent => Check
            RebuildLocalCorners();
            RecalculateWorldAabb();
            _lastModel = Owner.Transform.GetModelMatrix();
        }
    }
}
