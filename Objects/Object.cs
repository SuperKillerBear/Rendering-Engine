using RenderingEngine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Objects
{
    public class Object
    {
        private Object? parent;
        private List<Object> children = new List<Object>();

        private List<Component> components = new List<Component>();

        public Object(Object? Parent = null)
        {
            this.parent = Parent;
            
        }


        public Component? GetComponent<T>() where T: Component
        {
            foreach (var comp in this.components)
            {
                if (comp is T matchedComponent)
                    return matchedComponent;
            }
            return null;
        }


        public void AddComponent(Component component)
        {
            this.components.Add(component);
            component.SetOwner(this);
        }

    }
}
