using RogueLike.Actions;
using RogueLike.Extensions;
using RogueLike.Systems;
using RogueLike.Systems.Equipment;
using RogueLike.Systems.Items;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RogueLike.Actors
{
    public class MonsterFactory
    {
        public Monster Get(string type)
        {
            string path = $@"{Environment.CurrentDirectory}\Data\Monsters\{type}.xml";

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            XmlDocument monsterXml = new XmlDocument();
            monsterXml.Load(path);

            XmlNode fgColour = monsterXml.SelectSingleNode("/Monster/ForegroundColour");
            Color fg = new Color(
                    int.Parse(fgColour.ChildNodes.Item(0).InnerText),
                    int.Parse(fgColour.ChildNodes.Item(1).InnerText),
                    int.Parse(fgColour.ChildNodes.Item(2).InnerText)
                );

            XmlNode bgColour = monsterXml.SelectSingleNode("/Monster/BackgroundColour");
            Color bg = new Color(
                    int.Parse(bgColour.ChildNodes.Item(0).InnerText),
                    int.Parse(bgColour.ChildNodes.Item(1).InnerText),
                    int.Parse(bgColour.ChildNodes.Item(2).InnerText)
                );

            Monster monster = new Monster(fg, bg, char.Parse(monsterXml.SelectSingleNode("/Monster/Glyph").InnerText), 3)
            {
                Name = monsterXml.SelectSingleNode("/Monster/Name").InnerText,
                Level = float.Parse(monsterXml.SelectSingleNode("/Monster/Level").InnerText),
                ArmourClass = int.Parse(monsterXml.SelectSingleNode("/Monster/AC").InnerText),
                FovRange = int.Parse(monsterXml.SelectSingleNode("/Monster/FovRange").InnerText),
            };
            XmlNode stats = monsterXml.SelectSingleNode("/Monster/Stats");
            int count = 0;
            foreach (ActorStat stat in Enum.GetValues(typeof(ActorStat)))
            {
                monster.Stats[stat] = int.Parse(stats.ChildNodes.Item(0).InnerText);
                count++;
            }

            foreach (BodyPartType bpt in Enum.GetValues(typeof(BodyPartType)))
            {
                List<BodyPart> bodyParts = monster.Body.GetBodyPartsByType(bpt);
                if (bodyParts == null)
                    continue;

                foreach (BodyPart bp in bodyParts)
                {
                    bp.MaxHealth = int.Parse(monsterXml.SelectSingleNode($"/Monster/MaxHealth/{bpt}").InnerText);
                    bp.Health = int.Parse(monsterXml.SelectSingleNode($"/Monster/MaxHealth/{bpt}").InnerText);
                }
            }

            monster.Breed = new Breed();

            XmlNode quips = monsterXml.SelectSingleNode("/Monster/Quips");

            foreach (XmlNode quip in quips.ChildNodes)
            {
                QuipType qt;
                Enum.TryParse(quip.Attributes["type"].Value, out qt);
                monster.Breed.AddQuip(qt, quip.InnerText);
            }

            XmlNode flags = monsterXml.SelectSingleNode("/Monster/Flags");

            foreach (XmlNode flag in flags.ChildNodes)
            {
                monster.Breed.Flags.Add(flag.InnerText);
            }

            XmlNode languages = monsterXml.SelectSingleNode("/Monster/Languages");

            foreach (XmlNode language in languages.ChildNodes)
            {
                monster.Languages.Add(language.InnerText, int.Parse(language.Attributes["weight"].Value));
            }

            XmlNode naturalAttacks = monsterXml.SelectSingleNode("/Monster/NaturalAttacks");

            foreach (XmlNode attack in naturalAttacks)
            {
                int amount = int.Parse(attack.Attributes["amount"].Value);
                for (int i = 0; i < amount; i++)
                {
                    BodyPartType slot = (BodyPartType)Enum.Parse(typeof(BodyPartType), attack["Slot"].InnerText);
                    int atkBonus = int.Parse(attack["AttackBonus"].InnerText);
                    string dmgDice = attack["DamageDice"].InnerText;
                    int range = int.Parse(attack["Range"].InnerText);
                    Attack atk = new Attack(atkBonus, dmgDice, range)
                    {
                        Name = attack.LocalName
                    };
                    monster.Body.AddNaturalAttackToLimb(slot, atk);
                }
            }

            return monster;
        }

        public MonsterFactory()
        {

        }
    }
}
