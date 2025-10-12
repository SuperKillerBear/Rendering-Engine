using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using RenderingEngine.Gui;
using RenderingEngine.Meshes;
using RenderingEngine.Rendering;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine
{
    public static class FileHandler
    {
        public static string currentLevel;

        public static void Init()
        {
            //Register Components Here
            ComponentRegistry.RegisterSerializable<TransformComponent>(1);
            ComponentRegistry.RegisterSerializable<RigidBodyComponent>(2);

            ComponentRegistry.RegisterNonSerializable<RendererComponent>(3);
            
            //TODO: Change Box Collider to Serializable Later when Implemented
            ComponentRegistry.RegisterNonSerializable<BoxColliderComponent>(4);
            
            LoadGameSettings();
        }

        public static void SaveGameSettings()
        {
            string localPath = @"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\GameSettings.dat";
            using var stream = File.Open(localPath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(stream);

            writer.Write(0x474D4454); //GMDT
            WriteVector3D(writer, Camera.Position);
            WriteVector3D(writer, Camera.Rotation);

            byte[] selectedSceneNameData = Encoding.UTF8.GetBytes(SettingsPanel.selectedSceneName); //
            writer.Write((ushort)selectedSceneNameData.Length);
            writer.Write(selectedSceneNameData);

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

                ushort selectedSceneNameLength = reader.ReadUInt16();
                byte[] selectedSceneNameData = reader.ReadBytes(selectedSceneNameLength);
                string selectedSceneName = Encoding.UTF8.GetString(selectedSceneNameData);


                Console.WriteLine("INFO: Game Settings Loaded");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("WARN: Cannot Load Game Settings");
                return;
            }
        }

        public static void SaveScene(string name)
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
                //Write Headers
                writer.Write((ushort)Program.SceneObjects[i].name.Length); //Write Object Name Length
                writer.Write(Encoding.UTF8.GetBytes(Program.SceneObjects[i].name)); //Write Object Name

                //writer.Write(Program.SceneObjects[i].name); //Write Object Data

                writer.Write((uint)Program.SceneObjects[i].Components.Count); //Write Component Count

                foreach (var comp in Program.SceneObjects[i].Components)
                {
                    ushort? typeID = ComponentRegistry.GetTypeID(comp);

                    if (typeID == null)
                        continue; //Skip Unregistered Components

                    writer.Write((ushort)typeID); //Write Component Type ID

                    if (comp is ISerializable serializableComp)
                    {
                        serializableComp.Serialize(writer); //Serialize Component Data
                    }
                }
            }
            
            writer.Close();
        }

        public static void LoadScene(string name)
        {
            string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\LevelData\{name}.dat";
            Console.WriteLine($"Trying to Read Level: {name}");

            if (!File.Exists(localPath))
            {
                Console.WriteLine("WARN: Level Does Not Exist");
                return;
            }
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

            Program.ClearScene();

            for (int i = 0; i < objectCount; i++)
            {
                GameObject newObj = new GameObject(autoAddTransform: false);

                
                
                ushort objNameLen = reader.ReadUInt16(); //Read Object Name Length
                byte[] objNameData = reader.ReadBytes(objNameLen); //Read Object Name Data
                string objName = Encoding.UTF8.GetString(objNameData); //Decode Object Name

                newObj.name = objName;

                uint compCount = reader.ReadUInt32(); //Read 

                for (int c = 0; c < compCount; c++)
                {
                    ushort typeID = reader.ReadUInt16();

                    if (!ComponentRegistry.factories.ContainsKey(typeID))
                    {
                        Console.WriteLine($"Unknown Component Type ID: {typeID}, skipping object.");
                        continue;
                    }

                    Component newComp = ComponentRegistry.Deserialize(typeID, reader);

                    if (newComp is not RendererComponent) newObj.AssignComponent(newComp);

                }

                newObj.AddComponent<RendererComponent>().Mesh = new CubeMesh(Program.gl);

                

                newObj.Transform.UpdateModelMatrix(); //Transform is null

            }
            
            Console.WriteLine($"Name: {LevelName}");
            currentLevel = LevelName;
            reader.Close();

            Program.PhysicsEnabled = true; //Will always turn on physics when scene loaded
            Program.RenderingEnabled = true;

            
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

    public interface ISerializable
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);

    }

    public static class ComponentRegistry
    {
        public static readonly Dictionary<ushort, Func<Component>> factories = new();
        public static readonly Dictionary<Type, ushort> typeIDs = new();

        public static void RegisterSerializable<T>(ushort typeID) where T : Component, ISerializable, new()
        {
            factories[typeID] = () => new T();
            typeIDs[typeof(T)] = typeID;
        }

        public static void RegisterNonSerializable<T>(ushort typeID) where T : Component, new()
        {
            factories[typeID] = () => new T();
            typeIDs[typeof(T)] = typeID;
        }

        public static ushort? GetTypeID(Component component)
        {
            Type type = component.GetType();

            if (!typeIDs.ContainsKey(type))
            {
                Console.WriteLine($"WARN: Unregistered Component Type: {type.ToString()}");
                return null;
            }

            return typeIDs[type];
        }

        public static Component Deserialize(ushort typeID, BinaryReader reader)
        {
            if (!factories.ContainsKey(typeID))
            {
                throw new Exception($"Unknown Component Type: {typeID}");
            }
            Component comp = factories[typeID]();

            if (comp is ISerializable serializableComp)
            {
                serializableComp.Deserialize(reader);
            }

            return comp;
        }

         
    }

}
