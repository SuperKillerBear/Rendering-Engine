using RenderingEngine.Components;
using RenderingEngine.GameObjects;
using RenderingEngine.Gui;
using RenderingEngine.Meshes;
using RenderingEngine.Rendering;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public static string baseDir;
        public static string GameSettingsDir;
        public static string LevelDataDir;

        public static void Init()
        {
            //Register Components Here
            ComponentRegistry.RegisterSerializable<TransformComponent>(1);
            ComponentRegistry.RegisterSerializable<RigidBodyComponent>(2);

            ComponentRegistry.RegisterNonSerializable<RendererComponent>(3);
            
            //TODO: Change Box Collider to Serializable Later when Implemented
            ComponentRegistry.RegisterNonSerializable<BoxColliderComponent>(4);
            
            baseDir = AppContext.BaseDirectory;
            //GameSettingsDir = Path.Combine(baseDir, "MeshData", filename);
            GameSettingsDir = Path.Combine(baseDir, "GameSettings.dat");
            LevelDataDir = Path.Combine(baseDir, "LevelData");

            LoadGameSettings();
        }

        public static void SaveGameSettings()
        {
            //string localPath = @"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\GameSettings.dat";
            using var stream = File.Open(GameSettingsDir, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(stream);

            writer.Write(0x474D4454); //GMDT
            WriteVector3D(writer, Camera.Position);
            WriteVector3D(writer, Camera.Rotation);

            byte[] selectedSceneNameData = Encoding.UTF8.GetBytes(SettingsPanel.selectedSceneName); //
            writer.Write((ushort)selectedSceneNameData.Length);
            writer.Write(selectedSceneNameData);

            writer.Write((double)Renderer.SunIntensity);
            writer.Write((double)Renderer.AmbientIntensity);

            writer.Write(Program.ShowBoundingBoxes);
            writer.Write(Program.CapFPS);

            Console.WriteLine("INFO: Game Settings Saved");
        }

        public static void LoadGameSettings()
        {
            try
            {
                using var stream = File.Open(GameSettingsDir, FileMode.Open, FileAccess.Read, FileShare.Read);
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

                Renderer.SunIntensity = (float) reader.ReadDouble();
                Renderer.AmbientIntensity = (float) reader.ReadDouble();

                Program.ShowBoundingBoxes = (bool) reader.ReadBoolean();
                
                Program.CapFPS = (bool) reader.ReadBoolean();
                Program.UpdateFPSCap();

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
            //string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\LevelData\{name}.dat";
            string localPath = Path.Combine(LevelDataDir, $"{name}.dat");

            Console.WriteLine($"Writing Level: {localPath}");
            var writer = new BinaryWriter(File.Open(localPath, FileMode.OpenOrCreate));
            byte[] nameData = Encoding.UTF8.GetBytes(name);

            writer.Write(0x4C564C44); //LVLD

            

            writer.Write((ushort)nameData.Length); //Level Name Length
            writer.Write(nameData); //Level Name
            
            List<GameObject> ObjsToSave = new();
            for (int i = 0; i < Program.SceneObjects.Count; i++)
            {
                if (Program.SceneObjects[i].Parent == null)
		            Collect(Program.SceneObjects[i], ObjsToSave);
            }
            
            uint objsCount = (uint)ObjsToSave.Count;

            writer.Write(objsCount); //Object Count

            for (int i = 0; i < objsCount; i++)
            {
                GameObject obj = ObjsToSave[i];
                
                int parentID = obj.Parent != null ? obj.Parent.SceneID : -1;
                
                // MyID
                writer.Write(obj.SceneID); 
                
                // Write Parent Scene ID
                writer.Write(parentID);
                
                // Write Headers
                writer.Write((ushort)obj.Name.Length); //Write Object Name Length
                writer.Write(Encoding.UTF8.GetBytes(obj.Name)); //Write Object Name

                //writer.Write(Program.SceneObjects[i].name); //Write Object Data
                
                
                var serializable = obj.Components
                    .Select(c => (comp: c, id: ComponentRegistry.GetTypeID(c)))
                    .Where(x => x.id != null)
                    .ToList();
                
                writer.Write((uint)serializable.Count);
                
                foreach (var x in serializable)
                {
                    writer.Write((ushort)x.id!);
                    if (x.comp is ISerializable s) s.Serialize(writer);
                }
            }
            
            writer.Close();
        }

        public static void LoadScene(string name)
        {
            string localPath = Path.Combine(LevelDataDir, $"{name}.dat");
            Console.WriteLine($"Trying to Read Level: {name}");

            if (!File.Exists(localPath))
            {
                Console.WriteLine("WARN: Level Does Not Exist");
                return;
            }
            using var reader = new BinaryReader(File.Open(localPath, FileMode.Open));
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

            var Objs = new List<GameObject>((int)objectCount);
            var myIDs = new List<int>((int)objectCount);
            var parentIDs = new List<int>((int)objectCount);
            
            for (int i = 0; i < objectCount; i++) // Note => Scene Objects doesnt collect children
            {
                GameObject newObj = new GameObject(autoAddTransform: false);

                int myId = reader.ReadInt32(); //
                
                // Read Parent Scene ID (-1 if none)
                int parentId = reader.ReadInt32(); 
                
                myIDs.Add(myId);
                parentIDs.Add(parentId);
                
                
                
                
                ushort objNameLen = reader.ReadUInt16(); //Read Object Name Length
                byte[] objNameData = reader.ReadBytes(objNameLen); //Read Object Name Data
                string objName = Encoding.UTF8.GetString(objNameData); //Decode Object Name

                newObj.Name = objName;

                uint compCount = reader.ReadUInt32(); //Read 

                for (int c = 0; c < compCount; c++)
                {
                    ushort typeID = reader.ReadUInt16();

                    if (!ComponentRegistry.factories.ContainsKey(typeID))
	                    throw new Exception($"Unknown Component Type ID: {typeID} in file. Loader would desync.");


                    Component newComp = ComponentRegistry.Deserialize(typeID, reader, newObj);

                    newObj.AssignComponent(newComp, false);

                }
                
                for (int a = 0; a < Objs.Count; a++)
                {
                	var obj = Objs[a];
                
                	// init Transform first so child-dirty propagation works
                	for (int c = 0; c < obj.Components.Count; c++)
                	{
                		if (obj.Components[c] is TransformComponent t)
                			t.Init(obj);
                	}
                
                	// then init everything else
                	for (int c = 0; c < obj.Components.Count; c++)
                	{
                		if (obj.Components[c] is not TransformComponent)
                			obj.Components[c].Init(obj);
                	}
                }


                
                if (newObj.Transform != null)
                	newObj.Transform.CalcModelMatrix();
                else
                	Console.WriteLine($"WARN: Object '{newObj.Name}' has no Transform after load.");
                	
                	
                Objs.Add(newObj);
            }
            
            // Build Lookup
            var idMap = new Dictionary<int, GameObject>(Objs.Count);
            for (int i = 0; i < Objs.Count; i++)
            	idMap[myIDs[i]] = Objs[i];
            
            foreach (var obj in Objs)
            {
            	obj.Children.Clear();
            	obj.Parent = null;
            }

            for (int i = 0; i < Objs.Count; i++)
            {
            	int pId = parentIDs[i];
            	if (pId >= 0 && idMap.TryGetValue(pId, out var parent))
            	{
            		parent.AssignChild(Objs[i]); // updates both sides
            	}
            }
            
            for (int i = 0; i < Objs.Count; i++)
            {
                Objs[i].Transform.MarkDirtySingle();
            }
            
            Console.WriteLine($"Name: {LevelName}");
            currentLevel = LevelName;
            reader.Close();

            Program.PhysicsEnabled = true; // Will always turn on physics when scene loaded
            Program.RenderingEnabled = true;

            
        }
        
        private static void Collect(GameObject obj, List<GameObject> outList)
        {
            outList.Add(obj);
            foreach (var child in obj.Children)
            {
                Collect(child, outList);
            }
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

        public static Component Deserialize(ushort typeID, BinaryReader reader, GameObject Owner)
        {
            if (!factories.ContainsKey(typeID))
            {
                throw new Exception($"Unknown Component Type: {typeID}");
            }
            Component comp = factories[typeID]();
            
            if (comp is ISerializable serializableComp)
            {
                comp.SetOwner(Owner);
                serializableComp.Deserialize(reader);
            }

            return comp;
        }

         
    }

}
