using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class LevelHandler
    {
        public static string currentLevel;

        public static void SaveLevel(string name)
        {
            string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\LevelData\{name}.dat";
            Console.WriteLine($"Writing Level: {localPath}");
            var writer = new BinaryWriter(File.Open(localPath, FileMode.OpenOrCreate));
            byte[] nameData = Encoding.UTF8.GetBytes(name);

            writer.Write(0x4C564C44); //LVLD

            

            writer.Write((ushort)nameData.Length); //Level Name Length
            writer.Write(nameData); //Level Name

            uint dynObjCount = (uint)Renderer.dynObjs.Length;

            writer.Write(dynObjCount); //Object Count

            for (int i = 0; i < dynObjCount; i++)
            {
                writer.Write(Renderer.dynObjs[i].name); //Write Object Data
            }

            writer.Close();
        }

        public static void LoadLevel(string name)
        {
            string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\LevelData\{name}.dat";
            Console.WriteLine($"Trying to Read Level: {name}");
            var reader = new BinaryReader(File.Open(localPath, FileMode.Open));
            uint magic = reader.ReadUInt32();

            if (magic != 0x4C564C44) //LVLD
            {
                Console.WriteLine("Invalid Level File");
                return;
            }

            

            ushort length = reader.ReadUInt16(); //Read the Length of the name
            byte[] nameData = reader.ReadBytes(length); //Read Name Data

            string LevelName = Encoding.UTF8.GetString(nameData);

            uint objectCount = reader.ReadUInt32(); //Read Object Count

            for (int i = 0; i < objectCount; i++)
            {
                string objName = reader.ReadString(); //Read Object Data
                //TODO: Assign Object Data to DynObj List
                Console.WriteLine($"Read Object {i}: {objName}");
            }

            Console.WriteLine($"Name: {LevelName}");
            currentLevel = LevelName;
            reader.Close();
        }

        

    }

    
}
