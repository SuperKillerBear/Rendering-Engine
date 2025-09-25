using RenderingEngine.Rendering;
using RenderingEngine.Utilities;
using Silk.NET.Core.Native;
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

        public float xMin, xMax, yMin, yMax, zMin, zMax;

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

            xMin = Position.X - (Scale.X / 2);
            xMax = Position.X + (Scale.X / 2);

            yMin = Position.Y - (Scale.Y / 2);
            yMax = Position.Y + (Scale.Y / 2);

            zMin = Position.Z - (Scale.Z / 2);
            zMax = Position.Z + (Scale.Z / 2);


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

        public Vector3D<float> CalcCollisions(DynamicObject obj2)
        {
            Vector3D<float> resultant = Vector3D<float>.Zero;

            //Do AABB Collision Checks Here
            var dPos = obj2.Position - this.Position;

            var px = (this.Scale.X / 2 + obj2.Scale.X / 2) - MathF.Abs(dPos.X);
            var py = (this.Scale.Y / 2 + obj2.Scale.Y / 2) - MathF.Abs(dPos.Y);
            var pz = (this.Scale.Z / 2 + obj2.Scale.Z / 2) - MathF.Abs(dPos.Z);

            if (px > 0 && py > 0 && pz > 0)
            {
                resultant = new Vector3D<float>(
                    px * UMath.Sign(dPos.X),
                    py * UMath.Sign(dPos.Y),
                    pz * UMath.Sign(dPos.Z)
                    );

                
                
                
            
            }

            Console.WriteLine($"X: {px}");
            Console.WriteLine($"Y: {py}");
            Console.WriteLine($"Z: {pz}");

            Console.WriteLine($"Resultant: {resultant.ToString()}");
            Console.WriteLine($"Positions: {Position.ToString()}, {obj2.Position.ToString()}");
            Console.WriteLine($"Max, Min Y. {name}: {yMax}, {yMin}, {obj2.name}: {obj2.yMin}, {obj2.yMin}");

            return resultant;
        }
    }
}
