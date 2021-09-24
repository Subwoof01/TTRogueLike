using RogueLike.Systems.Equipment;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace RogueLike.Systems.Items
{
    public class ItemDataBase
    {
        public List<string> ActiveItems { get; private set; }

        public Item Get(string name)
        {
            string path = $@"{Environment.CurrentDirectory}\Data\Items\Items.xml";

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            XmlDocument itemsXml = new XmlDocument();
            itemsXml.Load(path);

            XmlNode item = itemsXml.SelectSingleNode($"/Items/{name}");

            XmlNode fgColour = item["ForegroundColour"];
            Color fg = new Color(
                    int.Parse(fgColour.ChildNodes.Item(0).InnerText),
                    int.Parse(fgColour.ChildNodes.Item(1).InnerText),
                    int.Parse(fgColour.ChildNodes.Item(2).InnerText)
                );

            XmlNode bgColour = item["BackgroundColour"];
            Color bg = new Color(
                    int.Parse(bgColour.ChildNodes.Item(0).InnerText),
                    int.Parse(bgColour.ChildNodes.Item(1).InnerText),
                    int.Parse(bgColour.ChildNodes.Item(2).InnerText)
                );

            ColoredGlyph glyph = new ColoredGlyph(
                    fg,
                    bg,
                    item["Glyph"].InnerText.ToCharArray()[0]
                );

            Item newItem = new Item(glyph, 2)
            {
                Name = name,
                Slot = (EquipSlot)Enum.Parse(typeof(EquipSlot), item["Slot"].InnerText),
                Melee = CreateAttack(item["Attack"]),
                SlotsNeeded = int.Parse(item["SlotsRequired"].InnerText)
            };

            return newItem;
        }

        private Attack CreateAttack(XmlNode attack)
        {
            if (attack != null)
            {
                Attack atk = new Attack(
                        int.Parse(attack["Bonus"].InnerText),
                        attack["Dice"].InnerText
                    );

                foreach (XmlNode damageType in attack["Types"].ChildNodes)
                {
                    Enum dType = null;
                    switch (damageType.Name)
                    {
                        case "Physical":
                            dType = (DamageType.Physical)Enum.Parse(typeof(DamageType.Physical), damageType.InnerText);
                            break;
                        case "Elemental":
                            dType = (DamageType.Elemental)Enum.Parse(typeof(DamageType.Elemental), damageType.InnerText);
                            break;
                        case "Energy":
                            dType = (DamageType.Energy)Enum.Parse(typeof(DamageType.Energy), damageType.InnerText);
                            break;
                    }

                    atk.DamageTypes.Add(new DamageType(dType));
                }

                return atk;
            }
            return null;
        }

        // TODO: Implement Defense and Use creators.

        private void FindActiveItems()
        {
            string[] lines = File.ReadAllLines($@"{Environment.CurrentDirectory}\Data\Items\ActiveItems.txt");

            foreach (string line in lines)
            {
                ActiveItems.Add(line);
            }
        }

        public ItemDataBase()
        {
            ActiveItems = new List<string>();
            FindActiveItems();
        }
    }
}
