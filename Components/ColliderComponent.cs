using ImGuiNET;
using RenderingEngine.GameObjects;
using RenderingEngine.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RenderingEngine.Components
{
    public class ColliderComponent : Component
    {
        public override string ComponentName => "Collider Component";

        public List<Vector2D<int>> chunks = new List<Vector2D<int>>();

        private float xMin, xMax, yMin, yMax, zMin, zMax;

        private Vector3D<float> lastRotation;

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner

            //Owner will not be null here

        }

        public void CalcChunks()
        {
            chunks.Clear();

            int chunkSize = Program.chunkSize;

            int chunkXMin = (int)xMin / chunkSize;
            int chunkXMax = (int)xMax / chunkSize;

            int chunkZMin = (int)zMin / chunkSize;
            int chunkZMax = (int)zMax / chunkSize;

            for (int x = chunkXMin; x <= chunkXMax; x++)
            {
                for (int z = chunkZMin; z <= chunkZMax; z++)
                {
                    chunks.Add(new Vector2D<int>(x, z));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            //No editable properties
        }

    }
}
