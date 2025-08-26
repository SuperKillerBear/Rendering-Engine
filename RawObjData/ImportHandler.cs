using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderingEngine.Utilities;

namespace RenderingEngine.RawObjData
{
    public static class ImportHandler
    {
        private static Dictionary<string, Vector3D<float>> materialMap = new();

        public static (List<float> vertices, List<uint> indices) LoadObjFile(string path)
        {
            //TODO: Add failsafe for 

            // Raw per-file arrays
            var positions = new List<Vector3D<float>>();     // v
            var uvs = new List<Vector2D<float>>();           // vt (may be unused)
            var normals = new List<Vector3D<float>>();       // vn

            // Final interleaved data and indices
            var outFloats = new List<float>(); // x,y,z, r,g,b
            var outIndices = new List<uint>();

            // Map to deduplicate (pos,uv,norm,material) -> index
            var indexMap = new Dictionary<(int p, int t, int n, string mat), uint>();

            // Material color map (string -> rgb)
            LoadMaterials(path);

            string currentMaterial = "default";

            

            static int ParseObjIndex(string token, int listCount)
            {
                // empty => -1
                if (string.IsNullOrEmpty(token)) return -1;
                if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int idx)) return -1;
                if (idx > 0) return idx - 1;
                // negative index: relative to end
                return listCount + idx;
            }

            // Read OBJ
            foreach (var rawLine in File.ReadLines($"{path}.obj"))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                switch (parts[0])
                {
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
                            currentMaterial = parts[1];
                            // ensure material exists in map (auto-generate if necessary)
                            if (!materialMap.ContainsKey(currentMaterial))
                            {
                                Console.WriteLine($"ColourMap Doesnt Contain Colour for Key: {currentMaterial}");
                                materialMap[currentMaterial] = new Vector3D<float>(1f, 0f, 1f);
                            }
                            else { Console.WriteLine($"Colour Loaded key: {currentMaterial}, RGB: {materialMap[currentMaterial]}"); }
                            
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
                            AddVertex(a.p, a.t, a.n, currentMaterial, positions, uvs, normals, indexMap, outFloats, outIndices);
                            AddVertex(b.p, b.t, b.n, currentMaterial, positions, uvs, normals, indexMap, outFloats, outIndices);
                            AddVertex(c.p, c.t, c.n, currentMaterial, positions, uvs, normals, indexMap, outFloats, outIndices);
                        }
                        break;

                    default:
                        // ignore other directives
                        break;
                }
            }

            return (outFloats, outIndices);

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
                    Vector3D<float> col = materialMap.TryGetValue(mat, out var mc) ? mc : new Vector3D<float>(1f, 0f, 1f);

                    // append interleaved vertex: x,y,z, r,g,b
                    outFloats.Add(pos.X);
                    outFloats.Add(pos.Y);
                    outFloats.Add(pos.Z);
                    outFloats.Add(col.X);
                    outFloats.Add(col.Y);
                    outFloats.Add(col.Z);

                    index = (uint)(outFloats.Count / 6 - 1);
                    indexMap[key] = index;
                }
                outIndices.Add(index);
            }

        }


        static void LoadMaterials(string path)
        {
            string currentMaterial = "";

            foreach (var rawLine in File.ReadLines($"{path}.mtl"))
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
                        currentMaterial = parts[1];
                        break;
                    case "Kd":
                        //Ambient Colour of Material
                        if (currentMaterial == "" || parts.Length < 4) continue;

                        Console.WriteLine($"1: {parts[1]}, 2: {parts[2]}, 3: {parts[3]}");

                        Vector3D<float> colour = new Vector3D<float>(
                            ParseF(parts[1]),
                            ParseF(parts[2]),
                            ParseF(parts[3])
                            );

                        materialMap[currentMaterial] = colour;
                        break;
                }


            }
        }

        // Helper local funcs:
        static float ParseF(string s) => float.Parse(s, CultureInfo.InvariantCulture);

        /*
        //Create Dictionary to be able to reference Materials
        private static Dictionary<string, Vector3D<float>> materialMap = new();

        public static (List<float> vertices, List<uint> indices) LoadObjFile(string path,
            Dictionary<string, (float r, float g, float b)>? materialColours = null)
        {
            var vertexData = new List<float>(); //Includes Position + Colour
            var uvs = new List<Vector2D<float>>();
            var normals = new List<Vector3D<float>>();

            var outIndices = new List<uint>();
            var outVertexData = new List<float>();

            bool loadedMaterials = false;

            Vector3D<float>? currentColour = null;
                        

            foreach (var rawLine in File.ReadLines($"{path}.obj"))
            {
                var line = rawLine.Trim();

                if (line.Length == 0 || line[0] == '#') continue;

                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) continue;

                switch (parts[0])
                {
                    case "v":
                        //Vertex Position x y z
                        if (parts.Length < 4) continue;

                        if (!loadedMaterials)
                        {
                            LoadMaterials(path);
                            loadedMaterials = true;
                        }

                        //Adding Vertex Position
                        vertexData.Add(ParseFloat(parts[1]));
                        vertexData.Add(ParseFloat(parts[2]));
                        vertexData.Add(ParseFloat(parts[3]));
                        
                        //Adding Vertex Colour
                        if (currentColour != null)
                        {
                            //using material colour
                            vertexData.Add(currentColour.Value.X);
                            vertexData.Add(currentColour.Value.Y);
                            vertexData.Add(currentColour.Value.Z);
                        }
                        else
                        {
                            //Add default colour
                            vertexData.Add(1.00f);
                            vertexData.Add(1.00f);
                            vertexData.Add(1.00f);
                        }

                        break;
                    case "vt":
                        //Texture Coordinate
                        if (parts.Length < 3) continue;
                        uvs.Add(new Vector2D<float>(
                            ParseFloat(parts[1]),
                            ParseFloat(parts[2])
                            ));
                        
                        break;
                    case "vn":
                        //Vertex Normal
                        normals.Add(new Vector3D<float>(
                            ParseFloat(parts[1]),
                            ParseFloat(parts[2]),
                            ParseFloat(parts[3])
                            ));
                        break;
                    case "usemtl":
                        if (!loadedMaterials)
                        {
                            LoadMaterials(path);
                            loadedMaterials = true;
                        }

                        if (parts.Length < 2) continue;

                        if (materialMap.ContainsKey(parts[1]))
                        {
                            currentColour = materialMap[parts[1]];
                        }
                        else
                        {
                            Console.WriteLine($"Cannot find key in Material Map for key: {parts[1]}");
                            currentColour = null;
                        }


                            break;
                    case "f":
                        //Face Indices
                        //f position index / textcoord index / normal index

                        if (parts.Length < 10) continue; //Check the "/" isnt included in parts

                        //Grab Indices from data
                        //Indices at parts 1, 4, 7
                        uint Ind0 = ParseUInt(parts[1]);
                        uint Ind1 = ParseUInt(parts[4]);
                        uint Ind2 = ParseUInt(parts[7]);

                        int loc0 = ((int) (Ind0) - 1) * 6; //Stride * row for index
                        int loc1 = ((int) (Ind1) - 1) * 6; //Stride * row for index
                        int loc2 = ((int) (Ind2) - 1) * 6; //Stride * row for index

                        var v0 =  new Vector3D<float>(vertexData[loc0], vertexData[loc0 + 1], vertexData[loc0 + 2]);
                        var v1 = new Vector3D<float>(vertexData[loc1], vertexData[loc1 + 1], vertexData[loc1 + 2]);
                        var v2 = new Vector3D<float>(vertexData[loc1], vertexData[loc2 + 1], vertexData[loc2 + 2]);

                        var edge1 = v1 - v0;
                        var edge2 = v2 - v0;

                        Vector3D<float> calculatedNormal = UMath.Normalize(UMath.Cross(edge1, edge2));

                        //Compare Calc vs stated normals
                        int statedNormalIndex = (int)(ParseUInt(parts[3])) - 1;
                        Vector3D<float> statedNormal = normals[statedNormalIndex];

                        //If backwards then swap indices b, c
                        if (UMath.Dot(calculatedNormal, statedNormal) < 0f)
                        {
                            //Swapped Ind2 with Ind1 for other direction
                            outIndices.Add(Ind0);
                            outIndices.Add(Ind2); 
                            outIndices.Add(Ind1);
                        }
                        else
                        {
                            outIndices.Add(Ind0);
                            outIndices.Add(Ind1);
                            outIndices.Add(Ind2);
                        }

                        break;
                        
                }
            }

            return (vertexData, outIndices);
        }
        
        
        static void LoadMaterials(string path)
        {
            string currentMaterial = "";

            foreach (var rawLine in File.ReadLines($"{path}.mtl"))
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
                        currentMaterial = parts[1];
                        break;
                    case "Ka":
                        //Ambient Colour of Material
                        if (currentMaterial == "" || parts.Length < 4) continue;

                        Vector3D<float> colour = new Vector3D<float>(
                            ParseFloat(parts[1]), 
                            ParseFloat(parts[2]),
                            ParseFloat(parts[3])
                            );

                        materialMap[currentMaterial] = colour;
                        break;
                }

                
            }
        }

        static float ParseFloat(string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }

        static uint ParseUInt(string str)
        {            
            return uint.Parse(str, CultureInfo.InvariantCulture);
        }


        */
    }
}
