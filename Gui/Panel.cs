using ImGuiNET;
using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
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
        private GameObject selectedObject;
        

        private InspectorPanel inspectorPanel;

        public HierarchyPanel(InspectorPanel insPanel)
        {
            this.inspectorPanel = insPanel;
        }

        public override void Draw()
        {
            ImGuiNET.ImGui.Begin("Hierarchy");

            bool clickedItem = false;
            int idx = 0;
            foreach (var obj in Program.SceneObjects)
            {
                ImGui.PushID(idx);
                if (ImGuiNET.ImGui.Selectable(obj.name, obj == selectedObject))
                {
                    selectedObject = obj;
                    clickedItem = true;
                    
                }
                ImGui.PopID();
                idx++;
            }
            

            if (ImGui.IsWindowHovered() && !clickedItem)
            {
                //Make Better Logic + Code Here
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right)) {
                    selectedObject = null;
                    ImGui.OpenPopup("HierarchyContextMenu");
                }

            }
            if (ImGui.BeginPopup("HierarchyContextMenu"))
            {
                if (ImGui.MenuItem("Add Object"))
                {
                    //Logic

                }
                ImGui.EndPopup();
            }
            
            inspectorPanel.SetSelected(selectedObject); 
            
            ImGuiNET.ImGui.End();
        }
    }

    class InspectorPanel : Panel
    {
        private GameObject selectedObject;
        public void SetSelected(GameObject obj) => selectedObject = obj;

        public override void Draw()
        {
            if (selectedObject == null) return;
            ImGui.Begin("Inspector");

            ImGui.InputText("Name", ref selectedObject.name, 32);
            ImGui.Checkbox("Debug", ref selectedObject.debug);
            var parent = selectedObject.parent;
            var name = parent == null ? string.Empty : parent.name;
            ImGui.Text($"Parent: {name}");
            ImGui.Text($"Child Count: {selectedObject.children.Count}");
            string result = string.Join(", ", selectedObject.children.Select(c => c.name));
            ImGui.Text($"Children: {result}");
            foreach (var comp in selectedObject.Components)
            {
                if (ImGui.CollapsingHeader(comp.ComponentName))
                {
                    comp.OnInspectorGUI();
                }
            }

            ImGui.End();
        }
    }

    class SettingsPanel : Panel
    {
        public static string selectedSceneName = "Test";
        private static bool loadSceneRequested = false;
        public override void Draw()
        {
            ImGui.Begin("Settings");

            ImGui.Text($"FPS: {Program.lastFPS}");
            ImGui.Text($"Screen: {Program.ScreenWidth}x{Program.ScreenHeight}");
            ImGui.Text($"Camera Position: {Camera.Position.X}, {Camera.Position.Y}, {Camera.Position.Z}");
            if (ImGui.Button("Reset Camera Position")) { Camera.Position = Vector3D<float>.Zero; }
            ImGui.Text($"Accumulated Mouse Positon: ({InputHandler.accumMouseRelX}, {InputHandler.accumMouseRelY})");
            ImGui.InputFloat("Sensitivity", ref Camera.Sensitivity);
            ImGui.InputInt("FOV", ref Camera.FOV, (int)Math.PI / 180);
            ImGui.InputInt("Chunk Size", ref Program.chunkSize, 1, 2);
            ImGui.SliderFloat("Tick Rate", ref Program.tickRate, 0.01f, 10f);
            ImGui.Text($"Scene Objects Count: {Program.SceneObjects.Count}");
            ImGui.Text($"Current Level Name: {FileHandler.currentLevel}");
            ImGui.InputText("Selected Level Name", ref selectedSceneName, 32);
            if (ImGui.Button("Save Scene")) { FileHandler.SaveScene(selectedSceneName); }
            
            bool loadSceneClicked = ImGui.Button("Load Scene");
            
            if (loadSceneClicked && !loadSceneRequested)
            {
                loadSceneRequested = true;
                FileHandler.LoadScene(selectedSceneName);
            }
            else if (!loadSceneClicked)
            {
                loadSceneRequested = false;
            }
            
            if (ImGui.Button("Clear Scene")) { Program.ClearScene(); }
            
            ImGui.End();
        }



    }

}
