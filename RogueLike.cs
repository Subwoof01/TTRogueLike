using System;
using System.Collections.Generic;
using GoRogue.FOV;
using GoRogue.GameFramework;
using RogueLike.MapGeneration;
using Map = RogueLike.MapGeneration.Map;
using RogueLike.Tiles;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using RogueLike.Systems;
using RogueLike.Actors;
using RogueLike.Actions;
using GoRogue.Pathing;
using System.IO;
using GoRogue.DiceNotation;
using System.Text.RegularExpressions;
using RogueLike.Extensions;
using RogueLike.Systems.Items;
using RogueLike.Systems.Equipment;
using SadConsole.UI;

namespace RogueLike
{
    public class RogueLike
    {
        public static ScreenObject RootContainer { get; private set; }
        public static Console RootConsole { get; private set; }
        public static Renderer Renderer { get; private set; }
        public static ItemDataBase ItemDatabase { get; private set; }
        public static TileFactory TileFactory { get; private set; }
        public static MonsterFactory MonsterFactory { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }
        public static ItemManager ItemManager { get; private set; }
        public static Map Map { get; private set; }
        public static Player Player { get; set; }
        public static AStar AStar { get; private set; }
        public static Dictionary<string, int> LanguageShift { get; private set; }

        public static int ScreenWidth = 160;
        public static int ScreenHeight = 50;

        private int _mapConsoleWidth = 106;
        private int _mapConsoleHeight = 32;

        private Console _messageConsole;
        private Console _playerStatsConsole;
        private ControlsConsole _inventoryConsole;
        private Console _currentShowingConsole;

        private bool _isPlayerTurn = true;

        private bool _drawMonsterLOS = false;
        private Console _fpsConsole;

        public RogueLike()
        {
            Settings.WindowTitle = "SadConsole Game";
            Settings.UseDefaultExtendedFont = true;
            Game.Create(ScreenWidth, ScreenHeight);
            Game.Instance.OnStart = Init;
            Game.Instance.FrameUpdate += Update;
            Game.Instance.FrameRender += Draw;
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        private void Init()
        {
            Settings.ResizeMode = Settings.WindowResizeOptions.Fit;

            // Create root empty screen to add child consoles to.
            RootContainer = new ScreenObject();
            Game.Instance.Screen = RootContainer;
            Game.Instance.DestroyDefaultStartingConsole();

            // Create main console.
            RootConsole = new Console(_mapConsoleWidth, _mapConsoleHeight)
            {
                Position = new Point(0, 0),
                DefaultBackground = Color.Black,
            };
            RootConsole.Clear();
            RootConsole.Cursor.PrintAppearanceMatchesHost = false;
            RootConsole.IsFocused = true;

            RootConsole.Resize(_mapConsoleWidth, _mapConsoleHeight, 128, 64, false);

            RootContainer.Children.Add(RootConsole);

            // Create message console.
            _messageConsole = new Console(_mapConsoleWidth, ScreenHeight - _mapConsoleHeight)
            {
                Position = new Point(0, _mapConsoleHeight),
                DefaultBackground = Color.Black
            };
            _messageConsole.Clear();
            _messageConsole.Cursor.PrintAppearanceMatchesHost = false;

            RootContainer.Children.Add(_messageConsole);

            // Create Player stats console.
            _playerStatsConsole = new Console(ScreenWidth - _mapConsoleWidth, ScreenHeight)
            {
                Position = new Point(_mapConsoleWidth, 0),
                DefaultBackground = Color.Black
            };
            _playerStatsConsole.Clear();
            _playerStatsConsole.Cursor.PrintAppearanceMatchesHost = false;

            RootContainer.Children.Add(_playerStatsConsole);

            // Create Inventory console.
            _inventoryConsole = new ControlsConsole(_mapConsoleWidth, _mapConsoleHeight)
            {
                Position = new Point(0, 0),
                DefaultBackground = Color.Black
            };
            _inventoryConsole.Clear();
            _inventoryConsole.Cursor.PrintAppearanceMatchesHost = false;

            RootContainer.Children.Add(_inventoryConsole);
            RootContainer.Children.MoveToBottom(_inventoryConsole);

            // Create FPS console.
            _fpsConsole = new Console(2, 2)
            {
                Position = new Point(ScreenWidth - 2, ScreenHeight - 1),
                DefaultBackground = Color.Black
            };
            _fpsConsole.Clear();
            _fpsConsole.Cursor.PrintAppearanceMatchesHost = false;

            RootContainer.Children.Add(_fpsConsole);

            _currentShowingConsole = RootConsole;

            // Create renderer for rendering entities.
            Renderer = new Renderer();
            RootConsole.SadComponents.Add(Renderer);

            TileFactory = new TileFactory();
            SchedulingSystem = new SchedulingSystem();
            MonsterFactory = new MonsterFactory();
            MessageLog = new MessageLog(_messageConsole);
            ItemDatabase = new ItemDataBase();
            ItemManager = new ItemManager(_inventoryConsole);

            ImportLanguages();

            Map = new Map(RootConsole.Width, RootConsole.Height);
            Map.Generate();

            // "D:\\_Projects\\RogueLike\\3DRogueLike\\Maps\\TestMap.txt"

            AStar = new AStar(Map.GetWalkability(), Distance.Manhattan);

            foreach (Actor a in Map.Actors)
            {
                if (a is Monster)
                {
                    (a as Monster).IsPlayerInFOV = new UpdateFovAction(a).Perform();
                    continue;
                }
                UpdateFovAction fov = new UpdateFovAction(a);
                fov.Perform();
            }
        }

        private void Draw(object sender, GameHost e)
        {
        }

        private void Update(object sender, GameHost e)
        {
            HandleGlobalKeyboardInput();

            if (_isPlayerTurn)
            {
                Action playerAction = Player.TakeTurn();
                if (playerAction != null)
                {
                    if (playerAction.Perform())
                    {
                        _isPlayerTurn = false;
                    }
                }
            }
            else
                TakeNPCTurns();

            DrawPlayerStats();

            if (_drawMonsterLOS)
            {
                foreach (Actor a in Map.Actors)
                {
                    if (a is Player)
                        continue;
                    foreach (Point p in a.Fov.CurrentFOV)
                    {
                        Map.Tiles[p.X, p.Y].Appearance.Background = Color.PaleVioletRed;
                    }
                    foreach (Point p in a.Fov.NewlyUnseen)
                    {
                        Map.Tiles[p.X, p.Y].Appearance.Background = Color.Black;
                    }
                }
            }

            double fps = 1 / (sender as Game).UpdateFrameDelta.TotalSeconds;
            _fpsConsole.Cursor
                .Move(new Point(0, 1))
                .SetPrintAppearance(Color.Yellow)
                .Print(fps.ToString("00"));
        }

        public static void CreatePlayer()
        {
            Player = new Player(Color.White, Color.Black, '@', 100)
            {
                Position = new Point(1, 1),
                Name = "Player",
                ArmourClass = 10,
                FovRange = 10,
                HearingRange = 10,
            };
            Player.Languages.Add("Common", 1);
            Renderer.Add(Player);
            Map.Actors.Add(Player);
            SchedulingSystem.Add(Player);
            Player.Equipment.EquipItemOnActor(ItemDatabase.Get("Battleaxe"));
            Player.Inventory.Add(ItemDatabase.Get("Battleaxe"));
            Player.Inventory.Add(ItemDatabase.Get("Battleaxe"));
            Player.Inventory.Add(ItemDatabase.Get("Battleaxe"));
            Player.Inventory.Add(ItemDatabase.Get("Shortsword"));
            Player.Inventory.Add(ItemDatabase.Get("Shortsword"));
        }

        private void ImportLanguages()
        {
            LanguageShift = new Dictionary<string, int>();

            string[] lines = File.ReadAllLines($@"{Environment.CurrentDirectory}\Data\Languages.txt");

            foreach (string s in lines)
            {
                LanguageShift.Add(s, Dice.Roll($"1d{lines.Length + 20}"));
            }
        }
        
        private void DrawPlayerStats()
        {
            int defaultX = 3;
            int x = 3, y = 2;
            _playerStatsConsole.Clear();

            _playerStatsConsole.Cursor
                .Move(x, y)
                .Print(Player.Name)
                .Move(x, ++y)
                .Move(x, ++y)
                .Print($"Total HP: {Player.TotalHealth}/{Player.TotalMaxHealth}")
                .Move(x, ++y)
                .Move(x, ++y)
                .Print("Body Part Health:")
                .Move(x, ++y);

            foreach (BodyPart bp in Player.Body.Parts)
            {
                if (bp.IsSevered)
                    bp.Appearance = new ColoredGlyph(Color.DarkRed);
                else if (bp.Health == 0)
                    bp.Appearance = new ColoredGlyph(Color.Red);
                else if (bp.Health == bp.MaxHealth)
                    bp.Appearance = new ColoredGlyph(Color.Green);
                else if (bp.Health >= bp.MaxHealth * 0.5)
                    bp.Appearance = new ColoredGlyph(Color.Yellow);
                else if (bp.Health > 0)
                    bp.Appearance = new ColoredGlyph(Color.Orange);
                else
                    bp.Appearance = new ColoredGlyph(Color.White);
            }

            string vitalACBonus = string.Format("{0:+0;-0;0}", Player.VitalACBonus);

            string headsLabel = $"Heads: (Vital {vitalACBonus} AC)";

            _playerStatsConsole.Cursor.PrintAppearance = Player.Body.Chest.Appearance;
            x += (int)(headsLabel.Length * 0.3);
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print($"Chest (Vital {vitalACBonus} AC): {Player.Body.Chest.Health}/{Player.Body.Chest.MaxHealth}");

            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);

            x = defaultX;

            int bodyPartLabelHeight = ++y;

            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print(headsLabel);

            foreach (BodyPart bp in Player.Body.Heads)
            {
                string text = "Head: ";
                if (bp.IsSevered)
                    text += "Severed";
                else
                    text += $"{bp.Health}/{bp.MaxHealth}";

                _playerStatsConsole.Cursor.PrintAppearance = bp.Appearance;
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print(text);
            }
            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);

            y = bodyPartLabelHeight;
            x += headsLabel.Length + 1;

            string necksLabel = $"Necks: (Vital {vitalACBonus} AC)";
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print(necksLabel);

            foreach (BodyPart bp in Player.Body.Necks)
            {
                string text = "Neck: ";
                if (bp.IsSevered)
                    text += "Severed";
                else
                    text += $"{bp.Health}/{bp.MaxHealth}";

                _playerStatsConsole.Cursor.PrintAppearance = bp.Appearance;
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print(text);
            }
            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);

            int highestBodyPartCount = 0;
            if (Player.Body.Heads.Count > highestBodyPartCount)
                highestBodyPartCount = Player.Body.Heads.Count;
            if (Player.Body.Necks.Count > highestBodyPartCount)
                highestBodyPartCount = Player.Body.Necks.Count;

            y += highestBodyPartCount;
            bodyPartLabelHeight = y;
            x = defaultX;

            string armsLabel = $"Arms: ";
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print(armsLabel);

            foreach (BodyPart bp in Player.Body.Arms)
            {
                string text = "Arm: ";
                if (bp.IsSevered)
                    text += "Severed";
                else
                    text += $"{bp.Health}/{bp.MaxHealth}";

                _playerStatsConsole.Cursor.PrintAppearance = bp.Appearance;
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print(text);
            }
            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);


            y = bodyPartLabelHeight;
            x += necksLabel.Length + 1;

            string handsLabel = $"Hands: ";
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print(handsLabel);

            foreach (BodyPart bp in Player.Body.Hands)
            {
                string text = "Hand: ";
                if (bp.IsSevered)
                    text += "Severed";
                else
                    text += $"{bp.Health}/{bp.MaxHealth}";

                _playerStatsConsole.Cursor.PrintAppearance = bp.Appearance;
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print(text);
            }
            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);

            highestBodyPartCount = 0;
            if (Player.Body.Arms.Count > highestBodyPartCount)
                highestBodyPartCount = Player.Body.Arms.Count;
            if (Player.Body.Hands.Count > highestBodyPartCount)
                highestBodyPartCount = Player.Body.Hands.Count;

            y += highestBodyPartCount;
            bodyPartLabelHeight = y;
            x = defaultX;

            string legsLabel = $"Legs: ";
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print(legsLabel);

            foreach (BodyPart bp in Player.Body.Legs)
            {
                string text = "Leg: ";
                if (bp.IsSevered)
                    text += "Severed";
                else
                    text += $"{bp.Health}/{bp.MaxHealth}";

                _playerStatsConsole.Cursor.PrintAppearance = bp.Appearance;
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print(text);
            }
            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);

            y = bodyPartLabelHeight;
            x += necksLabel.Length + 1;

            string feetLabel = $"Feet: ";
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Print(feetLabel);

            foreach (BodyPart bp in Player.Body.Feet)
            {
                string text = "Foot: ";
                if (bp.IsSevered)
                    text += "Severed";
                else
                    text += $"{bp.Health}/{bp.MaxHealth}";

                _playerStatsConsole.Cursor.PrintAppearance = bp.Appearance;
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print(text);
            }

            x = defaultX;

            _playerStatsConsole.Cursor.PrintAppearance = new ColoredGlyph(Color.White);
            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Move(x, ++y)
                .Print("Stats:")
                .Move(x, ++y);

            foreach (KeyValuePair<ActorStat, int> stat in Player.Stats)
            {
                string mod = string.Format("{0:+0;-0;0}", Player.GetStatModifier(stat.Key));

                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print($"{stat.Key.ToString().Substring(0, 3)}: {stat.Value} ({mod})");
            }

            _playerStatsConsole.Cursor
                .Move(x, ++y)
                .Move(x, ++y)
                .Print($"AC: {Player.ArmourClass}")
                .Move(x, ++y)
                .Move(x, ++y)
                .Print($"Speed: {Player.Speed}");

            _playerStatsConsole.DrawBox(
                    new Rectangle(0, 0, _playerStatsConsole.Width, _playerStatsConsole.Height),
                    ShapeParameters.CreateBorder(new ColoredGlyph(Color.DarkSlateGray, Color.DarkSlateGray))
                );
        }

        private void TakeNPCTurns()
        {

            IScheduleable scheduleable = SchedulingSystem.Get();
            if (scheduleable is Player)
            {
                _isPlayerTurn = true;
                SchedulingSystem.Add(Player);
            }
            else
            {
                Monster monster = scheduleable as Monster;
                if (monster != null)
                {
                    if (Distance.Chebyshev.Calculate(monster.Position, Player.Position) > monster.FovRange)
                    {
                        SchedulingSystem.Add(scheduleable);
                        return;
                    }
                    Action action = (scheduleable as Monster).TakeTurn();
                    if (action != null)
                        action.Perform();
                    SchedulingSystem.Add(scheduleable);
                }

                TakeNPCTurns();
            }
        }

        private void HandleGlobalKeyboardInput()
        {
            Keyboard keyboard = Game.Instance.Keyboard;
            if (!keyboard.HasKeysDown && !keyboard.HasKeysPressed)
                return;

            if (keyboard.IsKeyPressed(Keys.I))
            {
                if (_currentShowingConsole != _inventoryConsole)
                {
                    RootContainer.Children.MoveToTop(_inventoryConsole);
                    ItemManager.DrawPlayerInventoryConsole();
                    _currentShowingConsole = _inventoryConsole;
                }
                else
                {
                    RootContainer.Children.MoveToTop(RootConsole);
                    _currentShowingConsole = RootConsole;
                }

            }
            if (keyboard.IsKeyPressed(Keys.F5))
                Game.Instance.ToggleFullScreen();
            if (keyboard.IsKeyPressed(Keys.Escape))
                Environment.Exit(0);
            if (keyboard.IsKeyPressed(Keys.F6))
            {
                foreach (Tile tile in Map.Tiles)
                {
                    if (tile == null)
                        continue;
                    tile.IsExplored = true;
                }
            }
            if (keyboard.IsKeyPressed(Keys.PageUp))
            {
                MessageLog.Scroll(false);
            }

        }
    }
}
