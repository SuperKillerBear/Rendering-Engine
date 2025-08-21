using RenderingEngine.Objects;
using RenderingEngine.RawObjData;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Silk.NET.Core.Native.WinString;

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

            //gl.Enable(GLEnum.DepthTest); //Causes issues currently
            //gl.DepthFunc(GLEnum.Less);

            //Cull Backfaces to boost preformance
            gl.Enable(GLEnum.CullFace);
            gl.CullFace(GLEnum.Back);

            gl.UseProgram(shaderProgram);

            //Try Obj Parser

            string pasted = "\"C:\\Users\\ItsDaGrizz\\Desktop\\Rendering-Engine\\RawObjData\\Cube.obj\"";
            string fullpath = pasted.Trim();

            var (vertsList, indsList) = ImportHandler.LoadObjFile(fullpath);
            float[] verts = vertsList.ToArray();
            uint[] inds = indsList.ToArray();
            var mesh = new Mesh(gl, verts, inds);
            var loaded = new LoadedDynamicObject(gl, mesh);


            //ASSIGN OBJECTS
            dynObjs = new DynamicObject[] { loaded };

            //TODO: Write obj converter or if too hard, find easier blender export file type converter
        }

        public void Clear()
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
            
        }

        

        public void Draw()
        {
            

            var meshGroups = dynObjs.GroupBy(obj => obj.Mesh);

            // Orthographic projection example
            Matrix4X4<float> projection = Matrix4X4.CreateOrthographic(2f, 2f, 0.1f, 10f);

            //TODO: Make Only Calc on Update
            Matrix4X4<float> view = 
                Matrix4X4.CreateTranslation(InputHandler.Position) *
                Matrix4X4.CreateRotationX(InputHandler.Rotation.X) *
                Matrix4X4.CreateRotationY(InputHandler.Rotation.Y) *
                Matrix4X4.CreateRotationZ(InputHandler.Rotation.Z);

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
