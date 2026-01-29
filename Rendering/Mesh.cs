using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;

namespace RenderingEngine.Rendering
{
    public class Mesh
    {
        public uint VAO;
        public uint VBO;
        public uint EBO; //for indices
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public Mesh(float[] vertices, uint[] indices = null)
        {
            VertexCount = vertices.Length / 8; // 3 pos + 3 normals + 2 uv = 8

            VAO = Program.gl.GenVertexArray();
            Program.gl.BindVertexArray(VAO);

            VBO = Program.gl.GenBuffer();
            Program.gl.BindBuffer(GLEnum.ArrayBuffer, VBO);

            unsafe
            {
                fixed (float* v = &vertices[0])
                {
                    Program.gl.BufferData(GLEnum.ArrayBuffer,
                        (nuint)(vertices.Length * sizeof(float)),
                        v,
                        GLEnum.StaticDraw);
                }
                
                //Total Size of all Vertex Data
                uint stride = 8 * sizeof(float);

                //TODO: Update Object Parser + Mesh Data of all classes

                // Position
                Program.gl.VertexAttribPointer(0, 3, GLEnum.Float, false, stride, (void*) 0);
                Program.gl.EnableVertexAttribArray(0);

                // Normals
                Program.gl.VertexAttribPointer(1, 3, GLEnum.Float, false, stride, (void*)(3 * sizeof(float)));
                Program.gl.EnableVertexAttribArray(1);

                //UV Coords
                Program.gl.VertexAttribPointer(2, 2, GLEnum.Float, false, stride, (void*)(6 * sizeof(float)));
                Program.gl.EnableVertexAttribArray(2);
            }

            if (indices != null)
            {
                IndexCount = indices.Length;
                EBO = Program.gl.GenBuffer();
                Program.gl.BindBuffer(GLEnum.ElementArrayBuffer, EBO);

                unsafe
                {
                    fixed (uint* i = &indices[0])
                    {
                        Program.gl.BufferData(GLEnum.ElementArrayBuffer,
                            (nuint)(indices.Length * sizeof(uint)),
                            i,
                            GLEnum.StaticDraw);
                    }
                }
            }

            
        }



    }
}