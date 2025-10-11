using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class PhysicsObjectsHandler
    {
        private static int bufferSize = 15;
        private static RigidBodyComponent?[] PhysObjBuffer = new RigidBodyComponent[bufferSize];

        private static int pointer = 0;
        public static int AddObj(RigidBodyComponent obj)
        {
            if (pointer >= bufferSize - 1) 
            { 
                Console.WriteLine("WARN: Physics Obj Buffer Full, Cant add new object!");  
                return -1; 
            }
            
            PhysObjBuffer[pointer] = obj;

            pointer++;
            //Console.WriteLine($"Added Physics Object, new count: {pointer.ToString()}");
            return (pointer - 1);
        }

        
        
        public static void TickObjs(double deltaTime)
        {
            
            for (int i = 0; i < pointer; i++)
            {
                RigidBodyComponent? obj = PhysObjBuffer[i];
                if (obj != null)
                {
                    obj.TickPhysics(deltaTime);
                }
                
            }
        }

        public static void ClearAll()
        {
            PhysObjBuffer = new RigidBodyComponent[bufferSize];
        }


    }
}
