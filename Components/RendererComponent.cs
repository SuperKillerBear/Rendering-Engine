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
        public Mesh? Mesh { get; set; }

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner
            Mesh = null;
            Renderer.RenderingObjects.Add(this);
        }

    }
}
