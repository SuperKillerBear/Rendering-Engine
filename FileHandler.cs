using RenderingEngine.Rendering;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class FileHandler
    {
        public static string currentLevel;

        public static void SaveGameSettings()
        {
            string localPath = @"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\GameSettings.dat";
            using var stream = File.Open(localPath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(stream);

            writer.Write(0x474D4454); //GMDT
            WriteVector3D(writer, Camera.Position);
            WriteVector3D(writer, Camera.Rotation);

            Console.WriteLine("INFO: Game Settings Saved");
        }

        public static void LoadGameSettings()
        {
            string localPath = @"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\GameSettings.dat";

            try
            {
                using var stream = File.Open(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                // Minimum length: 4 bytes (magic) + 12 bytes (position) + 12 bytes (rotation) = 28 bytes
                if (stream.Length < 28)
                {
                    Console.WriteLine("WARN: Game Settings file is too short or empty.");
                    return;
                }

                using var reader = new BinaryReader(stream);
                uint magic = reader.ReadUInt32();

                if (magic != 0x474D4454) //GMDT
                {
                    Console.WriteLine("WARN: Invalid Game Settings File");
                    return;
                }

                Camera.Position = ReadVector3D(reader); //Set Cam Pos
                Camera.Rotation = ReadVector3D(reader); //Set Cam Rot

                Console.WriteLine("INFO: Game Settings Loaded");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("WARN: Cannot Load Game Settings");
                return;
            }
        }

        public static void SaveLevel(string name)
        {
            string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\LevelData\{name}.dat";
            Console.WriteLine($"Writing Level: {localPath}");
            var writer = new BinaryWriter(File.Open(localPath, FileMode.OpenOrCreate));
            byte[] nameData = Encoding.UTF8.GetBytes(name);

            writer.Write(0x4C564C44); //LVLD

            

            writer.Write((ushort)nameData.Length); //Level Name Length
            writer.Write(nameData); //Level Name
            
            uint objsCount = (uint)Program.SceneObjects.Count;

            writer.Write(objsCount); //Object Count

            for (int i = 0; i < objsCount; i++)
            {
                writer.Write(Program.SceneObjects[i].name); //Write Object Data

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
            /*
            uint objectCount = reader.ReadUInt32(); //Read Object Count

            for (int i = 0; i < objectCount; i++)
            {
                string objName = reader.ReadString(); //Read Object Data
                //TODO: Assign Object Data to DynObj List
                Console.WriteLine($"Read Object {i}: {objName}");
            }
            */
            Console.WriteLine($"Name: {LevelName}");
            currentLevel = LevelName;
            reader.Close();
        }

        private static void WriteVector3D(BinaryWriter writer, Vector3D<float> vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }

        private static Vector3D<float> ReadVector3D(BinaryReader reader)
        {
            return new Vector3D<float>(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
                );
            
        }

    }

    
}
