using RenderingEngine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.GameObjects
{
    public class GameObject : IDisposable
    {
        public string name = "Empty Object";
        public bool debug = false;

        public TransformComponent Transform { get; private set; }

        public GameObject? parent { get; private set; } = null;
        public List<GameObject> children { get; private set; } = new List<GameObject>(); 

        public List<Component> Components = new List<Component>();

        public GameObject(GameObject? Parent = null, bool autoAddTransform = true, string name = "Empty Object")
        {
            this.name = name;
            if (Parent != null) Parent.AssignChild(this);
            Program.SceneObjects.Add(this);
            if (autoAddTransform) Transform = this.AddComponent<TransformComponent>();
        }

        public void AssignChild(GameObject child)
        {
            this.children.Add(child);
            child.parent = this;
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

            if (component is TransformComponent && this.Transform == null)
                this.Transform = component as TransformComponent;

            component.Init(this);
        }

        public void Dispose()
        {
            foreach (var comp in this.Components)
            {
                if (comp is IDisposable disposable)
                    disposable.Dispose();
                
            }

            Components.Clear();
        }
    }
}
