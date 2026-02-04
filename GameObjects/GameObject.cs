using RenderingEngine.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.GameObjects
{
    public class GameObject : IDisposable
    {
        public string Name = "Empty Object";
        public bool debug = false;

        public TransformComponent Transform { get; private set; }

        public GameObject? Parent { get; set; } = null; //No longer private set
        public List<GameObject> Children { get; private set; } = new List<GameObject>(); 

        public List<Component> Components = new List<Component>();
        
        public int SceneID = -1;

        public GameObject(GameObject? Parent = null, bool autoAddTransform = true, string name = "Empty Object")
        {
            this.Name = name;
            if (Parent != null) Parent.AssignChild(this);
            
            this.SceneID = Program.SceneObjects.Count;
            Program.SceneObjects.Add(this);
            
            if (autoAddTransform) Transform = this.AddComponent<TransformComponent>();
        }
    
        public void SetParent(GameObject? newParent)
        {
        	if (Parent == newParent) return;
        
        	// remove from old parent's children
        	if (Parent != null)
        	{
        		Parent.Children.Remove(this);
        	}
        
        	Parent = newParent;
        
        	// add to new parent's children
        	if (newParent != null && !newParent.Children.Contains(this))
        	{
        		newParent.Children.Add(this);
        	}
        }

        public void AssignChild(GameObject child)
        {
            this.Children.Add(child);
            child.Parent = this;
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

        public void AssignComponent(Component component, bool shouldInitComp = true)
        {
            this.Components.Add(component);

            if (component is TransformComponent && this.Transform == null)
                this.Transform = component as TransformComponent;

            if (shouldInitComp) component.Init(this);
        }

        public void Dispose()
        {
            // Dispose of All Children
            for (int i = Children.Count - 1; i >= 0; i--)
        	{
        		var child = Children[i];
        		child.Dispose();
        	}
        	
        	Children.Clear();
        	
        	// Detatch Parent
        	if (Parent != null)
        	{
        		Parent.Children.Remove(this);
        		Parent = null;
        	}
        	
            foreach (var comp in this.Components)
            {
                if (comp is IDisposable disposable)
                    disposable.Dispose();
                
            }

            Components.Clear();
            
            // Remove from Scene List
            if (Program.SceneObjects.Contains(this)) Program.SceneObjects.Remove(this);
            else {Console.WriteLine($"GameObject not in SceneObjects: {this.Name}");}
        }
    }
}
