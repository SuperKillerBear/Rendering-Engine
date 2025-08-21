using Silk.NET.OpenGL;
using System.IO;

namespace RenderingEngine.Rendering
{
    public class Shader
    {
        public uint ProgramID { get; private set; }

        public Shader(GL gl, string vertexPath, string fragmentPath)
        {
            string vertexSrc = File.ReadAllText(vertexPath);
            string fragmentSrc = File.ReadAllText(fragmentPath);

            uint vertexShader = gl.CreateShader(GLEnum.VertexShader);
            gl.ShaderSource(vertexShader, vertexSrc);
            gl.CompileShader(vertexShader);
            CheckShaderCompileStatus(gl, vertexShader);

            uint fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentSrc);
            gl.CompileShader(fragmentShader);
            CheckShaderCompileStatus(gl, fragmentShader);

            ProgramID = gl.CreateProgram();
            gl.AttachShader(ProgramID, vertexShader);
            gl.AttachShader(ProgramID, fragmentShader);
            gl.LinkProgram(ProgramID);
            CheckProgramLinkStatus(gl, ProgramID);

            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }

        private void CheckShaderCompileStatus(GL gl, uint shader)
        {
            string infoLog = gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
                System.Console.WriteLine($"Shader compile log: {infoLog}");
        }

        private void CheckProgramLinkStatus(GL gl, uint program)
        {
            string infoLog = gl.GetProgramInfoLog(program);
            if (!string.IsNullOrWhiteSpace(infoLog))
                System.Console.WriteLine($"Program link log: {infoLog}");
        }
    }
}
