using GoRogue.DiceNotation;
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
            int roll = 0;
            Color fg;
            char c = ' ';
            switch (type)
            {
                case "Wall" or "#":
                    roll = Dice.Roll("1d4");

                    if (roll == 1)
                        fg = Color.DarkSlateGray;
                    else if (roll == 2)
                        fg = Color.DarkGray;
                    else if (roll == 3)
                        fg = Color.Gray;
                    else
                        fg = Color.Beige;


                    return new Tile(fg, Color.Black, '#', 1)
                    {
                        IsWalkable = false,
                        IsTransparent = false,
                        IsExplored = false,
                    };
                case "Floor" or ".":
                    roll = Dice.Roll("1d4");

                    if (roll == 1)
                        fg = Color.Green;
                    if (roll == 2)
                        fg = Color.DarkGreen;
                    if (roll == 3)
                        fg = Color.ForestGreen;
                    else
                        fg = Color.LawnGreen;

                    roll = Dice.Roll("1d5");

                    if (roll == 1)
                        c = '\'';
                    else if (roll == 2)
                        c = '\"';
                    else if (roll == 3)
                        c = '`';
                    else if (roll == 4)
                        c = ',';
                    else
                        c = '.';

                    return new Tile(fg, Color.Black, c, 1)
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
