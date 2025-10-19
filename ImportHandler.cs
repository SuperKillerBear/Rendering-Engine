using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderingEngine.Utilities;
using System.Runtime.CompilerServices;
using Silk.NET.Windowing;
using RenderingEngine.Rendering;

namespace RenderingEngine
{
    public static class ImportHandler
    {
        private static Dictionary<string, (string filename, Vector3D<float> baseColour)> 
            materialMap = new();

        public static Dictionary<string, List<List<(string name, string textureName, List<float> vertices, List<uint> indices)>>>
            loadedObjMap = new();

        public static void LoadObjFile(string filename)
        {
            if (loadedObjMap.ContainsKey(filename))
            { 
                Console.WriteLine($"Obj already loaded: {filename}"); 
                return; 
            }


            //TODO: Add failsafe if file doesnt exist

            string path = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\MeshData\{filename}\{filename}";

            // Raw per-file arrays
            var positions = new List<Vector3D<float>>();     // v
            var uvs = new List<Vector2D<float>>();           // vt (may be unused)
            var normals = new List<Vector3D<float>>();       // vn

            // Final interleaved data and indices
            var outFloats = new List<float>(); // x,y,z, r,g,b, u,v
            
            var outIndices = new List<uint>();

            // Map to deduplicate (pos,uv,norm,material) -> index
            var indexMap = new Dictionary<(int p, int t, int n, string mat), uint>();

            // Material color map (string -> rgb)
            LoadMaterials(path, filename);

            string currentMaterialName = "default";


            bool firstObject = true;
            bool firstMaterial = true;

            List<List<(string filename, string textureName, List<float> vertices, List<uint> indices)>> Objects = new();
            List<(string filename, string textureName, List<float> vertices, List<uint> indices)> SubMeshes = new();

            string currentObjectName = "";

            

            static int ParseObjIndex(string token, int listCount)
            {
                // empty => -1
                if (string.IsNullOrEmpty(token)) return -1;
                if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int idx)) return -1;
                if (idx > 0) return idx - 1;
                // negative index: relative to end
                return listCount + idx;
            }
            int RECOGNISEDOBJECTS = 0;
            int lineNum = 0;
            // Read OBJ
            foreach (var rawLine in File.ReadLines($"{path}.obj"))
            {
                lineNum++;
                var line = rawLine.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                switch (parts[0])
                {
                    case "o":
                        RECOGNISEDOBJECTS++;
                        Console.WriteLine($"Recognised Objects: {RECOGNISEDOBJECTS} on Line: {lineNum}");
                        if (!firstObject)
                        {
                            if (!firstMaterial && outFloats.Count > 0)
                            {
                                SubMeshes.Add((currentObjectName, currentMaterialName, new List<float>(outFloats), new List<uint>(outIndices)));
                            }
                            //Wont be null ADD TO OBJECTS LIST
                            Objects.Add(SubMeshes);
                            outFloats.Clear();
                            outIndices.Clear();
                            indexMap.Clear();
                            SubMeshes = new();
                        }

                        // Create a new submesh list for the new object
                        SubMeshes = new List<(string filename, string textureName, List<float> vertices, List<uint> indices)>();

                        currentObjectName = parts[1];
                        firstObject = false;
                        break;
                    case "v":
                        if (parts.Length < 4) continue;
                        positions.Add(new Vector3D<float>(ParseF(parts[1]), ParseF(parts[2]), ParseF(parts[3])));
                        break;

                    case "vt":
                        if (parts.Length < 3) continue;
                        uvs.Add(new Vector2D<float>(ParseF(parts[1]), ParseF(parts[2])));
                        break;

                    case "vn":
                        if (parts.Length < 4) continue;
                        normals.Add(new Vector3D<float>(ParseF(parts[1]), ParseF(parts[2]), ParseF(parts[3])));
                        break;

                    case "usemtl":
                        if (parts.Length >= 2)
                        {
                            if (!firstMaterial && outFloats.Count == 0)
                            {
                                Console.WriteLine($"EMPTY SUBMESH DETECTED: {SubMeshes.Count}, SKIPPING");
                            }

                            // ensure material exists in map (auto-generate if necessary)
                            if (!firstMaterial && outFloats.Count > 0)
                            {
                                Console.WriteLine("");
                                SubMeshes.Add((currentObjectName, currentMaterialName, new List<float>(outFloats), new List<uint>(outIndices)));
                                outFloats.Clear();
                                outIndices.Clear();
                                indexMap.Clear();
                            }

                            string key = $"{filename}/{currentMaterialName}";

                            if (!materialMap.ContainsKey(key))
                            {
                                Console.WriteLine($"ColourMap Doesnt Contain Colour for Key: {key}");
                                materialMap[key] = ($"EMPTY", new Vector3D<float>(1f, 0f, 1f));

                            }
                            else { Console.WriteLine($"Loaded key: {key}, Data: {materialMap[key]}"); }

                            //Generate Texture such that its stored in the MaterialHandler Dictionary for the filename for later
                            //TODO: Doesnt support basecolour, im lazy rn

                            //Creating Key for handle and textID
                            currentMaterialName = parts[1];
                            firstMaterial = false;
                            
                            MaterialHandler.GetTexture(filename, materialMap[key].filename); //Filename is wrong
                        }
                        break;

                    case "f":
                        // parts[1..] are tokens like "v/vt/vn" (triangles or polygons)
                        if (parts.Length < 4) continue; // need at least 3 vertices

                        // parse all vertex tokens for this face
                        var faceTokens = parts.Skip(1).ToArray();
                        var faceRefs = new (int p, int t, int n)[faceTokens.Length];

                        for (int i = 0; i < faceTokens.Length; i++)
                        {
                            var tok = faceTokens[i];
                            var sub = tok.Split('/');
                            int pIdx = ParseObjIndex(sub.Length > 0 ? sub[0] : "", positions.Count);
                            int tIdx = ParseObjIndex(sub.Length > 1 ? sub[1] : "", uvs.Count);
                            int nIdx = ParseObjIndex(sub.Length > 2 ? sub[2] : "", normals.Count);
                            faceRefs[i] = (pIdx, tIdx, nIdx);
                        }

                        // triangulate face as fan around vertex 0
                        for (int i = 1; i < faceRefs.Length - 1; i++)
                        {
                            var a = faceRefs[0];
                            var b = faceRefs[i];
                            var c = faceRefs[i + 1];

                            // Optionally compute geometric normal and compare to stated normals
                            // If you want flipping logic, you can compute triNormal here and compare to file normals.
                            // For now we keep order as-is (Blender usually exports correct winding).

                            // Add or reuse vertex a,b,c
                            AddVertex(a.p, a.t, a.n, currentMaterialName, positions, uvs, normals, indexMap, outFloats, outIndices);
                            AddVertex(b.p, b.t, b.n, currentMaterialName, positions, uvs, normals, indexMap, outFloats, outIndices);
                            AddVertex(c.p, c.t, c.n, currentMaterialName, positions, uvs, normals, indexMap, outFloats, outIndices);
                        }
                        break;

                    default:
                        // ignore other directives
                        break;
                }


            }

            //Add Final Object
            if (outFloats.Count > 0) SubMeshes.Add((currentObjectName, currentMaterialName, new List<float>(outFloats), new List<uint>(outIndices)));
            
            Objects.Add(SubMeshes); //Wont Be null

            loadedObjMap.Add(filename, Objects);

            

            // ---------- Local helpers ----------
            static void AddVertex(int pIdx, int tIdx, int nIdx, string mat,
                List<Vector3D<float>> positions,
                List<Vector2D<float>> uvs,
                List<Vector3D<float>> normals,                
                Dictionary<(int p, int t, int n, string mat), uint> indexMap,
                List<float> outFloats,
                List<uint> outIndices)
            {
                var key = (pIdx, tIdx, nIdx, mat);
                if (!indexMap.TryGetValue(key, out uint index))
                {
                    // position
                    Vector3D<float> pos = (pIdx >= 0 && pIdx < positions.Count) ? positions[pIdx] : new Vector3D<float>(0f, 0f, 0f);

                    // choose color from material map (fallback to Pink)
                    Vector3D<float> col = materialMap.TryGetValue(mat, out var mc) ? mc.Item2 : new Vector3D<float>(1f, 0f, 1f);

                    Vector2D<float> uv = (tIdx >= 0 && tIdx < uvs.Count)
                        ? uvs[tIdx]
                        : new Vector2D<float>(0f, 0f);

                    // append interleaved vertex: x,y,z, r,g,b, u,v
                    outFloats.Add(pos.X);
                    outFloats.Add(pos.Y);
                    outFloats.Add(pos.Z);

                    outFloats.Add(col.X);
                    outFloats.Add(col.Y);
                    outFloats.Add(col.Z);

                    outFloats.Add(uv.X);
                    outFloats.Add(uv.Y);

                    index = (uint)(outFloats.Count / 8 - 1);
                    indexMap[key] = index;
                }
                outIndices.Add(index);
            }

        }


        static void LoadMaterials(string path, string filename)
        {
            string currentMaterial = "EMPTY";
            string currentFilename = "EMPTY";
            Vector3D<float> currentKd = Vector3D<float>.One;

            foreach (var rawLine in   File.ReadLines($"{path}.mtl"))
            {
                var line = rawLine.Trim();

                if (line.Length == 0 || line[0] == '#') continue;

                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) continue;

                

                switch (parts[0])
                {
                    case "newmtl":
                        //New Material
                        if (parts.Length < 2) continue;

                        if (currentMaterial != "EMPTY")
                        {
                            //Not First => upload
                            //materialMap KEY = filename/currentMaterial => (filename, baseColour)
                            materialMap.Add($"{filename}/{currentMaterial}", (currentMaterial, currentKd));
                        }

                        currentMaterial = parts[1];
                        currentFilename = "EMPTY";
                        currentKd = Vector3D<float>.One;
                        break;
                    case "Kd":
                        //Ambient Colour of Material
                        if (currentMaterial == "" || parts.Length < 4) continue;

                        Console.WriteLine($"1: {parts[1]}, 2: {parts[2]}, 3: {parts[3]}");

                        currentKd = new Vector3D<float>(
                            ParseF(parts[1]),
                            ParseF(parts[2]),
                            ParseF(parts[3])
                            );

                        
                        
                        break;
                    case "map_Kd":
                        string texPath = @$"{parts[1]}";
                        string texPathName = Path.GetFileNameWithoutExtension(path);
                        currentFilename = texPathName;
                        break;
                }


            }

            //Add Final Material
            materialMap.Add($"{filename}/{currentMaterial}", (currentMaterial, currentKd));
        }

        // Helper local funcs:
        static float ParseF(string s) => float.Parse(s, CultureInfo.InvariantCulture);

        
    }
}
