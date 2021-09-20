using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Tiles
{
    public class TileFactory
    {
        public Tile Get(string type)
        {
            switch (type)
            {
                case "Wall" or "#":
                    return new Tile(Color.White, Color.Black, '#', 1)
                    {
                        IsWalkable = false,
                        IsTransparent = false,
                        IsExplored = false,
                    };
                case "Floor" or ".":
                    return new Tile(Color.White, Color.Black, '.', 1)
                    {
                        IsWalkable = true,
                        IsTransparent = true,
                        IsExplored = false,
                    };
                case "Door" or "+":
                    Tile door = new Tile(Color.White, Color.Black, '+', 1)
                    {
                        IsWalkable = false,
                        IsTransparent = false,
                        IsExplored = false
                    };
                    door.Flags.Add("Door");
                    return door;
                case "Bars" or "=":
                    return new Tile(Color.White, Color.Black, '=', 1)
                    {
                        IsWalkable = false,
                        IsTransparent = true,
                        IsExplored = false
                    };
                default:
                    return null;
            }
        }
    }
}
