using RenderingEngine.Meshes;
using RenderingEngine.Objects;
using RenderingEngine.RawObjData;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Silk.NET.Core.Native.WinString;
using Silk.NET.OpenGL.Extensions.ImGui;
using ImGuiNET;


namespace RenderingEngine.Rendering
{
    public class Renderer
    {
        private GL gl;        

        private uint shaderProgram;
        private int uModelLocation;

        //TEMPORARY
        public static DynamicObject[] dynObjs; 

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


            //Try Obj Parser

            //string pasted = @"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\RawObjData\RobinHoodBay";
            //string fullpath = pasted.Trim();


            //var (vertsList, indsList) = ImportHandler.LoadObjFile(fullpath);
            //float[] verts = vertsList.ToArray();
            //uint[] inds = indsList.ToArray();
            //var mesh = new Mesh(gl, verts, inds);
            //var loaded = new LoadedDynamicObject(gl, mesh, new Vector3D<float>(1.5f));

            //Create Floor Quad
            var floor = new Quad(gl);
            floor.Rotation.X = (float) Math.PI / 2;
            floor.Scale = new Vector3D<float>(5);
            floor.Position.Y = -0.5f;

            //Physics Object
            var cube = new PhysicsObject(new CubeMesh(gl));
            cube.Position.Y = 4.5f;
            
            //ASSIGN OBJECTS
            //dynObjs = new DynamicObject[] { loaded };
            dynObjs = [floor, cube];
            
        }

        public void Clear()
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
            
        }

        

        public void Draw()
        {
            

            var meshGroups = dynObjs.GroupBy(obj => obj.Mesh);

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

            


            foreach (var group in meshGroups)
            {
                var mesh = group.Key;
                gl.BindVertexArray(mesh.VAO);
                
                foreach (var obj in group)
                {
                    obj.UpdateModelMatrix();

                    Matrix4X4<float> model = obj.ModelMatrix;

                    

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
