using GoRogue.DiceNotation;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using RogueLike.Actors;
using RogueLike.Systems.Items;
using RogueLike.Tiles;
using SadConsole.Entities;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Console = SadConsole.Console;

namespace RogueLike.MapGeneration
{
    public class Map
    {
        private Generator _generator;
        private TransparencyMap _transparencyMap;
        private WalkabilityMap _walkabilityMap;

        public readonly int Width;
        public readonly int Height;

        public Tile[,] Tiles { get; private set; }

        public List<Actor> Actors { get; private set; }
        public List<Item> Items { get; private set; }

        public void Generate()
        {
            _generator.ConfigAndGenerateSafe(gen =>
            {
                gen.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps());
            });

            PlaceFloorsAndWalls();
            PlaceDoors();
            List<Rectangle> rooms = GetRooms();
            PlaceBars(rooms);
            PlacePlayerInRoom(rooms[0]);
            PlaceMonsters(rooms);
        }

        public void Generate(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);


            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    Tile tile = RogueLike.TileFactory.Get(lines[y][x].ToString());
                    tile.Position = new Point(x, y);
                    RogueLike.Renderer.Add(tile);
                    Tiles[x, y] = tile;
                }
            }
        }

        private void PlaceFloorsAndWalls()
        {
            ArrayView<bool> walkable = _generator.Context.GetFirst<ArrayView<bool>>("WallFloor");

            foreach (Point p in walkable.Positions())
            {
                if (!walkable[p])
                {
                    Tile wall = RogueLike.TileFactory.Get("Wall");
                    wall.Position = p;
                    RogueLike.Renderer.Add(wall);
                    Tiles[p.X, p.Y] = wall;
                }
                else
                {
                    Tile floor = RogueLike.TileFactory.Get("Floor");
                    floor.Position = p;
                    RogueLike.Renderer.Add(floor);
                    Tiles[p.X, p.Y] = floor;
                }
            }
        }

        private void PlaceDoors()
        {
            DoorList doors = _generator.Context.GetFirst<DoorList>("Doors");

            foreach (KeyValuePair<Rectangle, RoomDoors> kvp in doors.DoorsPerRoom)
            {
                foreach (Point p in kvp.Value.Doors)
                {
                    RogueLike.Renderer.Remove(Tiles[p.X, p.Y]);

                    Tile newTile = RogueLike.TileFactory.Get("Door");
                    newTile.Position = p;

                    RogueLike.Renderer.Add(newTile);
                    Tiles[p.X, p.Y] = newTile;
                }
            }
        }

        private void PlaceBars(List<Rectangle> rooms)
        {
            int barCount = Dice.Roll("1d4") - 1;

            foreach (Rectangle room in rooms)
            {
                for (int i = 0; i < barCount; i++)
                {
                    Point p = GetRandomWallPointInRoom(room);
                    Tile bars = RogueLike.TileFactory.Get("Bars");
                    bars.Position = p;
                    RogueLike.Renderer.Remove(Tiles[p.X, p.Y]);
                    RogueLike.Renderer.Add(bars);
                    Tiles[p.X, p.Y] = bars;
                }
            }
        }

        private List<Rectangle> GetRooms()
        {
            ItemList<Rectangle> roomsSteps = _generator.Context.GetFirst<ItemList<Rectangle>>("Rooms");
            List<Rectangle> rooms = new List<Rectangle>();
            foreach (ItemStepPair<Rectangle> room in roomsSteps)
            {
                rooms.Add(room.Item);
            }
            return rooms;
        }

        private void PlaceMonsters(List<Rectangle> spawnAreas)
        {
            foreach (Rectangle space in spawnAreas)
            {
                int amountOfMonsters = Dice.Roll("5d20") / 20;
                for (int i = 0; i < amountOfMonsters; i++)
                {
                    //Monster monster = new Monster(Color.Orange, Color.Black, 'k', 2)
                    //{
                    //    Name = "Kobold",
                    //    Position = GetRandomWalkablePointInRoom(space),
                    //    ArmourClass = Dice.Roll("1d12"),
                    //    Speed = 15,
                    //    FovRange = 8
                    //};
                    //monster.Breed = new Breed();

                    //monster.Level = Dice.Roll("1d4");
                    //monster.MaxHealth = Dice.Roll($"{monster.Level}d8");
                    //monster.Health = monster.MaxHealth;
                    //monster.Breed.Flags.Add("Dragonkin");
                    //monster.Breed.Flags.Add("Humanoid");

                    Monster monster = RogueLike.MonsterFactory.Get("Kobold");
                    monster.Position = GetRandomWalkablePointInRoom(space);
                    Actors.Add(monster);
                    RogueLike.Renderer.Add(monster);
                    RogueLike.SchedulingSystem.Add(monster);
                }
            }
        }
        
        private void PlacePlayerInRoom(Rectangle room)
        {
            if (RogueLike.Player == null)
                RogueLike.CreatePlayer();
            RogueLike.Player.Position = GetRandomWalkablePointInRoom(room);
            RogueLike.RootConsole.ViewPosition = ((int)(RogueLike.Player.Position.X - RogueLike.RootConsole.ViewWidth * 0.5), (int)(RogueLike.Player.Position.Y - RogueLike.RootConsole.ViewHeight * 0.5));
            Actors.Add(RogueLike.Player);
        }

        private Point GetRandomWalkablePointInRoom(Rectangle room)
        {
            List<Point> points = new List<Point>();
            foreach (Point p in room.Positions())
                if (Tiles[p.X, p.Y].IsWalkable && !Actors.Any(a => a.Position == p))
                    points.Add(p);

            return points[Dice.Roll($"1d{points.Count}") - 1];

        }
        
        private Point GetRandomWallPointInRoom(Rectangle room)
        {
            List<Point> points = new List<Point>();
            foreach (Point p in room.Expand(1, 1).PerimeterPositions())
            {
                if (Tiles[p.X, p.Y].Flags.Any(s => s == "Door"))
                    continue;
                points.Add(p);
            }

            return points[Dice.Roll($"1d{points.Count}") - 1];
        }

        public IGridView<bool> GetTransparency()
        {
            if (_transparencyMap == null)
                _transparencyMap = new TransparencyMap(Tiles);

            return _transparencyMap;
        }

        public IGridView<bool> GetWalkability()
        {
            if (_walkabilityMap == null)
                _walkabilityMap = new WalkabilityMap(Tiles);

            return _walkabilityMap;
        }

        public void SetCellTransparancy(Point point, bool isTranparent)
        {
            Tiles[point.X, point.Y].IsTransparent = isTranparent;

            if (_transparencyMap == null)
                _transparencyMap = new TransparencyMap(Tiles);

            _transparencyMap.UpdateTileTransparency(Tiles[point.X, point.Y]);
        }
        public void SetCellWalkability(Point point, bool isWalkable)
        {
            Tiles[point.X, point.Y].IsWalkable = isWalkable;

            if (_walkabilityMap == null)
                _walkabilityMap = new WalkabilityMap(Tiles);

            _walkabilityMap.UpdateTileWalkability(Tiles[point.X, point.Y]);
        }

        public Map(int width, int height)
        {
            _generator = new Generator(width, height);
            Tiles = new Tile[width, height];
            Actors = new List<Actor>();
            Items = new List<Item>();
            Width = width;
            Height = height;
        }
    }
}
