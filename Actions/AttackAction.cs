using RogueLike.Actors;
using RogueLike.Extensions;
using RogueLike.Systems;
using RogueLike.Systems.Equipment;
using RogueLike.Systems.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class AttackAction : Action
    {
        private Actor _attacker;
        private Actor _defender;
        private bool _isRanged;

        public override bool Perform()
        {
            if (_attacker.GetType().Equals(_defender.GetType()) || !_defender.IsAlive)
                return false;

            Dictionary<Attack, string> attacks = new Dictionary<Attack, string>();

            foreach (BodyPart bp in _attacker.Body.Parts)
            {
                bool includeNaturalAttack = true;

                foreach (KeyValuePair<EquipSlot, Item> item in bp.EquippedItems)
                {
                    if (item.Value == null)
                        continue;

                    if (_isRanged)
                    {
                        if (item.Value.Ranged != null)
                            if (!attacks.ContainsKey(item.Value.Ranged))
                                attacks.Add(item.Value.Ranged, item.Value.Name);
                        if (item.Value.Flags.Contains("DisablesNaturalAttack"))
                            includeNaturalAttack = false;

                    }
                    else
                    {
                        if (item.Value.Melee != null)
                            if (!attacks.ContainsKey(item.Value.Melee))
                                attacks.Add(item.Value.Melee, item.Value.Name);
                        if (item.Value.Flags.Contains("DisablesNaturalAttack"))
                            includeNaturalAttack = false;

                    }
                }

                if (includeNaturalAttack)
                    foreach (Attack naturalAttack in bp.NaturalAttacks)
                    {
                        if (_isRanged)
                        {
                            if (naturalAttack.AttackRange > 1)
                                attacks.Add(naturalAttack, bp.Type.ToDescription());
                        }
                        else
                        {
                            if (naturalAttack.AttackRange <= 1)
                                attacks.Add(naturalAttack, bp.Type.ToDescription());
                        }
                    }
            }

            if (attacks.Count == 0)
                return false;

            foreach (KeyValuePair<Attack, string> attack in attacks)
            {
                if (_isRanged)
                    continue;
                else
                    new MeleeAtackAction(_attacker, _defender, attack).Perform();
            }

            return true;
        }

        public AttackAction(Actor attacker, Actor defender, bool isRanged = false)
        {
            _attacker = attacker;
            _defender = defender;
            _isRanged = isRanged;
        }
    }
}
