using RogueLike.Actors;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class UpdateFovAction : Action
    {
        private Actor _pointOfViewActor;

        public override bool Perform()
        {
            if (_pointOfViewActor is Monster)
            {
                if (Distance.Manhattan.Calculate(_pointOfViewActor.Position, RogueLike.Player.Position) > _pointOfViewActor.FovRange)
                    return false;
            }

            _pointOfViewActor.Fov.Calculate(_pointOfViewActor.Position, _pointOfViewActor.FovRange, Distance.Manhattan);

            if (_pointOfViewActor is Monster)
            {
                if (_pointOfViewActor.Fov.NewlySeen.Any(p => p == RogueLike.Player.Position))
                    RogueLike.MessageLog.PrintLine($"{_pointOfViewActor.Name}: 'Taste steel, heathen!'", Color.Red);
                if (_pointOfViewActor.Fov.CurrentFOV.Any(p => p == RogueLike.Player.Position))
                {
                    return true;
                }
                return false;
            }
            else
            {
                foreach (Point p in _pointOfViewActor.Fov.CurrentFOV)
                {
                    RogueLike.Map.Tiles[p.X, p.Y].CurrentlySeen = true;
                    RogueLike.Map.Tiles[p.X, p.Y].IsExplored = true;
                }

                foreach (Point p in _pointOfViewActor.Fov.NewlyUnseen)
                {
                    RogueLike.Map.Tiles[p.X, p.Y].CurrentlySeen = false;
                }

                foreach (Actor m in RogueLike.Map.Actors)
                {
                    if (RogueLike.Map.Tiles[m.Position.X, m.Position.Y].CurrentlySeen)
                    {
                        m.IsVisible = true;
                        continue;
                    }
                    m.IsVisible = false;
                }
                return true;
            }
        }

        public UpdateFovAction(Actor pointOfView)
        {
            _pointOfViewActor = pointOfView;
        }
    }
}
