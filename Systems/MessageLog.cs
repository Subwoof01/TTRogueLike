using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Console;

namespace RogueLike.Systems
{
    public class MessageLog
    {
        private readonly Console _console;

        public MessageLog(Console console)
        {
            _console = console;
        }

        public void Print(string message, Color? colour = null)
        {
            Color c = (Color)((colour == null) ? Color.White : colour);

            _console.Cursor
                .SetPrintAppearance(c)
                .Print(message);
        }

        public void PrintLine(string message, Color ?colour = null)
        {
            Color c = (Color)((colour == null) ? Color.White : colour);

            _console.Cursor
                .NewLine()
                .SetPrintAppearance(c)
                .Print(message);
        }
    }
}
