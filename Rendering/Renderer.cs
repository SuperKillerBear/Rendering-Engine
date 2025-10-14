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


namespace RenderingEngine.Rendering
{
    public class Renderer
    {
        private GL gl;        

        private uint shaderProgram;
        private int uModelLocation;

        //TEMPORARY
        //public static DynamicObject[] dynObjs; 
        public static List<RendererComponent> RenderingObjects = new List<RendererComponent>();

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


        }

        public void Clear()
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
        }



        
                public void Draw()
                {
                    //Ensure shader program is being used
                    gl.UseProgram(shaderProgram);

                    var meshGroups = RenderingObjects.GroupBy(obj => obj.MeshID);

                    // Orthographic projection example
                    //Matrix4X4<float> projection = Matrix4X4.CreateOrthographic(2f, 2f, 0.1f, 10f);
                    Matrix4X4<float> projection =
                        Matrix4X4.CreatePerspectiveFieldOfView(
                            fieldOfView: Camera.FOV, // 60°
                            aspectRatio: Program.aspectRatio,
                            nearPlaneDistance: 0.1f,
                            farPlaneDistance: 100f
                        );

                    //TODO: Make Only Calc on Update
                    Matrix4X4<float> view =
                        Matrix4X4.CreateLookAt(Camera.Position,
                        Camera.Position + Camera.Forward,
                        Camera.Up);


                    int uViewLocation = gl.GetUniformLocation(shaderProgram, "uView");
                    int uProjectionLocation = gl.GetUniformLocation(shaderProgram, "uProjection");
                    int uTextureLocation = gl.GetUniformLocation(shaderProgram, "uTexture"); //not -1
                    int colorLocation = gl.GetUniformLocation(shaderProgram, "uBaseColor");

                    if (uTextureLocation == -1)
                    {
                        Console.WriteLine("WARN: uTexture uniform location is -1 (not found)");
                    }


                    foreach (var group in meshGroups)
                    {
                        Mesh mesh = MeshHandler.GetMesh(group.Key);
                        if (mesh == null) continue;

                        gl.BindVertexArray(mesh.VAO);

                        foreach (var rendrComp in group)
                        {
                            rendrComp.owner.Transform.UpdateModelMatrix();

                            Matrix4X4<float> model = rendrComp.owner.Transform.ModelMatrix;

                            //Set Material
                            Material mat = rendrComp.material ?? MaterialHandler.defaultMaterial;

                            // Set material properties
                            gl.Uniform3(colorLocation, mat.Colour.X, mat.Colour.Y, mat.Colour.Z);

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
                                gl.UniformMatrix4(uModelLocation, 1, false, (float*)&model);
                                gl.UniformMatrix4(uViewLocation, 1, false, (float*)&view);
                                gl.UniformMatrix4(uProjectionLocation, 1, false, (float*)&projection);


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

                }
            

    }
}
