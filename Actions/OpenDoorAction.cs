using RogueLike.Tiles;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class OpenDoorAction : Action
    {
        private Tile _door;

        public override bool Perform()
        {
            if (_door.IsWalkable)
                return false;

            RogueLike.Map.SetCellTransparancy(_door.Position, true);
            RogueLike.Map.SetCellWalkability(_door.Position, true);
            _door.Appearance.GlyphCharacter = '-';
            return true;
        }

        public OpenDoorAction(Tile door)
        {
            _door = door;
        }
    }
}
