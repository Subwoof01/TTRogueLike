using RogueLike.Actors;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class PeakAction : Action
    {
        private Actor _peaker;

        public override bool Perform()
        {
            Point[] points = new Point[8];
            points[0] = _peaker.Position + Direction.UpLeft;
            points[1] = _peaker.Position + Direction.Up;
            points[2] = _peaker.Position + Direction.UpRight;
            points[3] = _peaker.Position + Direction.Right;
            points[4] = _peaker.Position + Direction.DownRight;
            points[5] = _peaker.Position + Direction.Down;
            points[6] = _peaker.Position + Direction.DownLeft;
            points[7] = _peaker.Position + Direction.Left;

            List<Point> walkables = new List<Point>();

            for (int i = 0; i < points.Length; i++)
            {
                if (RogueLike.Map.Tiles[points[i].X, points[i].Y].IsWalkable)
                    walkables.Add(RogueLike.Map.Tiles[points[i].X, points[i].Y].Position);
            }

            foreach (Point p in walkables)
            {
                new UpdateFovAction(_peaker, p.X, p.Y).Perform();
            }
            return true;
        }

        public PeakAction(Actor peaker)
        {
            _peaker = peaker;
        }
    }
}
