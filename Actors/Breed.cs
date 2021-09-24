using RogueLike.Actions;
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

        public Dictionary<QuipType, List<string>> Quips { get; private set; }

        // TODO: Implement following:
        // - List of (unique) moves (new type Use). See: https://youtu.be/JxI3Eu5DPwE
        // - Drops.

        public void AddQuip(QuipType type, string quip)
        {
            if (!Quips.Any(qt => qt.Key == type))
                Quips[type] = new List<string>();
            Quips[type].Add(quip);
        }

        public Breed()
        {
            Flags = new List<string>();
            Quips = new Dictionary<QuipType, List<string>>();
        }
    }
}
