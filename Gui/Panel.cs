using ImGuiNET;
using RenderingEngine.Objects;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Gui
{
    class Panel
    {
        public virtual void Draw() { }

        public bool InputVector3D(string label, ref Vector3D<float> vec)
        {
            Vector3 newVec = new Vector3(vec.X, vec.Y, vec.Z);

            bool changed = ImGui.InputFloat3(label, ref newVec);
            if (changed) vec = new Vector3D<float>(newVec.X, newVec.Y, newVec.Z);
            return changed;
        }
    }

    class HierarchyPanel : Panel
    {
        private DynamicObject selectedObject;
        private DynamicObject[] sceneObjects;

        private InspectorPanel inspectorPanel;

        public HierarchyPanel(DynamicObject[] sceneObjects, InspectorPanel insPanel)
        {
            this.sceneObjects = sceneObjects;
            this.inspectorPanel = insPanel;
        }

        public override void Draw()
        {
            ImGuiNET.ImGui.Begin("Hierarchy");
            foreach (var obj in sceneObjects)
            {
                if (ImGuiNET.ImGui.Selectable(obj.name, obj == selectedObject))
                {
                    selectedObject = obj;
                }
            }
            inspectorPanel.SetSelected(selectedObject);
            ImGuiNET.ImGui.End();
        }
    }

    class InspectorPanel : Panel
    {
        private DynamicObject selectedObject;
        public void SetSelected(DynamicObject obj) => selectedObject = obj;

        public override void Draw()
        {
            if (selectedObject == null) return;
            ImGuiNET.ImGui.Begin("Inspector");

            ImGuiNET.ImGui.InputText("Name", ref selectedObject.name, 32);
            this.InputVector3D("Position", ref selectedObject.Position);
            this.InputVector3D("Rotation", ref selectedObject.Rotation);
            this.InputVector3D("Scale", ref selectedObject.Scale);
            ImGui.Text($"Chunks: {string.Join(", ", selectedObject.chunks.Select(c => $"({c.X}, {c.Y})"))}");

            ImGuiNET.ImGui.End();
        }
    }
}
