using RogueLike.Actors;
using RogueLike.Tiles;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class MoveAction : Action
    {
        private Actor _actor;
        private Direction _direction;

        public MoveAction(Actor actor, Direction direction)
        {
            _actor = actor;
            _direction = direction;
        }

        public override bool Perform()
        {
            Point target = _actor.Position + _direction;
            if (target.X < 0 || target.Y < 0 || target.X >= RogueLike.Map.Width || target.Y >= RogueLike.Map.Height)
                return false;

            Tile targetTile = RogueLike.Map.Tiles[target.X, target.Y];

            Actor targetActor = RogueLike.Map.Actors.Find(m => m.Position == targetTile.Position);

            if (targetActor != null)
            {
                if (_actor is Monster)
                    return false;

                new MeleeAttackAction(_actor, targetActor).Perform();
                return true;
            }

            if (targetTile.Flags.Any(s => s == "Door"))
            {
                new OpenDoorAction(targetTile).Perform();
            }

            if (targetTile.IsWalkable)
            {
                _actor.Position += _direction;

                if (_actor is Monster)
                {
                    (_actor as Monster).IsPlayerInFOV = new UpdateFovAction(_actor).Perform();
                }
                else
                {
                    new UpdateFovAction(_actor).Perform();
                }

                return true;
            }

            return false;
        }
    }
}
