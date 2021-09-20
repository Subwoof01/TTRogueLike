using GoRogue.DiceNotation;
using GoRogue.FOV;
using RogueLike.Systems;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actors
{
    public abstract class Actor : Entity, IScheduleable
    {
        public int Health;
        public int Level { get; set; }

        protected int _maxHealth;
        public int MaxHealth
        {
            get
            {
                return _maxHealth + ((Stats[ActorStat.Constitution] - 10) / 2 * Level);
            }
            set
            {
                _maxHealth = value;
            }
        }

        private int _armourClass;
        public int ArmourClass
        {
            get
            {
                return _armourClass + ((Stats[ActorStat.Dexterity] - 10) / 2);
            }
            set
            {
                _armourClass = value;
            }
        }

        public Dictionary<ActorStat, int> Stats { get; private set; }

        public int Speed { get; set; }

        public RecursiveShadowcastingFOV Fov { get; set; }
        public int FovRange { get; set; }

        public Actor(ref ColoredGlyph appearance, int zIndex) : base(ref appearance, zIndex)
        {
            Fov = new RecursiveShadowcastingFOV(RogueLike.Map.GetTransparency());
            InitialiseStats(); 
        }

        public Actor(ColoredGlyph appearance, int zIndex) : base(appearance, zIndex)
        {
            Fov = new RecursiveShadowcastingFOV(RogueLike.Map.GetTransparency());
            InitialiseStats();
        }

        public Actor(Color foreground, Color background, int glyph, int zIndex) : base(foreground, background, glyph, zIndex)
        {
            Fov = new RecursiveShadowcastingFOV(RogueLike.Map.GetTransparency());
            InitialiseStats();
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
                Die();
        }

        public void Die()
        {
            if (this is Player)
                return;

            RogueLike.Map.Actors.Remove(this);
            RogueLike.Renderer.Remove(this);
            RogueLike.SchedulingSystem.Remove(this);
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

        public abstract Action TakeTurn();
    }
}
