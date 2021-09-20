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

namespace RogueLike
{
    public class RogueLike
    {
        public static ScreenObject RootContainer { get; private set; }
        public static Console RootConsole { get; private set; }
        public static Renderer Renderer { get; private set; }
        public static TileFactory TileFactory { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }
        public static Map Map { get; private set; }
        public static Player Player { get; set; }
        public static AStar AStar { get; private set; }

        public static int ScreenWidth = 160;
        public static int ScreenHeight = 50;

        private int _mapConsoleWidth = 106;
        private int _mapConsoleHeight = 32;

        private Console _messageConsole;
        private Console _playerStatsConsole;

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
            Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

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

            // Create FPS console.
            _fpsConsole = new Console(2, 2)
            {
                Position = new Point(0, ScreenHeight - 1),
                DefaultBackground = Color.Black
            };
            _fpsConsole.Clear();
            _fpsConsole.Cursor.PrintAppearanceMatchesHost = false;

            RootContainer.Children.Add(_fpsConsole);


            // Create renderer for rendering entities.
            Renderer = new Renderer();
            RootConsole.SadComponents.Add(Renderer);

            TileFactory = new TileFactory();
            MessageLog = new MessageLog(_messageConsole);
            SchedulingSystem = new SchedulingSystem();

            Map = new Map(_mapConsoleWidth, _mapConsoleHeight);
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

        //private void CalculateFOV()
        //{
        //    _fov.Calculate(Player.Position, 10);

        //    foreach (Point p in _fov.CurrentFOV)
        //    {
        //        Map.Tiles[p.X, p.Y].CurrentlySeen = true;
        //        Map.Tiles[p.X, p.Y].IsExplored = true;
        //    }

        //    foreach (Point p in _fov.NewlyUnseen)
        //    {
        //        Map.Tiles[p.X, p.Y].CurrentlySeen = false;
        //    }

        //    foreach (Actor m in Map.Actors)
        //    {
        //        if (Map.Tiles[m.Position.X, m.Position.Y].CurrentlySeen)
        //        {
        //            m.IsVisible = true;
        //            continue;
        //        }
        //        m.IsVisible = false;
        //    }
        //}

        public static void CreatePlayer()
        {
            Player = new Player(Color.White, Color.Black, '@', 100)
            {
                Position = new Point(1, 1),
                Speed = 12,
                Name = "Player",
                MaxHealth = 20,
                Health = 20,
                ArmourClass = 10,
                FovRange = 10,
            };
            Renderer.Add(Player);
            Map.Actors.Add(Player);
            SchedulingSystem.Add(Player);
        }

        private void DrawPlayerStats()
        {
            int x = 1, y = 1;

            _playerStatsConsole.Cursor
                .Move(x, y)
                .Print(Player.Name)
                .Move(x, ++y)
                .Print($"HP: {Player.Health}/{Player.MaxHealth}");

            foreach (KeyValuePair<ActorStat, int> stat in Player.Stats)
            {
                _playerStatsConsole.Cursor
                    .Move(x, ++y)
                    .Print($"{stat.Key.ToString().Substring(0, 3)}: {stat.Value}");
            }
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
                    Action action = (scheduleable as Monster).TakeTurn();
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

        }
    }
}
