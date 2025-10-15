using ImGuiNET;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public abstract class Component : IDisposable
    {
        public GameObject? Owner = null;
        public abstract string ComponentName { get; }


        public void SetOwner(GameObject Owner)
        {
            this.Owner = Owner;
        }

        public virtual void Init(GameObject Owner)
        {
            this.Owner = Owner;
            //Base Init
        }

        public abstract void OnInspectorGUI();


        public static bool InputVector3D(string label, ref Vector3D<float> vec)
        {
            Vector3 newVec = new Vector3(vec.X, vec.Y, vec.Z);

            bool changed = ImGui.InputFloat3(label, ref newVec);
            if (changed) vec = new Vector3D<float>(newVec.X, newVec.Y, newVec.Z);
            return changed;
        }
    
        public virtual void Dispose()
        {
            //Default Behaviour
        }
    
    }

    
}
