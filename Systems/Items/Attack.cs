using GoRogue.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems.Items
{
    public class Attack
    {
        private int _attackBonus;
        private string _damageDice;

        public string Name;
        public List<DamageType> DamageTypes;
        public int AttackRange; 

        public int[] AttackAndDamageRoll()
        {
            return new int[] { _attackBonus, Dice.Roll(_damageDice) };
        }

        public Attack(int attackBonus, string damageDice, int attackRange = 1)
        {
            _attackBonus = attackBonus;
            _damageDice = damageDice;
            DamageTypes = new List<DamageType>();
            AttackRange = attackRange;
        }
    }
}
