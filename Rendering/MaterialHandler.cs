using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;
using Silk.NET.OpenGL.Extensions.ARB;
using System.IO.Enumeration;
using System.Reflection.Metadata.Ecma335;

namespace RenderingEngine.Rendering
{
    public class Material
    {
        public string Filename;
        public ulong BindlessHandle;
        public Vector3D<float> Colour;

        public Material(string filename, Vector3D<float> colour, ulong Handle)
        {
            this.Filename = filename;
            this.Colour = colour;
            this.BindlessHandle = Handle;
        }
    }


    public static class MaterialHandler
    {
        private static Dictionary<string, ulong> textureHandles = new();

        public static ulong defaultHandle;
        public static Material defaultMaterial;

        public static void Init()
        {
            // 1x1 white pixel
            byte[] whitePixel = { 255, 255, 255, 255 };

            uint defaultTexture = Program.gl.GenTexture();
            Program.gl.BindTexture(GLEnum.Texture2D, defaultTexture);
            unsafe
            {
                fixed (byte* p = &whitePixel[0])
                {
                    Program.gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba8,
                                          1, 1, 0, GLEnum.Rgba, GLEnum.UnsignedByte, p);
                }
            }

            // Make it bindless
            ArbBindlessTexture bindless = new ArbBindlessTexture((Silk.NET.Core.Contexts.INativeContext)Program.gl);
            defaultHandle = bindless.GetTextureHandle(defaultTexture);
            bindless.MakeImageHandleResident(defaultHandle, (ARB)GLEnum.ReadOnly);

            defaultMaterial = new Material("DEFAULT", new Vector3D<float>(255), defaultHandle);
        }

        
        public static Material CreateMaterial(string filename, Vector3D<float> defaultColour)
        {
            ulong handle = GetTextureHandle(filename);
            Material mat = new Material(filename, defaultColour, handle);
            return mat;
        }

        public static ulong GetTextureHandle(string filename)
        {
            if (textureHandles.ContainsKey(filename)) { return textureHandles[filename]; }
            try
            {
                string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\TextureData\{filename}.png";
                ImageResult image;
                using (FileStream fs = File.OpenRead(localPath))
                {
                    image = ImageResult.FromStream(fs, ColorComponents.RedGreenBlueAlpha);
                }

                int width = image.Width;
                int height = image.Height;
                byte[] pixels = image.Data;


                uint texture = Program.gl.GenTexture();
                Program.gl.BindTexture(GLEnum.Texture2D, texture);

                unsafe
                {
                    fixed (byte* p = &pixels[0])
                    {
                        Program.gl.TexImage2D(
                            GLEnum.Texture2D,
                            0,                      // mip level
                            (int)GLEnum.Rgba,       // internal format
                            (uint)width,
                            (uint)height,
                            0,                      // border
                            GLEnum.Rgba,            // format of source data
                            GLEnum.UnsignedByte,    // type
                            p                       // pointer to pixel data
                        );
                    }
                }

                // --- Set texture parameters ---
                Program.gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
                Program.gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
                Program.gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
                Program.gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Repeat);


                ArbBindlessTexture bindless = new ArbBindlessTexture((Silk.NET.Core.Contexts.INativeContext)Program.gl);
                ulong handle = bindless.GetTextureHandle(texture);
                bindless.MakeImageHandleResident(handle, (ARB)GLEnum.ReadOnly);

                textureHandles.Add(filename, handle);

                return handle;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: Cannot Load Texture: {ex}");
                return defaultHandle;
            }
        }
    }
}