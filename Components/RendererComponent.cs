using ImGuiNET;
using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
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


        public Material? material = null;

        private string inputTextureName = "";
        private string inputMeshName = "";

        public override void Init(GameObject Owner)
        {
            base.Init(Owner); //Init + Set Owner
            Renderer.RenderingObjects.Add(this);

        }

        //Multiple Objects can be loaded from one file, thus not a singular mesh can be returned
        //Bad Name
        public void SetMesh(string filename)
        {
            var ObjectList = MeshHandler.LoadMeshsFile(filename);

            foreach ((string[] names, uint[] meshIDs, int sourceObjIndex) in ObjectList)
            {
                var idCount = meshIDs.Length;
                if (idCount == 0)
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
                    newSubMesh.Transform.Scale(new Vector3D<float>(0.2f));

                    // Assign Mesh
                    rend.SetMeshID(meshIDs[i]);
                    rend.meshAddress = $"{filename}/{sourceObjIndex}/{i}";

                    // Apply Material - Now safely access submesh
                    var submesh = sourceObj[i];

                    // Create material with texture if available
                    string textureName = string.IsNullOrEmpty(submesh.textureName) ? "EMPTY" : submesh.textureName;
                    rend.material = MaterialHandler.CreateMaterial(filename, textureName, Vector3D<float>.One);

                    Console.WriteLine($"Created submesh: {names[i]} with texture: {textureName}");
                }
            }

            // Finally Remove this Renderer Component
            Renderer.RenderingObjects.Remove(this);
            if (Owner != null) Owner.RemoveComponent(this);
        }



        public void SetMeshID(uint id)
        {
            if (MeshHandler.GetMesh(id) != MeshHandler.Meshes[0])
            {
                this.MeshID = id;
                this.AssignedMesh = true;
            }
            
        }

        public override void OnInspectorGUI()
        {
            ImGui.Text($"Mesh Address: {meshAddress}");
            ImGui.Text($"Mesh ID: {MeshID.ToString()}");
            ImGui.Text($"Assigned Mesh: {AssignedMesh.ToString()}");

            ImGui.InputText("Load Mesh", ref inputMeshName, 64);
            if (ImGui.Button("Set Mesh"))
            {
                SetMesh(inputMeshName);
            }

            ImGui.InputText("Load Texture", ref inputTextureName, 64);
            if (ImGui.Button("Load Material"))
            {
                if (inputTextureName != "")
                {
                    material = MaterialHandler.CreateMaterial("", inputTextureName, new Vector3D<float>(1));
                }
            }

            if (material != null)
            {
                ImGui.Text($"Material: {material.Filename}");
                ImGui.Text($"Colour: {material.Colour.ToString()}");
                ImGui.Text($"Handle: {material.BindlessHandle.ToString()}");
                ImGui.Text($"Texture ID: {material.TextureID.ToString()}");
            }
            else
            {
                ImGui.Text("Material: NONE");
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            //TODO: Check if string is empty, if so write "EMPTY"

            byte[] meshAddressData = Encoding.UTF8.GetBytes(meshAddress);
            ushort addreessLength = (ushort)meshAddressData.Length;

            writer.Write(addreessLength);
            writer.Write(meshAddressData);

            bool hasMaterial = material != null;
            string data;

            data = hasMaterial ? material.Filename : "EMPTY";

            byte[] encodedData = Encoding.UTF8.GetBytes(data);
            ushort dataLength = (ushort)encodedData.Length;

            writer.Write(dataLength);
            writer.Write(encodedData);

            if (hasMaterial)
            {
                writer.Write(material.Colour.X);
                writer.Write(material.Colour.Y);
                writer.Write(material.Colour.Z);
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

            string meshAddress = Encoding.UTF8.GetString(meshAddressData);

            Console.WriteLine($"Loaded Mesh Address Data: {meshAddress}");
            SetMesh(meshAddress);

            length = reader.ReadUInt16();
            byte[] materialData = reader.ReadBytes(length);
            string materialAddress = Encoding.UTF8.GetString(materialData);

            Vector3D<float> colour = new Vector3D<float>(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );

            //Fix Later
            this.material = MaterialHandler.CreateMaterial("", materialAddress, colour);
        
            
        }
    }
}
