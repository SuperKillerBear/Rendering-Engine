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

            int objIndex = 0;
            foreach ((string[] names, uint[] meshIDs) in ObjectList) 
            {
                //Create Object
                GameObject newObject = new GameObject(Parent: Owner, name: "ChangeLater"); //Implement Passing Obj Names
                    
                var idCount = meshIDs.Count();

                if (idCount == 0) { meshAddress = "EMPTY"; objIndex++; continue; }

                for (int i = 0; i < idCount; i++)
                {
                    //Create Submesh GameObject
                    GameObject newSubMesh = new GameObject(Parent: newObject, name: names[i]);
                    var rend = newSubMesh.AddComponent<RendererComponent>();
                    newSubMesh.Transform.Scale = new Vector3D<float>(0.2f);

                    //Assign Mesh
                    rend.SetMeshID(meshIDs[i]);

                    //Mesh Address wont work for loading as scene, etc.
                    rend.meshAddress = $"{filename}/{newSubMesh.name}";

                    //Apply Material
                    var objList = ImportHandler.loadedObjMap[filename];
                    var obj = objList[objIndex];
                    Console.WriteLine($"THIS IS CAUSING THE ERROR: ID: {i}");
                    var submesh = obj[i]; //OUT OF RANGE ERROR WHEN i IS 1
                    rend.material = MaterialHandler.CreateMaterial(submesh.textureName, Vector3D<float>.One);
                }

                objIndex++;
            }
                

            //Finally Remove this Renderer Component
            Renderer.RenderingObjects.Remove(this);
            Owner.RemoveComponent(this);
            
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

            ImGui.InputText("Load Texture", ref inputTextureName, 64);
            if (ImGui.Button("Load Material"))
            {
                if (inputTextureName != "")
                {
                    material = MaterialHandler.CreateMaterial(inputTextureName, new Vector3D<float>(1));
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

            string data;

            if (material != null)
                data = material.Filename;
            else
                data = "EMPTY";

            byte[] encodedData = Encoding.UTF8.GetBytes(data);
            ushort dataLength = (ushort)encodedData.Length;

            writer.Write(dataLength);
            writer.Write(encodedData);

            writer.Write(material.Colour.X);
            writer.Write(material.Colour.Y);
            writer.Write(material.Colour.Z);
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

            this.material = MaterialHandler.CreateMaterial(materialAddress, colour);
        
            
        }
    }
}
