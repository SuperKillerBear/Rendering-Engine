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

        private static readonly bool debugMesh = false;

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

        public static List<(string[] names, uint[] ids, int objectIndex)> LoadMeshsFile(string filename)
        {
            List<(string[], uint[], int)> outObjects = new();

            if (loadedMeshes.ContainsKey(filename)) //Not Catching same key??
            {
                // Return cached - need to reconstruct from keys
                outObjects.Add(([filename], [loadedMeshes[filename]], 0));
                return outObjects;
            }

            try
            {
                ImportHandler.LoadObjFile(filename);
                var objs = ImportHandler.loadedObjMap[filename];
                int objIndex = 0;

                foreach (var obj in objs)
                {
                    List<uint> ids = new List<uint>();
                    List<string> names = new List<string>();
                    int meshIndex = 0;

                    foreach (var subMesh in obj)
                    {
                        float[] verts = subMesh.vertices.ToArray();
                        uint[] inds = subMesh.indices.ToArray();

                        Mesh loadedMesh = new Mesh(verts, inds);
                        uint id = (uint)Meshes.Count;

                        Meshes.Add(loadedMesh);

                        string key = $"{filename}/{objIndex}/{meshIndex}";
                        if (debugMesh) Console.WriteLine($"Creating mesh - Key: {key}, ID: {id}, Name: {subMesh.name}");

                        loadedMeshes.Add(key, id);

                        ids.Add(id);
                        names.Add(subMesh.name);
                        meshIndex++;
                    }

                    // Only add if we actually created meshes
                    if (ids.Count > 0)
                    {
                        outObjects.Add((names.ToArray(), ids.ToArray(), objIndex));
                    }
                    else
                    {
                        if (debugMesh) Console.WriteLine($"WARNING: Object {objIndex} in {filename} has no submeshes!");
                    }

                    objIndex++;
                }

                return outObjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: Mesh Handler => Could not load mesh: {ex}");
                outObjects.Add(([], [], -1));
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
