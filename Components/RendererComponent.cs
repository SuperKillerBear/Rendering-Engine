using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class RendererComponent : Component
    {
        public override string ComponentName => "Renderer Component";
        public Mesh? Mesh { get; set; }


        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner
            Mesh = null;
            Renderer.RenderingObjects.Add(this);
        }

        public override void OnInspectorGUI()
        {
            if (Mesh != null)
            {
                ImGuiNET.ImGui.Text("Mesh is Assigned");
            }
            else
            {
                ImGuiNET.ImGui.Text("No Mesh Assigned");
            }
        }

        

    }
}
