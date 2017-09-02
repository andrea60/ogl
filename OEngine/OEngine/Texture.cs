using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace OpenGL
{
    public class TextureImage
    {
        public uint TextureID;
        public TextureUnit GLTextureUnit { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<TextureFrame> Frames { get; set; } = new List<TextureFrame>();

        public void Subdivide(int nx, int ny)
        {
            var frameWidth = Width / nx;
            var frameHeight = Height / ny;

            for (var i = 0; i < nx; i++)
                for (var j = 0; j < ny; j++)
                    Frames.Add(new TextureFrame(this,frameWidth, frameHeight, frameWidth * i, frameHeight * i));
        }
    }

    public class TextureFrame
    {
        public TextureImage TextureImage { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public Vector2 UpLeft { get; set; }
        public Vector2 UpRight { get; set; }
        public Vector2 DownLeft { get; set; }
        public Vector2 DownRight { get; set; }

        public TextureFrame(TextureImage textureImage, int w, int h, int ox, int oy)
        {
            Width = w;
            Height = h;
            OffsetX = ox;
            OffsetY = oy;
            TextureImage = textureImage;

            var nW = w / (float)textureImage.Width;
            var nH = h / (float)textureImage.Height;
            var nOx = ox / (float)textureImage.Width;
            var nOy = oy / (float)textureImage.Height;

            UpLeft = new Vector2(nOx,nOy+nH);
            UpRight = new Vector2(nOx + nW,nOy + nH);
            DownRight = new Vector2(nOx + nW, nOy);
            DownLeft = new Vector2(nOx, nOy);
        }

    }
}
