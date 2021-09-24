using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Tiles
{
    public class Tile : Entity
    {
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public List<string> Flags;

        public ColoredGlyph CurrentlySeenAppearance;
        public ColoredGlyph ExploredAppearance;
        public ColoredGlyph DefaultAppearance;

        private bool _isExplored;
        public bool IsExplored 
        { 
            get
            {
                return _isExplored;
            }
            set
            {
                _isExplored = value;
                UpdateAppearance();
            }
        }
        private bool _currentlySeen;
        public bool CurrentlySeen 
        { 
            get
            {
                return _currentlySeen;
            }
            set
            {
                _currentlySeen = value;
                UpdateAppearance();
            }
        }

        private void UpdateAppearance()
        {
            if (CurrentlySeen)
                Appearance = new ColoredGlyph(CurrentlySeenAppearance.Foreground, CurrentlySeenAppearance.Background, Appearance.Glyph);
            else if (IsExplored)
                Appearance = new ColoredGlyph(ExploredAppearance.Foreground, ExploredAppearance.Background, Appearance.Glyph);
            else
                Appearance = new ColoredGlyph(Color.Black, Color.Black, Appearance.Glyph);
        }

        private void SetupAppearances(ColoredGlyph appearance)
        {
            //CurrentlySeenAppearance = appearance;

            int fRed = (int)(appearance.Foreground.R * 0.1);
            int fGreen = (int)(appearance.Foreground.G * 0.1);
            int fBlue = (int)(appearance.Foreground.B * 0.1);
            int bRed = (int)(appearance.Background.R * 0.1);
            int bGreen = (int)(appearance.Background.G * 0.1);
            int bBlue = (int)(appearance.Background.B * 0.1);

            ExploredAppearance = new ColoredGlyph(new Color(fRed, fGreen, fBlue), new Color(bRed, bGreen, bBlue));
        }

        public Tile(ref ColoredGlyph appearance, int zIndex) : base(ref appearance, zIndex)
        {
            DefaultAppearance = appearance;
            SetupAppearances(appearance);
            Flags = new List<string>();
        }

        public Tile(ColoredGlyph appearance, int zIndex) : base(appearance, zIndex)
        {
            DefaultAppearance = appearance;
            SetupAppearances(appearance);
            Flags = new List<string>();
        }

        public Tile(Color foreground, Color background, int glyph, int zIndex) : base(foreground, background, glyph, zIndex)
        {
            DefaultAppearance = new ColoredGlyph(foreground, background, glyph);
            SetupAppearances(new ColoredGlyph(foreground, background, glyph));
            Flags = new List<string>();
        }
    }
}
