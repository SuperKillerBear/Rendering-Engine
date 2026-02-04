using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class PhysicsObjectsHandler
    {
        private static int bufferSize = 30;
        private static RigidBodyComponent?[] PhysObjBuffer = new RigidBodyComponent[bufferSize];

        private static List<int> unusedIndices = new List<int>();

        private static int pointer = 0;
        public static int AddObj(RigidBodyComponent rb) //Issue, May be repeating objects as if object has both rigidbody and box collider => owner added twice
        {
            //Array.FindIndex(PhysObjBuffer, x => x.Contains("author"));
            //if (obj in PhysObjBuffer) 
            if (unusedIndices.Count > 0) //System to reuse indicies from list
            {
                int reusedIndex = unusedIndices[0];
                unusedIndices.Remove(reusedIndex);

                PhysObjBuffer[reusedIndex] = rb;
            
                return reusedIndex;
            }

            if (pointer >= bufferSize - 1)
            { 
                Console.WriteLine("WARN: Physics Obj Buffer Full, Cant add new object!");  
                return -1; 
            }
            
            PhysObjBuffer[pointer] = rb;

            pointer++;
            //Console.WriteLine($"Added Physics Object, new count: {pointer.ToString()}");
            return (pointer - 1);
        }

        public static void RemoveObj(RigidBodyComponent obj, int index)
        {
            if (index <= 0) return;
            if (PhysObjBuffer[index] == obj)
            {
                PhysObjBuffer[index] = null;
                unusedIndices.Add(index);
            }
        }
        
        
        public static void TickObjs(double deltaTime)
        {
            
            for (int i = 0; i < pointer; i++)
            {
                RigidBodyComponent rb = PhysObjBuffer[i];
                if (rb != null)
                {
                    rb.TickPhysics(deltaTime);
                    
                    
                }
                
                
            }
        }

        
        public static void ClearAll()
        {
            PhysObjBuffer = new RigidBodyComponent[bufferSize];
            unusedIndices.Clear();
            pointer = 0;
        }


    }
}
