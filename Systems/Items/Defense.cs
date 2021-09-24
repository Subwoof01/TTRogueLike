using RogueLike.Systems.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems.Items
{
    public class Defense
    {
        public int TotalACBonus { get; private set; }
        public int PositionalACBonus { get; private set; }

        public Defense(int totalACBonus, int positionalACBonus)
        {
            TotalACBonus = totalACBonus;
            PositionalACBonus = positionalACBonus;
        }
    }
}
