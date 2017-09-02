using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL.Managers
{
    
   
    public static class ResourceManager
    {
        public static Dictionary<string, TextureImage> SceneTextures { get; set; } = new Dictionary<string, TextureImage>();
        

        public static TextureImage RegisterTexture(string textureName, string path)
        {
            var (data,w,h) = LoadImage(path);
            var texture = DisplayManager.LoadGLTexture(data, w, h);
            if (SceneTextures.Keys.Contains(textureName))
                return null;
            SceneTextures.Add(textureName, texture);
            return texture;
        }

        private static (byte[], int, int) LoadImage(string path)
        {
            var image = new Bitmap(path);
            if ((image.Width & (image.Width -1)) != 0 || (image.Height & (image.Height -1)) != 0)
            {
                Debugger.Log($"Warning! Attempting to load an image file with non-power-of-two dimensions! (Width: {image.Width}, Height:{image.Height})");
            }
            var r = GetFloats(image);
            return (r,image.Width,image.Height);
        }

        private static byte[] GetFloats(Bitmap PNG)
        {
            byte[] rgbaB = new byte[4 * (PNG.Width * PNG.Height)];

            int i = 0;

            for (var y = PNG.Height-1; y >= 0; y--)
            {
                for (var x = 0; x < PNG.Width; x++)
                {
                    Color pix = PNG.GetPixel(x, y);

                    rgbaB[i++] = pix.R;
                    rgbaB[i++] = pix.G;
                    rgbaB[i++] = pix.B;
                    rgbaB[i++] = pix.A;
                }
            }

            return rgbaB;
        }

    }
}
