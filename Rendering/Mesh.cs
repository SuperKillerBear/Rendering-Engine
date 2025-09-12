using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
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

        public Mesh(GL gl, Vertex[] vertices, uint[] indices = null)
        {
            this.gl = gl;



            VertexCount = vertices.Length; // 3 pos + 3 colour + 2 uv = 8

            VAO = gl.GenVertexArray();
            gl.BindVertexArray(VAO);

            VBO = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ArrayBuffer, VBO);

            unsafe
            {
                fixed (Vertex* v = &vertices[0])
                {
                    gl.BufferData(GLEnum.ArrayBuffer,
                        (nuint)(vertices.Length * Marshal.SizeOf<Vertex>()),
                        v,
                        GLEnum.StaticDraw);
                }
                
                //Total Size of all Vertex Data
                uint stride = (uint) Marshal.SizeOf<Vertex>();

                //TODO: Update Object Parser + Mesh Data of all classes

                // Position
                gl.VertexAttribPointer(0, 3, GLEnum.Float, false, stride, (void*) 0);
                gl.EnableVertexAttribArray(0);

                // Color
                gl.VertexAttribPointer(1, 3, GLEnum.Float, false, stride, (void*)(3 * sizeof(float)));
                gl.EnableVertexAttribArray(1);

                //UV Coords
                gl.VertexAttribPointer(2, 2, GLEnum.Float, false, stride, (void*)(6 * sizeof(float)));
                gl.EnableVertexAttribArray(2);
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

            gl.BindVertexArray(0);
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
