using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEngine.Managers;

namespace OEngine.ComponentSystem.Components
{
    public class RenderTileComponent : BaseComponent
    {
        public static float WorldTileDimension = 0.1f;
        public float[] TextureUV { get; internal set; }
        public TextureFrame Tile { get; internal set; }

        public int TileMapX {
            get
            {
                return Convert.ToInt32(GameObject.Position.X / WorldTileDimension);
            }
        }
        public int TileMapY
        {
            get
            {
                return Convert.ToInt32(GameObject.Position.Y / WorldTileDimension);
            }
        }

        public RenderTileComponent(TextureFrame tile) : base(Managers.Managers.RENDERER)
        {
            SetTile(tile);

        }

        public void SetTile(TextureFrame tile)
        {
            Tile = tile;
            TextureUV = new float[]
            {
                tile.UpLeft.X,tile.UpLeft.Y,
                tile.DownRight.X,tile.DownRight.Y,
                tile.DownLeft.X,tile.DownLeft.Y,
                tile.UpLeft.X,tile.UpLeft.Y,
                tile.UpRight.X,tile.UpRight.Y,
                tile.DownRight.X,tile.DownRight.Y
            };
        }

        public override BaseComponent Clone()
        {
            return new RenderTileComponent(Tile);
        }

        public override void Initialize()
        {
            
        }

        public override bool MustUpdateEachFrame()
        {
            return false;
        }

        public override void OnDestroy()
        {
           
        }

        public override void Subscribe()
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
