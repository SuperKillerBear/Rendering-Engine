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

        public readonly TransformComponent Transform;

        private GameObject? parent;
        private List<GameObject> children = new List<GameObject>();

        public List<Component> Components = new List<Component>();

        public GameObject(GameObject? Parent = null)
        {
            this.parent = Parent;
            Program.SceneObjects.Add(this);
            Transform = this.AddComponent<TransformComponent>();
        }

        public bool RemoveComponent(Component component)
        {
            return this.Components.Remove(component);
        }

        public T? GetComponent<T>() where T: Component
        {
            foreach (var comp in this.Components)
            {
                if (comp is T matchedComponent)
                    return matchedComponent;
            }
            return null;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            this.Components.Add(component);
            component.Init(this);
            return component;
        }

        public void AssignComponent(Component component)
        {
            this.Components.Add(component);
            component.Init(this);
        }
    }
}
