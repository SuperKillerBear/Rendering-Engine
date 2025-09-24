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

        public List<Vector2D<int>> chunks = new List<Vector2D<int>>();

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
            this.CalcChunks();
        }

        public void CalcChunks()
        {
            chunks.Clear();

            float xMin = Position.X - (Scale.X / 2);
            float xMax = Position.X + (Scale.X / 2);

            float yMin = Position.Y - (Scale.Y / 2);
            float yMax = Position.Y + (Scale.Y / 2);

            float zMin = Position.Z - (Scale.Z / 2);
            float zMax = Position.Z + (Scale.Z / 2);


            int chunkSize = Program.chunkSize;

            int chunkXMin = (int)xMin / chunkSize;
            int chunkXMax = (int)xMax / chunkSize;

            int chunkZMin = (int)zMin / chunkSize;
            int chunkZMax = (int)zMax / chunkSize;

            for (int x = chunkXMin; x <= chunkXMax; x++)
            {
                for (int z = chunkZMin; z <= chunkZMax; z++)
                {
                    chunks.Add(new Vector2D<int>(x, z));
                }
            }
        }

        
    }
}
