using RogueLike.Actors;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
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
        private int _x;
        private int _y;

        public override bool Perform()
        {

            if (_pointOfViewActor is Monster)
            {
                if (Distance.Manhattan.Calculate(_pointOfViewActor.Position, RogueLike.Player.Position) > _pointOfViewActor.FovRange)
                    return false;
            }

            if (_x > 0 && _y > 0)
                _pointOfViewActor.Fov.Calculate(new Point(_x, _y), _pointOfViewActor.FovRange, Distance.Manhattan);
            else
                _pointOfViewActor.Fov.Calculate(_pointOfViewActor.Position, _pointOfViewActor.FovRange, Distance.Manhattan);

            if (_pointOfViewActor is Monster)
            {
                if (_pointOfViewActor.Fov.NewlySeen.Any(p => p == RogueLike.Player.Position))
                    new QuipAction((Monster)_pointOfViewActor, QuipType.Engage, 100).Perform();
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
                    int fRed = (int)(RogueLike.Map.Tiles[p.X, p.Y].DefaultAppearance.Foreground.R * _pointOfViewActor.Fov.DoubleResultView[p]);
                    int fGreen = (int)(RogueLike.Map.Tiles[p.X, p.Y].DefaultAppearance.Foreground.G * _pointOfViewActor.Fov.DoubleResultView[p]);
                    int fBlue = (int)(RogueLike.Map.Tiles[p.X, p.Y].DefaultAppearance.Foreground.B * _pointOfViewActor.Fov.DoubleResultView[p]);
                    int bRed = (int)(RogueLike.Map.Tiles[p.X, p.Y].DefaultAppearance.Background.R * _pointOfViewActor.Fov.DoubleResultView[p]);
                    int bGreen = (int)(RogueLike.Map.Tiles[p.X, p.Y].DefaultAppearance.Background.G * _pointOfViewActor.Fov.DoubleResultView[p]);
                    int bBlue = (int)(RogueLike.Map.Tiles[p.X, p.Y].DefaultAppearance.Background.B * _pointOfViewActor.Fov.DoubleResultView[p]);

                    RogueLike.Map.Tiles[p.X, p.Y].CurrentlySeenAppearance = new ColoredGlyph(new Color(fRed, fGreen, fBlue), new Color(bRed, bGreen, bBlue));
                    RogueLike.Map.Tiles[p.X, p.Y].CurrentlySeen = true;
                    RogueLike.Map.Tiles[p.X, p.Y].IsExplored = true;
                }
                
                if (_x < 0 && _y < 0)
                    foreach (Point p in _pointOfViewActor.Fov.BooleanResultView.Positions())
                    {
                        RogueLike.Map.Tiles[p.X, p.Y].CurrentlySeen = _pointOfViewActor.Fov.BooleanResultView[p];
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

        public UpdateFovAction(Actor pointOfViewActtor, int x = -1, int y = -1)
        {
            _pointOfViewActor = pointOfViewActtor;
            _x = x;
            _y = y;
        }
    }
}
