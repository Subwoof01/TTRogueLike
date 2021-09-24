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
using RogueLike.Actors;

namespace RogueLike.Systems.Equipment
{
    public class ItemManager
    {
        private ControlsConsole _playerInventoryConsole;
        private ControlsConsole _playerEquipmentConsole;
        private ListBox _items;

        public void DrawPlayerInventoryConsole()
        {
            int x = 1, y = 0;

            _playerInventoryConsole.Clear();

            _playerInventoryConsole.Cursor
                .Move(x, ++y)
                .Print("Inventory");

            UpdatePlayerInventoryList();
        }

        public void DrawEquipmentConsole()
        {
        }

        public void UpdatePlayerInventoryList()
        {
            _items.Items.Clear();

            int i = 0;
            foreach (Item item in RogueLike.Player.Inventory)
            {
                _items.Items.Add($"{i}. {item.Name}");
                i++;
            }
        }

        private void InitPlayerEquipmentConsole()
        {
            _playerEquipmentConsole.Cursor
                .Move(1, 1)
                .Print("Equipment");
        }

        private void InitPlayerInventoryConsole()
        {
            _items = new ListBox(_playerInventoryConsole.Width, _playerInventoryConsole.Height - 3);
            _items.Position = (1, 3);
            _items.MouseButtonClicked += Clicked;
            _playerInventoryConsole.Controls.Add(_items);

            Button dropButton = new Button(6);
            dropButton.Position = (_playerInventoryConsole.Width - 7, _playerInventoryConsole.Height - 2);
            dropButton.Click += _OnDropButtonClicked;
            dropButton.Text = "Drop";
            _playerInventoryConsole.Controls.Add(dropButton);
        }

        private void _OnDropButtonClicked(object sender, EventArgs e)
        {
            DropItem(RogueLike.Player, GetCurrentlySelectedItem());
        }

        private void Clicked(object sender, ControlBase.ControlMouseState e)
        {
            RogueLike.MessageLog.PrintLine(_items.SelectedIndex.ToString());
        }

        public void DropItem(Actor actor, Item item)
        {
            RogueLike.Map.Tiles[actor.Position.X, actor.Position.Y].Items.Add(item);
            actor.Inventory.Remove(item);
            RogueLike.Map.Tiles[actor.Position.X, actor.Position.Y].UpdateAppearance();
            UpdatePlayerInventoryList();
        }

        public void PickUpItem(Actor actor, Item item)
        {
            actor.Inventory.Add(item);
            RogueLike.Map.Tiles[actor.Position.X, actor.Position.Y].Items.Remove(item);
            RogueLike.Map.Tiles[actor.Position.X, actor.Position.Y].UpdateAppearance();
            UpdatePlayerInventoryList();
        }

        public int GetCurrentlySelectedItemIndex()
        {
            return _items.SelectedIndex;
        }

        public Item GetCurrentlySelectedItem()
        {
            return RogueLike.Player.Inventory[_items.SelectedIndex];
        }

        public ItemManager(ControlsConsole inventoryConsole, ControlsConsole equipmentConsole)
        {
            _playerInventoryConsole = inventoryConsole;
            _playerEquipmentConsole = equipmentConsole;
            InitPlayerInventoryConsole();
            InitPlayerEquipmentConsole();
        }
    }
}
