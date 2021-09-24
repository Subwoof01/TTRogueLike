using GoRogue.DiceNotation;
using GoRogue.Random;
using RogueLike.Actors;
using RogueLike.Extensions;
using RogueLike.Systems;
using RogueLike.Systems.Items;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class MeleeAtackAction : Action
    {
        private Actor _attacker;
        private Actor _defender;
        private KeyValuePair<Attack, string> _attack;

        public override bool Perform()
        {
            if (_attacker.GetType().Equals(_defender.GetType()) || !_defender.IsAlive)
                return false;

            TargetBodyPartAction targetAction = new TargetBodyPartAction(_defender);
            targetAction.Perform();
            BodyPart targetBodyPart = targetAction.Target;

            string attackSourceName = (_attack.Key.Name != "" && _attack.Key.Name != null) ? _attack.Key.Name : _attack.Value;

            RogueLike.MessageLog.PrintLine($"{_attacker.Name} attempts to attack {_defender.Name}'s {targetBodyPart.Type.ToDescription()} with their {attackSourceName}!", Color.DarkCyan);

            int[] attackBonusAndDamageRoll = _attack.Key.AttackAndDamageRoll();

            // TODO: Implement Finesse flag calculation.
            int attackRoll = Dice.Roll("1d20") + _attacker.GetStatModifier(ActorStat.Strength) + attackBonusAndDamageRoll[0];

            int ac = _defender.ArmourClass;

            if (targetBodyPart.IsVital)
                ac += _defender.VitalACBonus;

            string attackMessage = $"Attack Roll: {attackRoll} vs {ac} (10 Base, " +
                string.Format("{0:+0;-0;0}", _defender.GetStatModifier(ActorStat.Dexterity)) +
                " Dex";

            if (targetBodyPart.IsVital)
            {
                attackMessage += $", " +
                string.Format("{0:+0;-0;0}", _defender.VitalACBonus) +
                " Vital Bonus";
            }

            attackMessage += ")";

            RogueLike.MessageLog.PrintLine(attackMessage);

            if (attackRoll >= ac)
            {
                RogueLike.MessageLog.PrintLine($"Hit! {_attacker.Name} strikes the {_defender.Name}'s {targetBodyPart.Type.ToDescription()} with their {attackSourceName}.", Color.DarkGreen);

                // TODO: Implement Finesse flag calculation.
                int damage = attackBonusAndDamageRoll[1] + _attacker.GetStatModifier(ActorStat.Strength);
                if (damage <= 0)
                    damage = 1;

                RogueLike.MessageLog.PrintLine($"{_defender.Name}'s {targetBodyPart.Type.ToDescription()} takes {damage} damage.", Color.Coral);

                _defender.Body.OnBodyPartRemoved += _BodyPartSevered;

                if (_defender.TakeDamage(targetBodyPart, damage))
                    RogueLike.MessageLog.PrintLine($"{_defender.Name} dies.", Color.DarkMagenta);
                else if (_defender.TotalHealth <= _defender.TotalMaxHealth * 0.6)
                {
                    RogueLike.MessageLog.PrintLine($"{_defender.Name} is bloodied.", Color.IndianRed);
                    if (_defender is Monster)
                        new QuipAction((Monster)_defender, QuipType.Bloodied, 100).Perform();
                }

                _defender.Body.OnBodyPartRemoved -= _BodyPartSevered;

                return true;
            }
            RogueLike.MessageLog.PrintLine("Missed...", Color.DarkRed);
            if (_defender is Monster)
                new QuipAction((Monster)_defender, QuipType.Taunt).Perform();
            return false;
        }

        private void _BodyPartSevered(object sender, BodyPartAddedOrRemovedEventArgs e)
        {
            RogueLike.MessageLog.PrintLine($"{_attacker.Name} severs {_defender.Name}'s {e.BodyPart.Type.ToDescription()}!", Color.PaleVioletRed);
        }

        public MeleeAtackAction(Actor attacker, Actor defender, KeyValuePair<Attack, string> attack)
        {
            _attacker = attacker;
            _defender = defender;
            _attack = attack;
        }
    }
}
