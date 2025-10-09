using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderingEngine.GameObjects;

namespace RenderingEngine.Components
{
    public class Component
    {
        public GameObject? owner = null;

        public void SetOwner(GameObject Owner)
        {
            this.owner = Owner;
        }

        public virtual void Init(GameObject Owner)
        {
            this.owner = Owner;
            //Base Init
        }
    }

    
}
