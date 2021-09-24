using RogueLike.Systems.Equipment;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems.Items
{
    public class Item : Entity
    {
        public Item(ref ColoredGlyph appearance, int zIndex) : base(ref appearance, zIndex)
        {
            Flags = new List<string>();
        }

        public Item(ColoredGlyph appearance, int zIndex) : base(appearance, zIndex)
        {
            Flags = new List<string>();
        }

        public Item(Color foreground, Color background, int glyph, int zIndex) : base(foreground, background, glyph, zIndex)
        {
            Flags = new List<string>();
        }

        public EquipSlot Slot { get; set; }
        public int SlotsNeeded { get; set; }
        public Attack Melee { get; set; }
        public Attack Ranged { get; set; }
        public Defense Defense { get; set; }
        public Use Use { get; set; }
        public List<string> Flags { get; }
    }
}
