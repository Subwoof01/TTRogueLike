using GoRogue.Pathing;
using RogueLike.Actions;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actors
{
    public class Monster : Actor
    {
        public Breed Breed { get; set; }
        public bool IsPlayerInFOV { get; set; }
        public Path CurrentPath { get; set; }

        public Monster(ref ColoredGlyph appearance, int zIndex) : base(ref appearance, zIndex)
        {
        }

        public Monster(ColoredGlyph appearance, int zIndex) : base(appearance, zIndex)
        {
        }

        public Monster(Color foreground, Color background, int glyph, int zIndex) : base(foreground, background, glyph, zIndex)
        {
        }

        public override Action TakeTurn()
        {
            return new ChasePlayerAction(this, RogueLike.Player);
        }
    }
}
