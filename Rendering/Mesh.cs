using Silk.NET.OpenGL;
using System.Threading.Tasks.Dataflow;

namespace RenderingEngine.Rendering
{
    public class Mesh
    {
        private GL gl;
        public uint VAO;
        public uint VBO;
        public uint EBO; //for indices
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public Mesh(GL gl, float[] vertices, uint[] indices = null)
        {
            this.gl = gl;

            //
            //TODO, Make Attribute Length Constant Between files to have update ie not "6"
            //

            VertexCount = vertices.Length / 6; // 3 pos + 3 colour

            VAO = gl.GenVertexArray();
            gl.BindVertexArray(VAO);

            VBO = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ArrayBuffer, VBO);

            unsafe
            {
                fixed (float* v = &vertices[0])
                {
                    gl.BufferData(GLEnum.ArrayBuffer,
                        (nuint)(vertices.Length * sizeof(float)),
                        v,
                        GLEnum.StaticDraw);
                }
            

            // Position
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 6 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);

            // Color
            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 6 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);
            }

            if (indices != null)
            {
                IndexCount = indices.Length;
                EBO = gl.GenBuffer();
                gl.BindBuffer(GLEnum.ElementArrayBuffer, EBO);

                unsafe
                {
                    fixed (uint* i = &indices[0])
                    {
                        gl.BufferData(GLEnum.ElementArrayBuffer,
                            (nuint)(indices.Length * sizeof(uint)),
                            i,
                            GLEnum.StaticDraw);
                    }
                }
            }            
        }


        public void Draw(GL gl)
        {
            gl.BindVertexArray(VAO);
            if (IndexCount > 0)
            {
                unsafe
                {
                    gl.DrawElements(PrimitiveType.Triangles, (uint)IndexCount, DrawElementsType.UnsignedInt, null);
                }                
            }
            else
            {
                gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)VertexCount);
            }
        }


    }
}
