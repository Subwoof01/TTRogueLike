using RogueLike.Actions;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actors
{
    public class Player : Actor
    {
        public bool ReadKeyboardInputs = true;

        public Player(ref ColoredGlyph appearance, int zIndex) : base(ref appearance, zIndex)
        {
        }

        public Player(ColoredGlyph appearance, int zIndex) : base(appearance, zIndex)
        {
        }

        public Player(Color foreground, Color background, int glyph, int zIndex) : base(foreground, background, glyph, zIndex) 
        {
        }

        public override Action TakeTurn()
        {
            if (!ReadKeyboardInputs)
                return null;

            Keyboard keyboard = Game.Instance.Keyboard;

            if (!keyboard.HasKeysDown && !keyboard.HasKeysPressed)
                return null;

            if (keyboard.IsKeyPressed(Keys.Up))
                return new MoveAction(this, Direction.Up);
            if (keyboard.IsKeyPressed(Keys.Down))
                return new MoveAction(this, Direction.Down);
            if (keyboard.IsKeyPressed(Keys.Left))
                return new MoveAction(this, Direction.Left);
            if (keyboard.IsKeyPressed(Keys.Right))
                return new MoveAction(this, Direction.Right);
            if (keyboard.IsKeyPressed(Keys.P))
                return new PeakAction(this);

            return null;
        }
    }
}
