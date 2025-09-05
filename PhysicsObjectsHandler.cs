using RenderingEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class PhysicsObjectsHandler
    {
        private static int BufferSize = 15; //Change Later
        public static int pointer = 0;
        public static PhysicsObject?[] PhyObjBuffer = new PhysicsObject[BufferSize];

        public static int AddObj(PhysicsObject obj)
        {
            //Return -1 if Buffer Full
            if (pointer == PhyObjBuffer.Length) { return -1; }

            PhyObjBuffer[pointer] = obj;
            
            pointer++;

            return (pointer - 1);

        }

        public static void RemoveObj(int id)
        {
            //TODO
        }

        public static void TickObjects(double deltaTime)
        {
            //Assumes no gaps in buffer
            for (int i = 0; i <= pointer; i++)
            {
                PhysicsObject? obj = PhyObjBuffer[i];
                if (obj != null)
                {
                    obj.TickPhysics(deltaTime); 
                };


               
            }
        }



    }
}
