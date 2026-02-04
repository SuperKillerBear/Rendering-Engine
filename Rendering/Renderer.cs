using RenderingEngine.Meshes;
using RenderingEngine.GameObjects;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Silk.NET.Core.Native.WinString;
using Silk.NET.OpenGL.Extensions.ImGui;
using ImGuiNET;
using RenderingEngine.Components;
using Silk.NET.OpenGL.Extensions.ARB;
using System.Text.Json;
using RenderingEngine.Utilities;


namespace RenderingEngine.Rendering
{
    public class Renderer
    {
        private GL gl;
        private DebugLines debugLines;
        

        private uint shaderProgram;
        private int uModelLocation;

        //TEMPORARY
        //public static DynamicObject[] dynObjs; 
        public static List<RendererComponent> RenderingObjects = new List<RendererComponent>();


        int uViewLocation, uProjectionLocation, uColorLocation;
        int uTextureLocation, uLightDirLocation, uLightColourLocation, uAmbientColorLocation;
        
        
        public static float SunIntensity = 1.5f, AmbientIntensity = 4f;

        public Renderer(GL gl, uint shaderProgram)
        {
            this.gl = gl;
            this.shaderProgram = shaderProgram;
            uModelLocation = gl.GetUniformLocation(shaderProgram, "uModel");

            //Enable Depth Testing
            gl.Enable(GLEnum.DepthTest);
            gl.DepthFunc(GLEnum.Less);

            //Cull Backfaces to boost preformance
            gl.Enable(GLEnum.CullFace);
            gl.CullFace(GLEnum.Back);

            //Enable Shader Program
            gl.UseProgram(shaderProgram);


            uViewLocation = gl.GetUniformLocation(shaderProgram, "uView");
            uProjectionLocation = gl.GetUniformLocation(shaderProgram, "uProjection");
            uColorLocation = gl.GetUniformLocation(shaderProgram, "uBaseColor");

            uTextureLocation = gl.GetUniformLocation(shaderProgram, "uTexture"); //not -1

            uLightDirLocation = gl.GetUniformLocation(shaderProgram, "uLightDir");
            uLightColourLocation = gl.GetUniformLocation(shaderProgram, "uLightColor");
            uAmbientColorLocation = gl.GetUniformLocation(shaderProgram, "uAmbientColor");

            debugLines = new DebugLines(gl, "Shaders/debug_lines.vert", "Shaders/debug_lines.frag");

        }

        public void Clear()
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
        }

        public void Draw()
        {
            gl.UseProgram(shaderProgram);

            var meshGroups = RenderingObjects.GroupBy(obj => obj.MeshID);

            // Orthographic projection example
            //Matrix4X4<float> projection = Matrix4X4.CreateOrthographic(2f, 2f, 0.1f, 10f);

            float fov = Camera.FOV == 0 ? (int) Math.PI / 2 : Camera.FOV * (MathF.PI / 180f); //Disallow 0 FOV

            Matrix4X4<float> projection =
                Matrix4X4.CreatePerspectiveFieldOfView(
                    fieldOfView: fov, // 60°
                    aspectRatio: Program.aspectRatio,
                    nearPlaneDistance: 0.1f,
                    farPlaneDistance: 1000f
                );

            //TODO: Make Only Calc on Update
            Matrix4X4<float> view =
                Matrix4X4.CreateLookAt(Camera.Position,
                Camera.Position + Camera.Forward,
                Camera.Up);


                    

            if (uTextureLocation == -1)
            {
                Console.WriteLine("WARN: uTexture uniform location is -1 (not found)");
            }

            unsafe
            {
                
                gl.UniformMatrix4(uViewLocation, 1, false, (float*)&view);
                gl.UniformMatrix4(uProjectionLocation, 1, false, (float*)&projection);

                Vector3D<float> calcLightDir = UMath.Normalize(new Vector3D<float>(0, 1, 0));
                gl.Uniform3(uLightDirLocation, calcLightDir.X, calcLightDir.Y, calcLightDir.Z); //Normalise Value
                gl.Uniform3(uLightColourLocation, 1.0f * SunIntensity, 1.0f * SunIntensity, 1.0f * SunIntensity);
                gl.Uniform3(uAmbientColorLocation, 0.2f * AmbientIntensity, 0.2f * AmbientIntensity, 0.2f * AmbientIntensity);

            }


            foreach (var group in meshGroups)
            {
                Mesh mesh = MeshHandler.GetMesh(group.Key);
                if (mesh == null) continue;

                gl.BindVertexArray(mesh.VAO);

                foreach (var rendrComp in group)
                {
                    //Implememt GPU more efficent method such that data upload only new data
                    //Not same data each frame
                    Matrix4X4<float> model = rendrComp.Owner.Transform.GetModelMatrix();

                    //Set Material
                    Material mat = rendrComp.Material ?? MaterialHandler.defaultMaterial;

                    // Set material properties
                    gl.Uniform3(uColorLocation, mat.Colour.X, mat.Colour.Y, mat.Colour.Z);
                    
        

                    // Upload the bindless texture handle
                    if (mat.BindlessHandle != 0)
                    {
                        //Are Different Handles
                        MaterialHandler.bindless.ProgramUniformHandle(shaderProgram, uTextureLocation, mat.BindlessHandle);

                    }
                    else
                        MaterialHandler.bindless.ProgramUniformHandle(shaderProgram, uTextureLocation, MaterialHandler.defaultHandle);



                    unsafe
                    {
                        gl.GetInteger(GLEnum.CurrentProgram, out int curProg);
                        if ((uint)curProg != shaderProgram)

                        {
                            Console.WriteLine($"WRONG PROGRAM before uModel: cur={curProg} expected={shaderProgram}");
                        }
                        
                        //UploadMat4(uModelLocation, model);
                        gl.UniformMatrix4(uModelLocation, 1, false, (float*)&model);


                        if (mesh.IndexCount > 0)
                        {
                            //Sharing Vertices so more Efficient
                            gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.IndexCount, DrawElementsType.UnsignedInt, null);
                        }
                        else
                        {
                            // Draw the mesh
                            gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.VertexCount);
                        }


                    }


                }

            }

            if (Program.ShowBoundingBoxes)
            {
                debugLines.Begin();

                // You need to loop over your colliders here.
                // Example assumes: Owner has ColliderComponent with WORLD AABB min/max.

                bool collision = false;

                foreach (var gameObject in Program.SceneObjects) //Issue is that the parent objects are not renderingObjects
                {
                    
                    BoxColliderComponent? col = gameObject.GetComponent<BoxColliderComponent>();

                    if (col != null)
                    {                        
                        debugLines.AddAabb(col.WorldMin, col.WorldMax);
                        if (col.IsColliding) collision = true;
                    }
                    
                }
                
                Vector3D<float> colour = collision ? new Vector3D<float>(1f, 0f, 0f) : new Vector3D<float>(0f, 1f, 0f);

                debugLines.Flush(view, projection, colour, alwaysOnTop: true);

                // Restore main program because DebugLines switched programs
                gl.UseProgram(shaderProgram);
            }


        }


        

            

    }
}
