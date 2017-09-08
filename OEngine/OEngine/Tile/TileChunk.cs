using OEngine.ComponentSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.Threading.Tasks;

namespace OEngine.Tile
{
    public class TileChunk
    {
        public static int ChunkSize = 32;
        public RenderTileComponent[][] Tiles { get; internal set; }

        private float[] UVArray = new float[ChunkSize * ChunkSize * 12];
        private float[] VertexArray = new float[ChunkSize * ChunkSize * 18];
        private uint UVVBO;
        private uint VAVBO;
        private uint VAO;

        public TileChunk()
        {
            Tiles = new RenderTileComponent[ChunkSize][];
            for (var i = 0; i < ChunkSize; i++)
            {
                Tiles[i] = new RenderTileComponent[ChunkSize];
            }
            Initialize();
        }

        public void Initialize()
        {
            int i = 0;
            int j = 0;
            //for (var x = 0; x < ChunkSize; x++)
            //    for (var y = 0; y < ChunkSize; y++)
            //    {
            //        var tile = Tiles[x][y];
            //        VertexArray[i++] = tile?.GameObject.Position.X ?? 0;
            //        VertexArray[i++] = tile?.GameObject.Position.Y + tile.GameObject.Scale.Y ?? 0;
            //        VertexArray[i++] = 0f;

            //        VertexArray[i++] = tile?.GameObject.Position.X + tile.GameObject.Scale.X ?? 0;
            //        VertexArray[i++] = tile?.GameObject.Position.Y ?? 0;
            //        VertexArray[i++] = 0f;

            //        VertexArray[i++] = tile?.GameObject.Position.X ?? 0;
            //        VertexArray[i++] = tile?.GameObject.Position.Y ?? 0;
            //        VertexArray[i++] = 0f;

            //        VertexArray[i++] = tile?.GameObject.Position.X ?? 0;
            //        VertexArray[i++] = tile?.GameObject.Position.Y + tile.GameObject.Scale.Y ?? 0;
            //        VertexArray[i++] = 0f;

            //        VertexArray[i++] = tile?.GameObject.Position.X + tile.GameObject.Scale.X ?? 0;
            //        VertexArray[i++] = tile?.GameObject.Position.Y + tile.GameObject.Scale.Y ?? 0;
            //        VertexArray[i++] = 0f;

            //        VertexArray[i++] = tile?.GameObject.Position.X + tile.GameObject.Scale.X ?? 0;
            //        VertexArray[i++] = tile?.GameObject.Position.Y ?? 0;
            //        VertexArray[i++] = 0f;

            //        UVArray[j++] = tile?.TextureUV[0] ?? 0;
            //        UVArray[j++] = tile?.TextureUV[1] ?? 0;


            //        UVArray[j++] = tile?.TextureUV[2] ?? 0;
            //        UVArray[j++] = tile?.TextureUV[3] ?? 0;


            //        UVArray[j++] = tile?.TextureUV[4] ?? 0;
            //        UVArray[j++] = tile?.TextureUV[5] ?? 0;


            //        UVArray[j++] = tile?.TextureUV[6] ?? 0;
            //        UVArray[j++] = tile?.TextureUV[7] ?? 0;



            //        UVArray[j++] = tile?.TextureUV[8] ?? 0;
            //        UVArray[j++] = tile?.TextureUV[9] ?? 0;


            //        UVArray[j++] = tile?.TextureUV[10] ?? 0;
            //        UVArray[j++] = tile?.TextureUV[11] ?? 0;
            //    }

            GL.GenBuffers(1, out VAVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VAVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * VertexArray.Length, VertexArray, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out UVVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, UVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * UVArray.Length, UVArray, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            GL.EnableVertexArrayAttrib(VAO, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VAVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexArrayAttrib(VAO, 1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, UVVBO);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);


        }

        public void Draw()
        {
            int i = 0;
            int j = 0;
            for(var x=0;x<ChunkSize;x++)
                for(var y=0;y<ChunkSize;y++)
                {
                    var tile = Tiles[x][y];

                    UVArray[j++] = tile?.TextureUV[0] ?? 0;
                    UVArray[j++] = tile?.TextureUV[1] ?? 0;


                    UVArray[j++] = tile?.TextureUV[2] ?? 0;
                    UVArray[j++] = tile?.TextureUV[3] ?? 0;


                    UVArray[j++] = tile?.TextureUV[4] ?? 0;
                    UVArray[j++] = tile?.TextureUV[5] ?? 0;


                    UVArray[j++] = tile?.TextureUV[6] ?? 0;
                    UVArray[j++] = tile?.TextureUV[7] ?? 0;



                    UVArray[j++] = tile?.TextureUV[8] ?? 0;
                    UVArray[j++] = tile?.TextureUV[9] ?? 0;


                    UVArray[j++] = tile?.TextureUV[10] ?? 0;
                    UVArray[j++] = tile?.TextureUV[11] ?? 0;


                    VertexArray[i++] = tile?.GameObject.Position.X ?? 0;
                    VertexArray[i++] = tile?.GameObject.Position.Y + tile.GameObject.Scale.Y ?? 0;
                    VertexArray[i++] = 0f;

                    VertexArray[i++] = tile?.GameObject.Position.X + tile.GameObject.Scale.X ?? 0;
                    VertexArray[i++] = tile?.GameObject.Position.Y ?? 0;
                    VertexArray[i++] = 0f;

                    VertexArray[i++] = tile?.GameObject.Position.X ?? 0;
                    VertexArray[i++] = tile?.GameObject.Position.Y ?? 0;
                    VertexArray[i++] = 0f;

                    VertexArray[i++] = tile?.GameObject.Position.X ?? 0;
                    VertexArray[i++] = tile?.GameObject.Position.Y + tile.GameObject.Scale.Y ?? 0;
                    VertexArray[i++] = 0f;

                    VertexArray[i++] = tile?.GameObject.Position.X + tile.GameObject.Scale.X ?? 0;
                    VertexArray[i++] = tile?.GameObject.Position.Y + tile.GameObject.Scale.Y ?? 0;
                    VertexArray[i++] = 0f;

                    VertexArray[i++] = tile?.GameObject.Position.X + tile.GameObject.Scale.X ?? 0;
                    VertexArray[i++] = tile?.GameObject.Position.Y ?? 0;
                    VertexArray[i++] = 0f;


                }

            GL.BindBuffer(BufferTarget.ArrayBuffer,UVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * UVArray.Length, UVArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VAVBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * VertexArray.Length, VertexArray);

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, ChunkSize * ChunkSize);
        }

        public void AddTile(RenderTileComponent tile, int x, int y)
        {
            if (x < Tiles.Length && y < Tiles[x].Length)
                Tiles[x][y] = tile;
        }

        public void RemoveTile(int x, int y)
        {
            Tiles[x][y] = null;
        }
            

        
    }
}
