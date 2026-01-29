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
            CheckShaderCompileStatus(gl, vertexShader, vertexPath);

            uint fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentSrc);
            gl.CompileShader(fragmentShader);
            CheckShaderCompileStatus(gl, fragmentShader, fragmentPath);

            ProgramID = gl.CreateProgram();
            gl.AttachShader(ProgramID, vertexShader);
            gl.AttachShader(ProgramID, fragmentShader);
            gl.LinkProgram(ProgramID);
            CheckProgramLinkStatus(gl, ProgramID);

            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }

        private void CheckShaderCompileStatus(GL gl, uint shader, string path)
        {
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            string infoLog = gl.GetShaderInfoLog(shader);

            if (!string.IsNullOrWhiteSpace(infoLog))
                System.Console.WriteLine($"Shader compile log ({path}):\n{infoLog}");

            if (status == 0)
                throw new Exception($"Shader compilation failed ({path}).\n{infoLog}");
            else Console.WriteLine($"Shader Successfully Compiled ({path})");
        }

        private void CheckProgramLinkStatus(GL gl, uint program)
        {
            gl.GetProgram(program, GLEnum.LinkStatus, out int status);
            string infoLog = gl.GetProgramInfoLog(program);

            if (!string.IsNullOrWhiteSpace(infoLog))
                System.Console.WriteLine($"Program link log: {infoLog}");
            if (status == 0)
		        throw new Exception($"Program link failed.\n{infoLog}");
        }
    }
}
