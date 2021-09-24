using GoRogue.Pathing;
using RogueLike.Actors;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;

namespace RogueLike.Actions
{
    public class ChasePlayerAction : Action
    {
        private Monster _start;
        private Actor _target;

        public override bool Perform()
        {
            if (!_start.IsAlive)
                return false;

            Path path = RogueLike.AStar.ShortestPath(_start.Position, _target.Position);

            if (path == null)
            {
                new QuipAction(_start, QuipType.Idle, 5).Perform();
                return false;
            }

            _start.CurrentPath = path;

            if (path.Length == 0)
                return false;

            new MoveAction(_start, Direction.GetDirection(_start.Position, path.GetStep(0))).Perform();

            return true;

        }

        public ChasePlayerAction(Monster start, Actor target)
        {
            _start = start;
            _target = target;
        }
    }
}
