using RogueLike.Systems.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Console;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace RogueLike.Systems.Equipment
{
    public class ItemManager
    {
        private ControlsConsole _playerInventoryConsole;
        private ListBox _items;

        public void DrawPlayerInventoryConsole()
        {
            int x = 1, y = 0;

            _playerInventoryConsole.Clear();
            _items.Items.Clear();

            _playerInventoryConsole.Cursor
                .Move(x, ++y)
                .Print("Inventory");

            int i = 0;
            foreach (Item item in RogueLike.Player.Inventory)
            {
                _items.Items.Add($"{i}. {item.Name}");
                i++;
            }
        }

        private void InitPlayerInventoryConsole()
        {
            _items = new ListBox(_playerInventoryConsole.Width, 3);
            _items.Position = new Point(1, 3);
            _playerInventoryConsole.Controls.Add(_items);
        }

        public ItemManager(ControlsConsole inventoryConsole)
        {
            _playerInventoryConsole = inventoryConsole;
            InitPlayerInventoryConsole();
        }
    }
}
