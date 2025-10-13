using RenderingEngine.GameObjects;
using RenderingEngine.Rendering;
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
            if (AssignedMesh)
            {
                ImGuiNET.ImGui.Text("Mesh is Assigned");
            }
            else
            {
                ImGuiNET.ImGui.Text("No Mesh Assigned");
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
