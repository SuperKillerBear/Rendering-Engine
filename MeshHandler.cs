using RenderingEngine.Meshes;
using RenderingEngine.RawObjData;
using RenderingEngine.Rendering;
using Silk.NET.Input.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RenderingEngine
{
    public static class MeshHandler
    {
        public static readonly List<Mesh> Meshes = new List<Mesh>();

        private static readonly Dictionary<string, uint> loadedMeshes = new();

        public static void Init()
        {
            AddDefaultMesh();
        }

        private static void AddDefaultMesh()
        {
            Meshes.Add(new CubeMesh());         //0
            Meshes.Add(new QuadMesh());         //1
            Meshes.Add(new TriangleMesh());     //2
        }

        public static Mesh GetMesh(uint ID)
        {
            if (ID < Meshes.Count)
            {
                return Meshes[(int)ID]; 
            }

            Console.WriteLine($"ERROR: Cannot get mesh of ID: {ID}");
            return Meshes[0];
            
        }

        public static uint? LoadMeshFile(string filename)
        {
            if (loadedMeshes.ContainsKey(filename)) return loadedMeshes[filename];

            //Dont include file type
            string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\MeshData\{filename}";
            
            try
            {
                var (vertsList, indsList) = ImportHandler.LoadObjFile(localPath);
                float[] verts = vertsList.ToArray();
                uint[] inds = indsList.ToArray();

                Mesh loadedMesh = new Mesh(verts, inds);

                uint id = (uint)Meshes.Count;
                Meshes.Add(loadedMesh);

                loadedMeshes.Add(filename, id);

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: Could not load mesh: {ex}");
                return null;
            }
            
        }

        public static void UnloadAll()
        {
            Meshes.Clear();
            AddDefaultMesh();
            loadedMeshes.Clear();
        }


    }
}
