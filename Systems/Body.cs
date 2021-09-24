using GoRogue.Random;
using RogueLike.Actors;
using RogueLike.Systems.Equipment;
using RogueLike.Systems.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems
{
    public class Body
    {
        public Actor BelongsTo;
        private List<BodyPart> _bodyParts;

        private BodyPart _chest;
        public BodyPart Chest
        {
            get
            {
                return _chest;
            }
            set
            {
                if (value != null)
                    UpdateOnBodyPartAdded(value);
                else
                    UpdateOnBodyPartRemoved(_chest);
                _chest = value;
            }
        }
        public ReadOnlyCollection<BodyPart> Parts { get; }

        public delegate void BodyPartAddedHandler(object sender, BodyPartAddedOrRemovedEventArgs e);
        public delegate void BodyPartRemovedHandler(object sender, BodyPartAddedOrRemovedEventArgs e);
        public event BodyPartAddedHandler OnBodyPartAdded;
        public event BodyPartRemovedHandler OnBodyPartRemoved;


        public List<BodyPart> Legs
        {
            get
            {
                List<BodyPart> legs = new List<BodyPart>();

                legs.AddRange(Parts.Where(bp => bp.Type == BodyPartType.Leg));

                return legs;
            }
        }
        public List<BodyPart> Arms
        {
            get
            {
                List<BodyPart> arms = new List<BodyPart>();

                arms.AddRange(Parts.Where(bp => bp.Type == BodyPartType.Arm));

                return arms;
            }
        }
        public List<BodyPart> Heads
        {
            get
            {
                List<BodyPart> head = new List<BodyPart>();

                head.AddRange(Parts.Where(bp => bp.Type == BodyPartType.Head));

                return head;
            }
        }
        public List<BodyPart> Feet
        {
            get
            {
                List<BodyPart> feet = new List<BodyPart>();

                feet.AddRange(Parts.Where(bp => bp.Type == BodyPartType.Foot));

                return feet;
            }
        }
        public List<BodyPart> Necks
        {
            get
            {
                List<BodyPart> necks = new List<BodyPart>();

                necks.AddRange(Parts.Where(bp => bp.Type == BodyPartType.Neck));

                return necks;
            }
        }
        public List<BodyPart> Hands
        {
            get
            {
                List<BodyPart> hands = new List<BodyPart>();

                hands.AddRange(Parts.Where(bp => bp.Type == BodyPartType.Hand));

                return hands;
            }
        }

        public void AddBodyPart(BodyPart part)
        {
            _bodyParts.Add(part);
            UpdateOnBodyPartAdded(part);
        }

        public void RemoveBodyPart(BodyPart part)
        {
            _bodyParts.Remove(part);
            UpdateOnBodyPartRemoved(part);
        }

        public List<BodyPart> GetBodyPartsByType(BodyPartType type)
        {
            switch (type)
            {
                case BodyPartType.Head:
                    return Heads;
                case BodyPartType.Neck:
                    return Necks;
                case BodyPartType.Arm:
                    return Arms;
                case BodyPartType.Hand:
                    return Hands;
                case BodyPartType.Leg:
                    return Legs;
                case BodyPartType.Foot:
                    return Feet;
                default:
                    return null;
            }
        }

        private void UpdateOnBodyPartAdded(BodyPart part)
        {
            if (OnBodyPartAdded == null)
                return;

            OnBodyPartAdded(this, new BodyPartAddedOrRemovedEventArgs(part));
        }
        private void UpdateOnBodyPartRemoved(BodyPart part)
        {
            if (OnBodyPartRemoved == null)
                return;

            OnBodyPartRemoved(this, new BodyPartAddedOrRemovedEventArgs(part));
        }

        public BodyPart GetRandomBodyPart(BodyPartType type)
        {
            List<BodyPart> parts = GetBodyPartsByType(type);

            if (parts == null)
                return Chest;

            return parts[GlobalRandom.DefaultRNG.Next(parts.Count)];
        }

        public void AddNewArm(int maxHealth = 20)
        {
            BodyPart arm = new BodyPart(BodyPartType.Arm, maxHealth, BelongsTo);
            BodyPart hand = new BodyPart(BodyPartType.Hand, (int)(maxHealth * 0.75), BelongsTo);

            Chest.ConnectedTo.Add(arm);
            arm.ConnectedTo.Add(hand);

            _bodyParts.Add(arm);
            UpdateOnBodyPartAdded(arm);
            _bodyParts.Add(hand);
            UpdateOnBodyPartAdded(hand);
        }

        public void AddNewLeg(int maxHealth = 20)
        {
            BodyPart leg = new BodyPart(BodyPartType.Leg, maxHealth, BelongsTo);
            BodyPart foot = new BodyPart(BodyPartType.Foot, (int)(maxHealth * 0.75), BelongsTo);

            Chest.ConnectedTo.Add(leg);
            leg.ConnectedTo.Add(foot);

            _bodyParts.Add(leg);
            UpdateOnBodyPartAdded(leg);
            _bodyParts.Add(foot);
            UpdateOnBodyPartAdded(foot);
        }
        public void AddNewHead(int maxHealth = 20)
        {
            BodyPart head = new BodyPart(BodyPartType.Head, maxHealth, BelongsTo, true);
            BodyPart neck = new BodyPart(BodyPartType.Neck, (int)(maxHealth * 0.75), BelongsTo, true);

            Chest.ConnectedTo.Add(neck);
            neck.ConnectedTo.Add(head);

            _bodyParts.Add(neck);
            UpdateOnBodyPartAdded(neck);
            _bodyParts.Add(head);
            UpdateOnBodyPartAdded(head);
        }

        public BodyPart AddNewChest(int maxHealth = 30)
        {
            BodyPart chest = new BodyPart(BodyPartType.Chest, maxHealth, BelongsTo, true);
            _bodyParts.Add(chest);
            return chest;
        }

        public bool AddNaturalAttackToLimb(BodyPartType slot, Attack attack)
        {
            foreach (BodyPart bp in GetBodyPartsByType(slot))
            {
                if (!bp.NaturalAttacks.Contains(attack))
                {
                    bp.NaturalAttacks.Add(attack);
                    return true;
                }
            }
            return false;
        }

        public Body(Actor belongsTo)
        {
            BelongsTo = belongsTo;
            _bodyParts = new List<BodyPart>();
            Parts = new ReadOnlyCollection<BodyPart>(_bodyParts);
        }
    }

    public class BodyPartAddedOrRemovedEventArgs : EventArgs
    {
        public BodyPart BodyPart { get; private set; }

        public BodyPartAddedOrRemovedEventArgs(BodyPart part)
        {
            BodyPart = part;
        }
    }
}
