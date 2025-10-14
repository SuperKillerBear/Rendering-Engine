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

        public void SetMesh(string filename)
        {
            uint? meshID = MeshHandler.LoadMeshFile(filename);
            if (meshID != null)
            {
                meshAddress = filename;
                AssignedMesh = true;
                MeshID = meshID.Value;
            }
            else
            {
                meshAddress = "EMPTY";
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
        }

        public void Deserialize(BinaryReader reader)
        {
            ushort length = reader.ReadUInt16();
            byte[] meshAddressData = reader.ReadBytes(length);

            string meshAddress = Encoding.UTF8.GetString(meshAddressData);

            Console.WriteLine($"Loaded Mesh Address Data: {meshAddress}");
            SetMesh(meshAddress);
        }
    }
}
