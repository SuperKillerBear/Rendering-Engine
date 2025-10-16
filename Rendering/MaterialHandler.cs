using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ARB;
using StbImageSharp;
using System.IO.Enumeration;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace RenderingEngine.Rendering
{
    public class Material
    {
        public string Filename;
        public ulong BindlessHandle;
        public uint TextureID;
        public Vector3D<float> Colour;

        public Material(string filename, Vector3D<float> colour, ulong Handle, uint textureID)
        {
            this.Filename = filename;
            this.Colour = colour;
            this.BindlessHandle = Handle;
            this.TextureID = textureID;
        }
    }


    public static class MaterialHandler
    {
        //Filename => TextID, Handle
        private static Dictionary<string, (uint textureID, ulong handle)> textureHandles = new();

        public static ArbBindlessTexture bindless;

        public static ulong defaultHandle;
        public static Material defaultMaterial;
        private static uint defaultTextureID;

        public static void Init()
        {
            // 1x1 white pixel
            byte[] whitePixel = { 255, 255, 255, 255 };

            defaultTextureID = Program.gl.GenTexture();
            Program.gl.BindTexture(GLEnum.Texture2D, defaultTextureID);
            unsafe
            {
                fixed (byte* p = &whitePixel[0])
                {
                    Program.gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba8,
                                          1, 1, 0, GLEnum.Rgba, GLEnum.UnsignedByte, p);
                }
            }

            // Make it bindless
            if (Program.gl.TryGetExtension<ArbBindlessTexture>(out bindless))
            {
                defaultHandle = bindless.GetTextureHandle(defaultTextureID);
                bindless.MakeTextureHandleResident(defaultHandle);
            }
            else
            {
                //Wont happen as checked in Program
                Console.WriteLine("ERR: Bindless Textures Not Supported");
                Program.Cleanup();
            }

            defaultMaterial = new Material("DEFAULT", new Vector3D<float>(1f), defaultHandle, defaultTextureID);

        }


        public static Material CreateMaterial(string filename, Vector3D<float> baseColour)
        {
            var data = GetTexture(filename);
            string name = data.handle == defaultHandle ? "DEFAULT" : filename;
            Material mat = new Material(name, baseColour, data.handle, data.TextureID);
            return mat;
        }

 

        public static (ulong handle, uint TextureID) GetTexture(string filename)
        {
            if (filename == "EMPTY") { Console.WriteLine("Loading DefaultMaterial for EMPTY"); return (defaultHandle, defaultTextureID); }

            if (textureHandles.ContainsKey(filename)) 
            { 
                var data = textureHandles[filename];
                return (data.handle, data.textureID);
            }
            try
            {
                string localPath = @$"C:\Users\ItsDaGrizz\Desktop\Rendering-Engine\TextureData\{filename}.png";
                ImageResult image;
                using (FileStream fs = File.OpenRead(localPath))
                {
                    image = ImageResult.FromStream(fs, ColorComponents.RedGreenBlueAlpha);
                }

                FlipImageVertically(image.Data, image.Width, image.Height, 4);


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

                ulong handle = bindless.GetTextureHandle(texture);

                bindless.MakeTextureHandleResident(handle);

                bool resident = bindless.IsTextureHandleResident(handle);
                Console.WriteLine($"Handle {handle} resident? {resident}");


                textureHandles.Add(filename, (texture, handle));

                return (handle, texture);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: Cannot Load Texture: {ex}");
                return (defaultHandle, defaultTextureID);
            }
        }

        
        private static void FlipImageVertically(byte[] data, int width, int height, int channels)
        {
            for (int y = 0; y < height / 2; y++)
            {
                int top = y * width * channels;
                int bottom = (height - y - 1) * width * channels;

                for (int x = 0; x < width * channels; x++)
                {
                    byte temp = data[top + x];
                    data[top + x] = data[bottom + x];
                    data[bottom + x] = temp;
                }
            }
        }



        public static void UnloadTextures()
        {
            foreach (var tex in textureHandles.Values)
                Program.gl.DeleteTexture(tex.textureID);
            textureHandles.Clear();
        }





    }
}