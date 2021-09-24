using RogueLike.Actors;
using RogueLike.Systems.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems.Equipment
{
    public enum EquipSlot
    {
        NONE,
        Head,
        Neck,
        Chest,
        Shoulders,
        Hand,
        Foot,
        Finger
    }

    // TODO: REDESIGN WHOLE SYSTEM
    public class EquipmentSystem
    {
        private Body _owner;

        public EquipmentSystem(Body owner)
        {
            owner.OnBodyPartAdded += _body_OnBodyPartAdded;
            owner.OnBodyPartRemoved += _body_OnBodyPartRemoved;
            _owner = owner;
        }

        private void _body_OnBodyPartRemoved(object sender, BodyPartAddedOrRemovedEventArgs e)
        {
        }

        private void _body_OnBodyPartAdded(object sender, BodyPartAddedOrRemovedEventArgs e)
        {
            switch (e.BodyPart.Type)
            {
                case BodyPartType.Hand:
                    e.BodyPart.EquippedItems.Add(EquipSlot.Hand, null);
                    e.BodyPart.EquippedItems.Add(EquipSlot.Finger, null);
                    break;
                case BodyPartType.Head:
                    e.BodyPart.EquippedItems.Add(EquipSlot.Head, null);
                    break;
                case BodyPartType.Chest:
                    e.BodyPart.EquippedItems.Add(EquipSlot.Chest, null);
                    e.BodyPart.EquippedItems.Add(EquipSlot.Shoulders, null);
                    break;
                case BodyPartType.Foot:
                    e.BodyPart.EquippedItems.Add(EquipSlot.Foot, null);
                    break;
            }
        } 
        
        public bool EquipItemOnActor(Item item)
        {
            List<BodyPart> openSlots = new List<BodyPart>();

            int openSlotCount = 0;
            foreach (BodyPart bp in _owner.Parts)
            {
                foreach (KeyValuePair<EquipSlot, Item?> slot in bp.EquippedItems)
                {
                    if (slot.Key == item.Slot)
                        if (slot.Value == null)
                        {
                            openSlots.Add(bp);
                            openSlotCount++;
                        }

                    if (openSlotCount >= item.SlotsNeeded)
                    {
                        foreach (BodyPart openSlot in openSlots)
                            openSlot.EquippedItems[slot.Key] = item;

                        return true;
                    }
                }
            }

            return false;
        }

        public bool UnEquipItemOnActor(Item item)
        {
            bool removed = false;
            foreach (BodyPart bp in _owner.Parts)
            {
                if (bp.EquippedItems[item.Slot] != null)
                {
                    bp.UnEquipItem(item);
                    removed = true;
                }
            }
            return removed;
        }

        public bool UnEquipItemOnActorInSlot(EquipSlot slot)
        {
            bool removed = false;
            Item item = null;
            foreach (BodyPart bp in _owner.Parts)
            {
                if (bp.EquippedItems[slot] != null)
                {
                    item = bp.UnEquipItemInSlot(slot);
                    removed = true;
                }
            }

            if (item != null)
            {
                // TODO: Add item dropping/going to inventory here.
            }

            return removed;
        }
    }
}
