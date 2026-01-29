using System.Drawing;
using RenderingEngine.Components;
using Silk.NET.Maths;

namespace RenderingEngine 
{
    public static class BoxColliderHandler 
    {
        private static int bufferSize = 150;
        private static BoxColliderComponent?[] ColliderObjBuffer = new BoxColliderComponent[bufferSize];

        private static List<int> unusedIndices = new List<int>();

        private static int pointer = 0;
        public static int AddObj(BoxColliderComponent rb) 
        {
            //Issue, May be repeating objects as if object has both rigidbody and box collider => owner added twice
            //Array.FindIndex(PhysObjBuffer, x => x.Contains("author"));
            //if (obj in PhysObjBuffer) 

            //System to reuse indicies from list
            if (unusedIndices.Count > 0) 
            {
                int reusedIndex = unusedIndices[0];
                unusedIndices.Remove(reusedIndex);

                ColliderObjBuffer[reusedIndex] = rb;
            
                return reusedIndex;
            }

            if (pointer >= bufferSize - 1) 
            { 
                Console.WriteLine("WARN: Collider Obj Buffer Full, Cant add new object!");  
                return -1; 
            }
            
            ColliderObjBuffer[pointer] = rb;

            pointer++;
            return (pointer - 1);
        }

        public static void RemoveObj(BoxColliderComponent obj, int index)
        {
            if (index <= 0) return;
            if (ColliderObjBuffer[index] == obj)
            {
                ColliderObjBuffer[index] = null;
                unusedIndices.Add(index);
            }
        }
        

        
        private static bool Intersects(in Vector3D<float> aMin, in Vector3D<float> aMax, in Vector3D<float> bMin, in Vector3D<float> bMax)
        {
            return
                aMin.X <= bMax.X && aMax.X >= bMin.X &&
                aMin.Y <= bMax.Y && aMax.Y >= bMin.Y &&
                aMin.Z <= bMax.Z && aMax.Z >= bMin.Z;
        }

        public static void CalcColisions()
        {
            // 1) Update AABBs first
            for (int i = 0; i < pointer; i++)
            {
                var box = ColliderObjBuffer[i];
                if (box != null) box.TickCollider();
            }

            // 2) Pair checks
            for (int i = 0; i < pointer; i++)
            {
                var a = ColliderObjBuffer[i];

                for (int j = i + 1; j < pointer; j++)
                {
                    var b = ColliderObjBuffer[j];

                    // Skip same owner
                    if (a.Owner == b.Owner)
                        continue;

                    if (!Intersects(a.WorldMin, a.WorldMax, b.WorldMin, b.WorldMax))
                        continue;

                    OnCollision(a, b);
                }
            }
        }

        private static void OnCollision(BoxColliderComponent a, BoxColliderComponent b)
        {
            // Example
            a.IsColliding = true;
            b.IsColliding = true;

            // Or
            // a.Owner.OnCollision(b.Owner);
            // b.Owner.OnCollision(a.Owner);
        }


        public static void ClearAll()
        {
            ColliderObjBuffer = new BoxColliderComponent[bufferSize];
        }

    }
}