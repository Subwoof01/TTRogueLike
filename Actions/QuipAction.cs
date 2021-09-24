using GoRogue.DiceNotation;
using RogueLike.Actors;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Actions
{
    public enum QuipType
    {
        Taunt,
        Engage,
        Flee,
        Idle,
        Patrol,
        Bloodied,
        Noise
    }

    public class QuipAction : Action
    {
        private Monster _quipper;
        private QuipType _type;
        private int _chance;

        public override bool Perform()
        {
            if (RogueLike.Distance.Calculate(_quipper.Position, RogueLike.Player.Position) > RogueLike.Player.HearingRange)
                return false;

            if (Dice.Roll("1d100") >= _chance)
                return false;

            int index = Dice.Roll($"1d{_quipper.Breed.Quips[_type].Count}") - 1;
            KeyValuePair<string, int> language = _quipper.Languages.RandomElementByWeight(e => e.Value);

            string baseQuip = $"({language.Key}) '{_quipper.Breed.Quips[_type][index]}'";
            string quip = $"{_quipper.Name}: ";

            if (RogueLike.Player.Languages.Any(l => l.Key == language.Key))
            {
                quip += baseQuip;
            }
            else
            {
                foreach (char c in baseQuip)
                {
                    if (c.Equals(' ') || c.Equals(':') || c.Equals('!') || c.Equals('?') || c.Equals('\'') || c.Equals('(') || c.Equals(')'))
                    {
                        quip += c;
                        continue;
                    }
                    quip += (char)(c + RogueLike.LanguageShift[language.Key]);
                }
            }

            RogueLike.MessageLog.PrintLine(quip, _quipper.Appearance.Foreground);

            return true;
        }

        public QuipAction(Monster quipper, QuipType type, int percentChance = 30)
        {
            _quipper = quipper;
            _type = type;
            _chance = percentChance;
        }
    }
}
