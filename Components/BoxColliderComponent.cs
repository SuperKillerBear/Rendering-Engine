using ImGuiNET;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class BoxColliderComponent : ColliderComponent
    {
        public override string ComponentName => "Box Collider Component";

        public Vector3D<float> size = new Vector3D<float>(1, 1, 1);
        public Vector3D<float> centre= new Vector3D<float>(0, 0, 0);

        //TODO => WRITE CODE FOR THIS COMPONENET

        public override void OnInspectorGUI()
        {
            InputVector3D("Size", ref size);
            InputVector3D("Centre", ref centre);
            ImGui.Text($"Chunks: {string.Join(", ", chunks.Select(c => $"({c.X}, {c.Y})"))}");


            if (ImGui.Button("Update Calcs"))
            {
                CalcChunks();
            }
        }

    }
}
