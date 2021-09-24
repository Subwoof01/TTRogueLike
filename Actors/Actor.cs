using GoRogue.DiceNotation;
using GoRogue.FOV;
using RogueLike.Extensions;
using RogueLike.Systems;
using RogueLike.Systems.Equipment;
using RogueLike.Systems.Items;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actors
{
    public enum BodyPartType
    {
        Head,
        Neck,
        Chest,
        Arm,
        Hand,
        Leg,
        Foot,
    }

    public abstract class Actor : Entity, IScheduleable
    {
        public bool IsAlive { get; private set; }
        public float Level { get; set; }

        public int TotalMaxHealth
        {
            get
            {
                int totalMaxHealth = 0;

                foreach (BodyPart bp in Body.Parts)
                    totalMaxHealth += bp.MaxHealth;

                return totalMaxHealth;
            }
        }

        public int TotalMaxHealthWithoutVitals
        {
            get
            {
                int totalMaxHealth = 0;

                foreach (BodyPart bp in Body.Parts)
                {
                    if (bp.IsVital)
                        continue;
                    totalMaxHealth += bp.MaxHealth;
                }

                return totalMaxHealth;
            }
        }

        public int TotalHealthWithoutVitals
        {
            get
            {
                int totalHealth = 0;

                foreach (BodyPart bp in Body.Parts)
                {
                    if (bp.IsVital)
                        continue;
                    totalHealth += bp.Health;
                }

                return totalHealth;
            }
        }

        public int TotalHealth
        {
            get
            {
                int totalHealth = 0;

                foreach (BodyPart bp in Body.Parts)
                    totalHealth += bp.Health;

                return totalHealth;
            }
        }

        private int _armourClass;
        public int ArmourClass
        {
            get
            {
                return _armourClass + GetStatModifier(ActorStat.Dexterity);
            }
            set
            {
                _armourClass = value;
            }
        }

        public int VitalACBonus
        {
            get
            {
                int bonus = 1;
                int dex = GetStatModifier(ActorStat.Dexterity) * 2;

                if (dex > bonus)
                    return dex;
                else
                    return bonus;
            }
        }
        public Body Body { get; private set; }
        public Dictionary<ActorStat, int> Stats { get; private set; }
        public Dictionary<string, int> Languages { get; private set; }
        public EquipmentSystem Equipment { get; private set; }
        public List<Item> Inventory { get; private set; }

        private int _speed = 0;
        public int Speed
        {
            get
            {
                int baseSlowdown = 20;
                int dexMod = GetStatModifier(ActorStat.Dexterity);

                //if (BodyParts[BodyPartType.Foot].Health == 0)
                //    baseSlowdown += 5;
                //if (BodyParts[BodyPartType.Foot].Health == 0)
                //    baseSlowdown += 5;

                //if (BodyParts[BodyPartType.Leg].Health == 0)
                //    baseSlowdown += 10;
                //if (BodyParts[BodyPartType.Leg].Health == 0)
                //    baseSlowdown += 10;

                //if (BodyParts[BodyPartType.Leg].Health == 0 && BodyParts[BodyPartType.Leg].Health == 0)
                //    baseSlowdown += 40;

                return baseSlowdown - dexMod - _speed;
            }
            set
            {
                _speed = value;
            }
        }
        public RecursiveShadowcastingFOV Fov { get; set; }
        public int FovRange { get; set; }
        public int HearingRange { get; set; }

        public Actor(ref ColoredGlyph appearance, int zIndex) : base(ref appearance, zIndex)
        {
            InitialiseMembers();
            InitialiseStats(); 
            InitialiseBodyParts();
        }

        public Actor(ColoredGlyph appearance, int zIndex) : base(appearance, zIndex)
        {
            InitialiseMembers();
            InitialiseStats();
            InitialiseBodyParts();
        }

        public Actor(Color foreground, Color background, int glyph, int zIndex) : base(foreground, background, glyph, zIndex)
        {
            InitialiseMembers();
            InitialiseStats();
            InitialiseBodyParts();
        }

        public bool TakeDamage(BodyPart bodyPart, int damage)
        {
            bodyPart.TakeDamage(damage);

            if (Body.Chest.Health <= 0 || Body.Necks.Count <= 0 || Body.Heads.Count <= 0 || TotalHealthWithoutVitals <= 0)
                return Die();

            return false;
        }

        public bool Die()
        {
            IsAlive = false;

            if (this is Player)
                return true;

            RogueLike.Map.Actors.Remove(this);
            RogueLike.Renderer.Remove(this);
            RogueLike.SchedulingSystem.Remove(this);
            return true;
        }

        public int GetStatModifier(ActorStat stat)
        {
            return (Stats[stat] - 10) / 2;
        }

        private void InitialiseStats()
        {
            Level = 1;
            Stats = new Dictionary<ActorStat, int>();
            foreach (ActorStat stat in Enum.GetValues(typeof(ActorStat)))
            {
                Stats[stat] = Dice.Roll("4d6k3");
            }
        }

        private void InitialiseBodyParts()
        {
            Body = new Body(this);
            Equipment = new EquipmentSystem(Body);
            Body.Chest = Body.AddNewChest();
            Body.AddNewHead();
            Body.AddNewArm();
            Body.AddNewArm();
            Body.AddNewLeg();
            Body.AddNewLeg();

        }

        private void InitialiseMembers()
        {
            IsAlive = true;

            Fov = new RecursiveShadowcastingFOV(RogueLike.Map.GetTransparency());
            Languages = new Dictionary<string, int>();
            Inventory = new List<Item>();
        }

        public abstract Action TakeTurn();
    }
}
