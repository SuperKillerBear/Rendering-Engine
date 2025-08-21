using RenderingEngine.Objects;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;
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
        private DynamicObject[] dynObjs; 

        public Renderer(GL gl, uint shaderProgram)
        {
            this.gl = gl;
            this.shaderProgram = shaderProgram;
            uModelLocation = gl.GetUniformLocation(shaderProgram, "uModel");


            //ASSIGN OBJECTS
            dynObjs = new DynamicObject[] { new Triangle(gl) };
        }

        public void Clear()
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        }

        //shader.SetMatrix("model", obj.ModelMatrix);

        public void Draw()
        {
            var meshGroups = dynObjs.GroupBy(obj => obj.Mesh);

            // Orthographic projection example
            Matrix4X4<float> projection = Matrix4X4.CreateOrthographic(2f, 2f, 0.1f, 10f);
            Matrix4X4<float> view = Matrix4X4.CreateTranslation(0, 0, -2f); // move camera back

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
