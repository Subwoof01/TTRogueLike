using SadConsole;
using SadConsole.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SadConsole.UI.Controls.SurfaceViewer;
using Console = SadConsole.Console;

namespace RogueLike.Systems
{
    public class MessageLog
    {
        private Console _console;
        private ScrollableConsole _scrollConsole;

        public MessageLog(Console console)
        {
            _console = console;
            _scrollConsole = new ScrollableConsole(console.Width, console.Height - 1, 1000);
            _scrollConsole.Cursor.PrintAppearanceMatchesHost = false;
            _scrollConsole.Clear();

            _console.Children.Add(_scrollConsole);

            _scrollConsole.Position = (0, 1);
        }

        public void Print(string message, Color? colour = null)
        {
            Color c = (Color)((colour == null) ? Color.White : colour);

            _scrollConsole.Cursor
                .SetPrintAppearance(c)
                .Print(message);

            DrawConsoleSeparator();
        }

        public void PrintLine(string message, Color ?colour = null)
        {
            Color c = (Color)((colour == null) ? Color.White : colour);

            _scrollConsole.Cursor
                .NewLine()
                .SetPrintAppearance(c)
                .Print(message);

            DrawConsoleSeparator();
        }

        public void Scroll(bool up = true)
        {
        }

        public void DrawConsoleSeparator()
        {
            _console.DrawLine(
                    new Point(0, 0),
                    new Point(_console.Width, 0),
                    null,
                    foreground: Color.DarkSlateGray,
                    background: Color.DarkSlateGray
                );
        }
    }
}
