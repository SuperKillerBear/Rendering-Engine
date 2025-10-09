using RenderingEngine.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class PhysicsObjectsHandler
    {
        private static int bufferSize = 15;
        private static PhysicsObject?[] PhysObjBuffer = new PhysicsObject[bufferSize];

        private static int pointer = 0;
        public static int AddObj(PhysicsObject obj)
        {
            if (pointer >= bufferSize - 1) 
            { 
                Console.WriteLine("Physics Obj Buffer Full, Cant add new object!");  
                return -1; 
            }
            
            PhysObjBuffer[pointer] = obj;

            pointer++;

            return (pointer - 1);
        }

        public static void RemoveObj(int id)
        {
            PhysicsObject? obj = PhysObjBuffer[id];

            if (obj != null)
            {
                PhysObjBuffer[id] = null;
            }
            else { Console.WriteLine($"Object id {id.ToString()} is already null!"); }
        }


        public static void TickObjs(double deltaTime)
        {
            for (int i = 0; i < pointer; i++)
            {
                PhysicsObject? obj = PhysObjBuffer[i];
                if (obj != null)
                {
                    obj.TickPhysics(deltaTime);
                }
            }
        }


    }
}
