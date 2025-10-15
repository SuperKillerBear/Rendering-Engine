using ImGuiNET;
using RenderingEngine.GameObjects;
using RenderingEngine.Utilities;
using Silk.NET.Input;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RenderingEngine.Components
{
    public class ColliderComponent : Component
    {
        public override string ComponentName => "Collider Component";

        public List<Vector2D<int>> chunks = new List<Vector2D<int>>();

        private float xMin, xMax, yMin, yMax, zMin, zMax;

        private Vector3D<float> lastRotation;

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner

            //Owner will not be null here

        }

        public void CalcAABBMaxMins()
        {
            if (Owner.Transform.Rotation == Vector3D<float>.Zero)
            {

                FlatAABB();

                var min = new Vector3D<float>(xMin, yMin, zMin);
                var max = new Vector3D<float>(xMax, yMax, zMax);

                if (Owner.debug)
                {
                    Console.WriteLine("Didnt Rotate AABB Calc");
                    Console.WriteLine($"{Owner.name}, Min: {min.ToString()}, Max: {max.ToString()}");
                }
            }
            else
            {
                FlatAABB();

                var min = new Vector3D<float>(xMin, yMin, zMin);
                var max = new Vector3D<float>(xMax, yMax, zMax);

                Vector3D<float> newMin, newMax;
                UMath.RotateAABB(min, max, Owner.Transform.Rotation, out newMin, out newMax);

                xMin = newMin.X;
                yMin = newMin.Y;
                zMin = newMin.Z;

                xMax = newMax.X;
                yMax = newMax.Y;
                zMax = newMax.Z;

                if (Owner.debug)
                {
                    Console.WriteLine($"{Owner.name}, Rotated AABB Calc, LastRot: {lastRotation.ToString()}, newRot: {Owner.Transform.Rotation.ToString()}");
                    Console.WriteLine($"OldMin: {min.ToString()}, OldMax: {max.ToString()}");
                    Console.WriteLine($"NewMin: {newMin.ToString()}, NewMax: {newMax.ToString()}");
                }
            }
            lastRotation = Owner.Transform.Rotation;
        }

        private void FlatAABB()
        {
            if (Owner.Transform == null) return;
            //Update to allow for parent / child continous transform scalers, etc
            xMin = Owner.Transform.Position.X - (Owner.Transform.Scale.X / 2);
            xMax = Owner.Transform.Position.X + (Owner.Transform.Scale.X / 2);
            yMin = Owner.Transform.Position.Y - (Owner.Transform.Scale.Y / 2);
            yMax = Owner.Transform.Position.Y + (Owner.Transform.Scale.Y / 2);
            zMin = Owner.Transform.Position.Z - (Owner.Transform.Scale.Z / 2);
            zMax = Owner.Transform.Position.Z + (Owner.Transform.Scale.Z / 2);
        }

        public void CalcChunks()
        {
            chunks.Clear();

            CalcAABBMaxMins();


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

        public Vector3D<float> CalcCollisions(ColliderComponent obj2)
        {
            Vector3D<float> resultant = Vector3D<float>.Zero;

            //Do AABB Collision Checks Here
            var dPos = obj2.Owner.Transform.Position - Owner.Transform.Position;

            var px = (this.xMax / 2 + obj2.xMax / 2) - MathF.Abs(dPos.X);
            var py = (this.yMax / 2 + obj2.yMax / 2) - MathF.Abs(dPos.Y);
            var pz = (this.zMax / 2 + obj2.zMax / 2) - MathF.Abs(dPos.Z);

            if (px > 0 && py > 0 && pz > 0)
            {
                resultant = new Vector3D<float>(
                    px * UMath.Sign(dPos.X),
                    py * UMath.Sign(dPos.Y),
                    pz * UMath.Sign(dPos.Z)
                    );


            }

            /*
            Console.WriteLine($"X: {px}");
            Console.WriteLine($"Y: {py}");
            Console.WriteLine($"Z: {pz}");

            Console.WriteLine($"Resultant: {resultant.ToString()}");
            Console.WriteLine($"Positions: {Position.ToString()}, {obj2.Position.ToString()}");
            Console.WriteLine($"Max, Min Y. {name}: {yMax}, {yMin}, {obj2.name}: {obj2.yMin}, {obj2.yMin}");
            */

            return resultant;
        }

        public override void OnInspectorGUI()
        {
            //No editable properties
        }

    }
}
