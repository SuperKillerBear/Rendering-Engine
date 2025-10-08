using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class Component
    {
        Object owner;

        public void SetOwner(Object Owner)
        {
            this.owner = Owner;
        }
    }

    
}
