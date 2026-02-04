using ImGuiNET;
using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
using RenderingEngine.Utilities;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngine.Components
{
    public class RendererComponent : Component, ISerializable
    {
        public override string ComponentName => "Renderer Component";
        private bool AssignedMesh = false;
        public uint MeshID { get; private set; }
        private string meshAddress = "EMPTY";


        public Material? Material = null;

        private string GuiInputTextureName = "";
        private string GuiInputMeshName = "";

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner
            Renderer.RenderingObjects.Add(this);

        }

        //Bad Name
        public void SetMesh(string filename)
        {
            var ObjectList = MeshHandler.LoadMeshsFile(filename);

            //Note* Source

            foreach ((string[] names, uint[] meshIDs, int sourceObjIndex) in ObjectList)
            {
                //Cant Handle when no meshID, but all source ObjIndex = -1
                var idCount = meshIDs.Length;
                if (sourceObjIndex == -1)
                {
                    // Create Submesh GameObject
                    GameObject newObj = new GameObject(name: $"UnknownMeshObject");
                    var rend = newObj.AddComponent<RendererComponent>();
                    newObj.Transform.Scale(new Vector3D<float>(0.2f));

                    // Assign Mesh
                    rend.SetMeshID(0);
                    rend.meshAddress = $"{filename}/{sourceObjIndex}";

                    // Create material with texture if available
                    string textureName = "EMPTY";

                    //Note* If Texture name is passed to be "EMPTY" => Debug Texture is loaded automatically
                    rend.Material = MaterialHandler.CreateMaterial(filename, textureName, Vector3D<float>.One); 
                    
                    
                    Console.WriteLine("Rendered Empty Object");
                    continue;
                }
                else if (idCount == 0)
                {
                    Console.WriteLine($"Skipping empty object in {filename}");
                    continue;
                }

                // Create parent object for this OBJ
                //string objName = $"{filename}_obj_{sourceObjIndex}";
                string objName = $"{names[0]}";
                GameObject newObject = new GameObject(Parent: Owner, name: objName);
                

                // Get the source object data
                var objList = ImportHandler.loadedObjMap[filename];

                // Validate object index
                if (sourceObjIndex < 0 || sourceObjIndex >= objList.Count)
                {
                    Console.WriteLine($"ERROR: Invalid object index {sourceObjIndex} for {filename}");
                    continue;
                }

                var sourceObj = objList[sourceObjIndex];

                for (int i = 0; i < idCount; i++)
                {
                    // Validate submesh index
                    if (i >= sourceObj.Count)
                    {
                        Console.WriteLine($"ERROR: Submesh index {i} out of range for object {sourceObjIndex} (has {sourceObj.Count} submeshes)");
                        break;
                    }

                    // Create Submesh GameObject
                    GameObject newSubMesh = new GameObject(Parent: newObject, name: $"Submesh {i}");
                    var rend = newSubMesh.AddComponent<RendererComponent>();
                    

                    // Assign Mesh
                    rend.SetMeshID(meshIDs[i]);
                    rend.meshAddress = $"{filename}/{sourceObjIndex}/{i}";
                
                    // Apply Material - Now safely access submesh
                    var submesh = sourceObj[i];

                    // Create material with texture if available
                    string textureName = string.IsNullOrEmpty(submesh.textureName) ? "EMPTY" : submesh.textureName;

                    //Note* If Texture name is passed to be "EMPTY" => Debug Texture is loaded automatically
                    rend.Material = MaterialHandler.CreateMaterial(filename, textureName, Vector3D<float>.One); 

                    Console.WriteLine($"Created submesh: {names[i]} with texture: {textureName}");
                }
            }

            // Finally Remove this Renderer Component
            Renderer.RenderingObjects.Remove(this);
            if (Owner != null) {
                Owner.RemoveComponent(this);
                Owner.Transform.Scale(new Vector3D<float>(0.2f));
            }
        }

        public void SetMeshID(uint id)
        {
            //Checking if != Cube Mesh
            if (MeshHandler.GetMesh(id) != MeshHandler.Meshes[0]) 
            {
                this.MeshID = id;
                this.AssignedMesh = true;
            }
            else
            {
                this.MeshID = 0;
                this.AssignedMesh = false;
            }
            
        }

        public override void OnInspectorGUI()
        {
            ImGui.Text($"Mesh Address: {meshAddress}");
            ImGui.Text($"Mesh ID: {MeshID.ToString()}");
            ImGui.Text($"Assigned Mesh: {AssignedMesh.ToString()}");

            ImGui.InputText("Load Mesh", ref GuiInputMeshName, 64);
            if (ImGui.Button("Set Mesh"))
            {
                SetMesh(GuiInputMeshName);
            }

            ImGui.InputText("Load Texture", ref GuiInputTextureName, 64);
            if (ImGui.Button("Load Material"))
            {
                if (GuiInputTextureName != "")
                {
                    Material = MaterialHandler.CreateMaterial("", GuiInputTextureName, new Vector3D<float>(1));
                }
            }

            if (Material != null)
            {
                ImGui.Text($"Material: {Material.Filename}");
                ImGui.Text($"Colour: {Material.Colour.ToString()}");
                ImGui.Text($"Handle: {Material.BindlessHandle.ToString()}");
                ImGui.Text($"Texture ID: {Material.TextureID.ToString()}");
            }
            else
            {
                ImGui.Text("Material: NONE");
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            byte[] meshAddressData = Encoding.UTF8.GetBytes(meshAddress);
            ushort addressLength = (ushort)meshAddressData.Length;

            writer.Write(addressLength);
            writer.Write(meshAddressData);

            bool hasMaterial = Material != null;
            string data;

            data = hasMaterial ? Material.Filename : "EMPTY";

            byte[] encodedData = Encoding.UTF8.GetBytes(data);
            ushort dataLength = (ushort)encodedData.Length;

            writer.Write(dataLength);
            writer.Write(encodedData);

            if (hasMaterial)
            {
                UMath.WriteSilkVec3(writer, Material.Colour);
            }
            else
            {
                writer.Write(1f);
                writer.Write(1f);
                writer.Write(1f);
            }
                

            
        }

        public void Deserialize(BinaryReader reader)
        {
            ushort length = reader.ReadUInt16();
            byte[] meshAddressData = reader.ReadBytes(length);

            meshAddress = Encoding.UTF8.GetString(meshAddressData);
            Console.WriteLine($"Loaded Mesh Address Data: {meshAddress}");
            
            
            length = reader.ReadUInt16();
            byte[] materialData = reader.ReadBytes(length);
            string materialAddress = Encoding.UTF8.GetString(materialData);
            var parts = meshAddress.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            var colour = UMath.ReadSilkVec3(reader);
            
            if (parts.Length < 1)
            {
                SetMesh("EMPTY");
            }
            else
            {
                string file = parts[0];
                
                MeshHandler.LoadMeshsFile(file);
                if (MeshHandler.loadedMeshes.TryGetValue(meshAddress, out uint id))
                {
                    SetMeshID(id);
                    Material = MaterialHandler.CreateMaterial(file, materialAddress, colour);
                }
                else
                {
                    Console.WriteLine($"WARN: Mesh Key not found after load: {meshAddress}");
                    SetMesh(file); //Rebuilds entire object
                }
            }

        
        }
    }
}
