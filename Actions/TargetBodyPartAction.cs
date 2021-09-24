using GoRogue.Random;
using RogueLike.Actors;
using RogueLike.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public class TargetBodyPartAction : Action
    {
        private Actor _target;
        public BodyPart Target;

        public override bool Perform()
        {
            Array values = Enum.GetValues(typeof(BodyPartType));

            Target = _target.Body.Parts.RandomElementByWeight<BodyPart>(x => (x.IsVital) ? 10 + x.BelongsTo.GetStatModifier(ActorStat.Intelligence) : 10);
            return true;
        }

        public TargetBodyPartAction(Actor target)
        {
            _target = target;
        }
    }
}
