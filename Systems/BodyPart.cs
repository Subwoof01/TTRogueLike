using RogueLike.Actors;
using RogueLike.Extensions;
using RogueLike.Systems.Equipment;
using RogueLike.Systems.Items;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems
{
    public class BodyPart
    {
        public BodyPartType Type;

        public int Health { get; set; }
        public bool IsSevered { get; private set; }
        public bool IsVital { get; private set; }
        public ColoredGlyph Appearance { get; set; }
        public Dictionary<EquipSlot, Item?> EquippedItems { get; private set; }
        public List<Attack> NaturalAttacks { get; private set; }

        private int _maxHealth;
        public int MaxHealth
        {
            get
            {
                return _maxHealth + ((BelongsTo.Stats[ActorStat.Constitution] - 10) / 2 * (int)BelongsTo.Level);
            }
            set
            {
                _maxHealth = value;
            }
        }
        public List<BodyPart> ConnectedTo { get; }
        public Actor BelongsTo { get; }

        public BodyPart(BodyPartType type, int maxHealth, Actor belongsTo, bool isVital = false)
        {
            IsVital = isVital;
            IsSevered = false;
            Type = type;
            _maxHealth = maxHealth;
            BelongsTo = belongsTo;
            Health = MaxHealth;
            ConnectedTo = new List<BodyPart>();
            Appearance = new ColoredGlyph(Color.White);
            EquippedItems = new Dictionary<EquipSlot, Item?>();
            NaturalAttacks = new List<Attack>();
        }

        public void EquipItem(Item item)
        {
            EquippedItems[item.Slot] = item;
        }

        public void UnEquipItem(Item item)
        {
            EquippedItems[item.Slot] = null;
        }

        public Item UnEquipItemInSlot(EquipSlot slot)
        {
            Item item = EquippedItems[slot];
            EquippedItems[slot] = null;
            return item;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            
            if (Health <= 0)
                Die();
        }

        public void Die()
        {
            if (Health < 0)
            {
                IsSevered = true;
                BelongsTo.Body.RemoveBodyPart(this);
            }

            Health = 0;

            foreach (BodyPart bp in ConnectedTo)
            {
                bp.Die();
            }
        }
    }
}
