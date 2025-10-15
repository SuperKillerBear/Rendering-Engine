using RenderingEngine.Meshes;
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

        public static readonly Dictionary<string, uint> loadedMeshes = new();

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

        public static List<(string[], uint[])> LoadMeshsFile(string filename)
        {
            List<(string[], uint[])> outObjects = new();
            if (loadedMeshes.ContainsKey(filename))
            {
                outObjects.Add(([filename], [loadedMeshes[filename]]));
                return outObjects;
            }

            //Dont include file type
            string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\MeshData\{filename}";
            
            try
            {
                ImportHandler.LoadObjFile(filename);
                var objs = ImportHandler.loadedObjMap[filename];

                List<uint> ids = new List<uint>();
                List<string> names = new List<string>();

                int objIndex = 0;
                foreach (var obj in objs)
                {
                    int meshIndex = 0;
                    foreach (var subMesh in obj)
                    {
                        float[] verts = subMesh.vertices.ToArray();
                        uint[] inds = subMesh.indices.ToArray();

                        Mesh loadedMesh = new Mesh(verts, inds);

                        uint id = (uint)Meshes.Count;

                        Console.WriteLine($"ID: {id}");

                        Meshes.Add(loadedMesh);

                        Console.WriteLine($"New Mesh Count: {Meshes.Count}");
                        string key = $"New Key: {filename}/{objIndex}/{meshIndex}/{subMesh.name}";
                        Console.WriteLine(key);
                        loadedMeshes.Add(key, id);

                        ids.Add(id);
                        names.Add(subMesh.name);
                        Console.WriteLine($"New IDs Values: {string.Join(", ", ids)}");

                        meshIndex++;
                    }
                    outObjects.Add((names.ToArray(), ids.ToArray()));
                    objIndex++;
                }
                
                return outObjects;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: Mesh Handler => Could not load mesh: {ex}"); //Throw Error
                outObjects.Add(([], []));
                return outObjects;
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
