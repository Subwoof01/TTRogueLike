using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems.Items
{
    public class DamageType
    {
        public enum Physical
        {
            Piercing,
            Slashing,
            Bludgeoning
        }

        public enum Elemental
        {
            Acid,
            Cold,
            Electricity,
            Fire,
            Sonic
        }

        public enum Energy
        {
            Positive,
            Negative,
            Force
        }

        public Enum Type { get; }

        public DamageType(Enum type)
        {
            Type = type;
        }
    }
}
