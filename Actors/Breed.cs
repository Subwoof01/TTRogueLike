using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actors
{
    public class Breed
    {
        public List<string> Flags { get; private set; }


        // TODO: Implement following:
        // - List of (unique) moves (new type Use). See: https://youtu.be/JxI3Eu5DPwE
        // - Drops.

        public Breed()
        {
            Flags = new List<string>();
        }
    }
}
