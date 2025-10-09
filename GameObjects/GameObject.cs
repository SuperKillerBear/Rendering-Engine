using RenderingEngine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.GameObjects
{
    public class GameObject
    {
        public string name = "Empty Object";
        public bool debug = false;



        private GameObject? parent;
        private List<GameObject> children = new List<GameObject>();

        private List<Component> components = new List<Component>();

        public GameObject(GameObject? Parent = null)
        {
            this.parent = Parent;
            this.AddComponent(new TransformComponent());
        }

        public bool RemoveComponent(Component component)
        {
            return this.components.Remove(component);
        }

        public T? GetComponent<T>() where T: Component
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
            component.Init(this);
        }

    }
}
