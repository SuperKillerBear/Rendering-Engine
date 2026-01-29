using System.Drawing;
using System.Runtime.CompilerServices;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace RenderingEngine.Rendering
{
    public sealed class DebugLines : IDisposable
    {
        private readonly GL gl;
        private Shader shader;

		private uint vao;
		private uint vbo;
		private uint program;

		private int uViewLoc;
		private int uProjLoc;
		private int uColorLoc;

		private float[] cpu;
		private int floatCount;
		private int capacityFloats;

		public DebugLines(GL gl, string vertPath, string fragPath, int initialMaxLines = 4096)
        {
            this.gl = gl;

            capacityFloats = Math.Max(1, initialMaxLines) * 2 * 3;
            cpu = new float[capacityFloats];
            floatCount = 0;

            InitGpu();

            shader = new Shader(gl, vertPath, fragPath);

            uViewLoc = gl.GetUniformLocation(shader.ProgramID, "uView");
            uProjLoc = gl.GetUniformLocation(shader.ProgramID, "uProjection");
            uColorLoc = gl.GetUniformLocation(shader.ProgramID, "uColor");

        }


		private unsafe void InitGpu()
		{
			vao = gl.GenVertexArray();
			vbo = gl.GenBuffer();

			gl.BindVertexArray(vao);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

			gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(capacityFloats * sizeof(float)), null, BufferUsageARB.DynamicDraw);

			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)(3 * sizeof(float)), (void*)0);

			gl.BindVertexArray(0);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		}



		public void Begin()
		{
			floatCount = 0;
		}

		private unsafe void EnsureCapacityFloats(int needed)
		{
			if (needed <= capacityFloats)
				return;

			int newCap = capacityFloats;
			while (newCap < needed)
				newCap *= 2;

			Array.Resize(ref cpu, newCap);
			capacityFloats = newCap;

			gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
			gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(capacityFloats * sizeof(float)), null, BufferUsageARB.DynamicDraw);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddLine(in Vector3D<float> a, in Vector3D<float> b)
		{
			EnsureCapacityFloats(floatCount + 6);

			cpu[floatCount + 0] = a.X;
			cpu[floatCount + 1] = a.Y;
			cpu[floatCount + 2] = a.Z;

			cpu[floatCount + 3] = b.X;
			cpu[floatCount + 4] = b.Y;
			cpu[floatCount + 5] = b.Z;

			floatCount += 6;
		}

		public void AddAabb(in Vector3D<float> min, in Vector3D<float> max)
		{
			Vector3D<float> c000 = new(min.X, min.Y, min.Z);
			Vector3D<float> c100 = new(max.X, min.Y, min.Z);
			Vector3D<float> c010 = new(min.X, max.Y, min.Z);
			Vector3D<float> c110 = new(max.X, max.Y, min.Z);

			Vector3D<float> c001 = new(min.X, min.Y, max.Z);
			Vector3D<float> c101 = new(max.X, min.Y, max.Z);
			Vector3D<float> c011 = new(min.X, max.Y, max.Z);
			Vector3D<float> c111 = new(max.X, max.Y, max.Z);

			// bottom
			AddLine(c000, c100);
			AddLine(c100, c110);
			AddLine(c110, c010);
			AddLine(c010, c000);

			// top
			AddLine(c001, c101);
			AddLine(c101, c111);
			AddLine(c111, c011);
			AddLine(c011, c001);

			// verticals
			AddLine(c000, c001);
			AddLine(c100, c101);
			AddLine(c110, c111);
			AddLine(c010, c011);
		}

		public unsafe void Flush(in Matrix4X4<float> view, in Matrix4X4<float> projection, Vector3D<float> color, bool alwaysOnTop)
		{
			if (floatCount == 0)
				return;

			gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
			fixed (float* p = cpu)
			{
				gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (nuint)(floatCount * sizeof(float)), p);
			}

			bool depthWasEnabled = gl.IsEnabled(GLEnum.DepthTest);

			if (alwaysOnTop)
				gl.Disable(GLEnum.DepthTest);
			else
				gl.Enable(GLEnum.DepthTest);

			gl.UseProgram(shader.ProgramID);


            fixed (Matrix4X4<float>* v = &view)
            fixed (Matrix4X4<float>* p = &projection)
            {
                gl.UniformMatrix4(uViewLoc, 1, false, (float*)v);
                gl.UniformMatrix4(uProjLoc, 1, false, (float*)p);
            }
            

			gl.Uniform3(uColorLoc, color.X, color.Y, color.Z);

			gl.BindVertexArray(vao);

			uint vertexCount = (uint)(floatCount / 3);
			gl.DrawArrays(PrimitiveType.Lines, 0, vertexCount);

			gl.BindVertexArray(0);
			gl.UseProgram(0);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

			if (depthWasEnabled)
				gl.Enable(GLEnum.DepthTest);
			else
				gl.Disable(GLEnum.DepthTest);
		}

		public void Dispose()
		{
			if (shader != null && shader.ProgramID != 0) gl.DeleteProgram(shader.ProgramID);
			if (vbo != 0) gl.DeleteBuffer(vbo);
			if (vao != 0) gl.DeleteVertexArray(vao);
		}
    }
}