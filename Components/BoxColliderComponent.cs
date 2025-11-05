using ImGuiNET;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class BoxColliderComponent : ColliderComponent
    {
        public override string ComponentName => "Box Collider Component";

        //TODO => WRITE CODE FOR THIS COMPONENET


        public Vector3D<float> size = new Vector3D<float>(1, 1, 1);
        public Vector3D<float> centre= new Vector3D<float>(0, 0, 0);

        Vector3D<float> halfSize;
        Vector3D<float> Max;
        Vector3D<float> Min;
        Vector3D<float> boxCentre;
        Vector3D<float>[] unRotatedCorners = new Vector3D<float>[8];
        Vector3D<float>[] rotatedCorners = new Vector3D<float>[8];



        public override void Init(GameObject Owner)
        {
                base.Init(Owner); //Init + Set Owner
                PopulateCorner();
                CalculateAABB();
        }

        public void PopulateCorner()
        {
            float halfSizeX = (float)(size.X / 2);
            float halfSizeY = (float)(size.Y / 2);
            float halfSizeZ = (float)(size.Z / 2);

            halfSize = new Vector3D<float>(halfSizeX, halfSizeY, halfSizeZ);

            float minX = (float)(centre.X - halfSizeX);
            float minY = (float)(centre.Y - halfSizeY);
            float minZ = (float)(centre.Z - halfSizeZ);

            float maxX = (float)(centre.X + halfSizeX);
            float maxY = (float)(centre.Y + halfSizeY);
            float maxZ = (float)(centre.Z + halfSizeZ);


            unRotatedCorners[0] = new Vector3D<float>(minX, minY, minZ);
            unRotatedCorners[1] = new Vector3D<float>(minX, minY, maxZ);
            unRotatedCorners[2] = new Vector3D<float>(minX, maxY, minZ);
            unRotatedCorners[3] = new Vector3D<float>(maxX, minY, minZ);
            unRotatedCorners[4] = new Vector3D<float>(minX, maxY, maxZ);
            unRotatedCorners[5] = new Vector3D<float>(maxX, minY, maxZ);
            unRotatedCorners[6] = new Vector3D<float>(maxX, maxY, minZ);
            unRotatedCorners[7] = new Vector3D<float>(maxX, maxY, maxZ);
        }

        public void CalculateAABB()
        {
           
            for (int i = 0; i < unRotatedCorners.Length; i++)
                rotatedCorners[i] = Vector3D.Transform(unRotatedCorners[i], Owner.Transform.GetModelMatrix());

            float minX = rotatedCorners.Min(c => c.X);
            float minY = rotatedCorners.Min(c => c.Y);
            float minZ = rotatedCorners.Min(c => c.Z);

            float maxX = rotatedCorners.Max(c => c.X);
            float maxY = rotatedCorners.Max(c => c.Y);
            float maxZ = rotatedCorners.Max(c => c.Z);

            Min = new Vector3D<float>(minX, minY, minZ);
            Max = new Vector3D<float>(maxX, maxY, maxZ);

            boxCentre = (Min + Max) * 0.5f;
        }


        //Saves Computation Time by not recalculating corners every time
        //Uses this if only position is dirty
        public void TranslateCorners(Vector3D<float> translation)
        {
            for (int i = 0; i < rotatedCorners.Length; i++)
            {
                rotatedCorners[i] += translation;
            }
        }


        public Vector3D<float> CollideAABBs(BoxColliderComponent obj2)
        {
            Vector3D<float> resultant = Vector3D<float>.Zero;

            
            Vector3D<float> delta = obj2.boxCentre - boxCentre;
            Vector3D<float> overlap = (halfSize + obj2.halfSize) - new Vector3D<float>(
                MathF.Abs(delta.X),
                MathF.Abs(delta.Y),
                MathF.Abs(delta.Z)
                );


            if (overlap.X < 0 || overlap.Y < 0 || overlap.Z < 0)
            {
                return resultant; //No Collision
            }

            //Find Axis of Minimum Penetration
            if (overlap.X < overlap.Y && overlap.X < overlap.Z)
            {
                resultant.X = (delta.X < 0) ? -overlap.X : overlap.X;
            }
            else if (overlap.Y < overlap.X && overlap.Y < overlap.Z)
            {
                resultant.Y = (delta.Y < 0) ? -overlap.Y : overlap.Y;
            }
            else
            {
                resultant.Z = (delta.Z < 0) ? -overlap.Z : overlap.Z;
            }


            return resultant;
        }


        public override void OnInspectorGUI()
        {
            InputVector3D("Size", ref size);
            InputVector3D("Centre", ref centre);
            ImGui.Text($"Min: {Min.ToString()}");
            ImGui.Text($"Max: {Max.ToString()}");
            ImGui.Text($"Chunks: {string.Join(", ", chunks.Select(c => $"({c.X}, {c.Y})"))}");

            debugVar("Half Size", halfSize.ToString());
            debugVar("Box Centre", boxCentre.ToString());
            debugVar("UnRotated Corners", string.Join(", ", unRotatedCorners.Select(c => c.ToString())));
            debugVar("Rotated Corners", string.Join(", ", rotatedCorners.Select(c => c.ToString())));
            


            if (ImGui.Button("Update Calcs"))
            {
                CalcChunks();
            }
        }


        private void debugVar(string name, string value)
        {
            ImGui.Text($"{name}: {value}");
        }
        


        

    }
}
