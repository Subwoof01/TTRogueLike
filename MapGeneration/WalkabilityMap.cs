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
    public class WalkabilityMap : GridViewBase<bool>
    {
        private bool[,] _walkability;

        public override bool this[Point pos]
        {
            get
            {
                return _walkability[pos.X, pos.Y];
            }
        }

        public override int Height
        {
            get
            {
                return _walkability.GetLength(1);
            }
        }
        public override int Width
        {
            get
            {
                return _walkability.GetLength(0);
            }
        }

        public void UpdateTileWalkability(Tile tile)
        {
            _walkability[tile.Position.X, tile.Position.Y] = tile.IsWalkable;
        }

        public WalkabilityMap(Tile[,] tiles)
        {
            _walkability = new bool[tiles.GetLength(0), tiles.GetLength(1)];

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (tiles[x, y] == null)
                        continue;
                    _walkability[x, y] = tiles[x, y].IsWalkable;
                }
            }
        }
    }
}
