using RogueLike.Tiles;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.MapGeneration
{
    public class TransparencyMap : GridViewBase<bool>
    {
        private bool[,] _transparency;

        public override bool this[Point pos]
        {
            get
            {
                return _transparency[pos.X, pos.Y];
            }
        }

        public override int Height
        {
            get
            {
                return _transparency.GetLength(1);
            }
        }
        public override int Width
        {
            get
            {
                return _transparency.GetLength(0);
            }
        }

        public void UpdateTileTransparency(Tile tile)
        {
            _transparency[tile.Position.X, tile.Position.Y] = tile.IsTransparent;
        }

        public TransparencyMap(Tile[,] tiles)
        {
            _transparency = new bool[tiles.GetLength(0), tiles.GetLength(1)];

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (tiles[x, y] == null)
                        continue;
                    _transparency[x, y] = tiles[x, y].IsTransparent;
                }
            }
        }
    }
}
