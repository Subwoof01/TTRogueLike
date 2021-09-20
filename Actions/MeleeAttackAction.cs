using GoRogue.DiceNotation;
using RogueLike.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class MeleeAttackAction : Action
    {
        private Actor _attacker;
        private Actor _defender;

        // TODO: Implement this.
        public override bool Perform()
        {
            RogueLike.MessageLog.PrintLine($"{_attacker.Name} attacks {_defender.Name}!");

            int attackRoll = Dice.Roll("1d20") + _attacker.GetStatModifier(ActorStat.Strength);
            RogueLike.MessageLog.PrintLine($"Attack Roll: {attackRoll} vs {_defender.ArmourClass}");

            if (attackRoll >= _defender.ArmourClass)
            {
                RogueLike.MessageLog.PrintLine("Attack hits!");
                int damage = Dice.Roll($"1d4") + _attacker.GetStatModifier(ActorStat.Strength);

                RogueLike.MessageLog.PrintLine($"Damage: {damage}");
                _defender.TakeDamage(damage);

                RogueLike.MessageLog.PrintLine($"{_defender.Name} health: {_defender.Health}");
                return true;
            }
            RogueLike.MessageLog.PrintLine("Attack misses...");
            return false;
        }

        public MeleeAttackAction(Actor attacker, Actor defender)
        {
            _attacker = attacker;
            _defender = defender;
        }
    }
}
