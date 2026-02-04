using RenderingEngine.Components;
using Silk.NET.Maths;

namespace RenderingEngine.GameObjects
{
    public struct Contact
    {
    	public BoxColliderComponent ACol;
    	public BoxColliderComponent BCol;
    
    	public RigidBodyComponent? ARb; // null => static
    	public RigidBodyComponent? BRb;
    
    	public Vector3D<float> Normal;	// points A -> B
    	public float Penetration;
    }
    
    
}


