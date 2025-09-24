using RenderingEngine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Objects
{
    public abstract class DynamicObject
    {
        public uint 
            VAO,  //Vertex Array Object => How Vertex Attributes are Mapped
            VBO,  //Vertex Buffer Object => Vertices Pos + Colour
            EBO;  //Element Buffer Object => Indices

        public Vector3D<float> Position;
        public Vector3D<float> Rotation;
        public Vector3D<float> Scale = Vector3D<float>.One;

        public string name = "Dynamic Object";

        
        public Mesh Mesh { get; set; }

        // Per-object transform ie rotation, position, scale
        public Matrix4X4<float> ModelMatrix { get; set; }

        public void UpdateModelMatrix()
        {
            this.ModelMatrix =
                Matrix4X4.CreateScale(Scale) *
                Matrix4X4.CreateRotationX(Rotation.X) *
                Matrix4X4.CreateRotationY(Rotation.Y) *
                Matrix4X4.CreateRotationZ(Rotation.Z) *
                Matrix4X4.CreateTranslation(Position);
        }

        public DynamicObject(Mesh mesh) 
        { 
            this.Mesh = mesh;            
        }

        

        
    }
}
