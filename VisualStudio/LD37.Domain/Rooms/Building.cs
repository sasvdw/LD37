using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LD37.Domain.Cousins;
using LD37.Domain.Items;
using LD37.Domain.Movement;

namespace LD37.Domain.Rooms
{
    public class Building
    {
        private readonly Room[] itemSpawnableRooms;
        private readonly ItemToSpawnSelector itemToSpawnSelector;
        private readonly Random random;
        private readonly HashSet<Item> items;
        private readonly HashSet<Cousin> cousins;
        public IDictionary<Point, Room> RoomsWithPoints { get; }

        public IEnumerable<Room> Rooms => this.RoomsWithPoints.Select(x => x.Value);

        public IEnumerable<Item> Items => this.items;

        public Building()
        {
            this.RoomsWithPoints = new Dictionary<Point, Room>();
            this.cousins = new HashSet<Cousin>();
            this.items = new HashSet<Item> { Beker.Instance };
            this.random = new Random();
        }

        public Building(
            IEnumerable<Cousin> cousinsToAdd,
            ItemToSpawnSelector itemToSpawnSelector)
            : this()
        {
            foreach(var cousin in cousinsToAdd)
            {
                this.cousins.Add(cousin);
            }
            var cousinsToAddToBuilding = new CousinsToAddToBuilding(cousinsToAdd);
            this.itemToSpawnSelector = itemToSpawnSelector;
            var chance = 4;

            var spawnCoordinates = RoomCoordinates.SpawnCoordinates.ToList();

            var coordinates = RoomCoordinates.Coordinates.ToList();
            foreach(var coordinate in coordinates)
            {
                if(cousinsToAddToBuilding.CousinsLeftCount > 0 && spawnCoordinates.Contains(coordinate))
                {
                    var randomNum = this.random.Next(0, chance + 1 - cousinsToAddToBuilding.CousinsLeftCount);
                    var shouldSpawn = randomNum == 0;
                    chance--;

                    if(shouldSpawn)
                    {
                        var cousin = cousinsToAddToBuilding.GetRandomAndRemoveCousin();

                        this.RoomsWithPoints[coordinate] = cousin.SpawnRoom;
                        continue;
                    }

                    this.RoomsWithPoints[coordinate] = new Room();
                    continue;
                }

                if(RoomCoordinates.BekerSpawnCoordinate == coordinate)
                {
                    this.RoomsWithPoints[coordinate] = new BekerRoom();
                    continue;
                }

                this.RoomsWithPoints[coordinate] = new Room();
            }

            this.itemSpawnableRooms = this.RoomsWithPoints
                .Where(x => !(x.Value is SpawnRoom) && !(x.Value is BekerRoom))
                .Select(x => x.Value).ToArray();

            if(cousinsToAddToBuilding.CousinsLeftCount > 0)
            {
                throw new Exception("Not all cousins have been placed inside the building!");
            }

            foreach(var roomCoordinate in this.RoomsWithPoints.Keys)
            {
                var coordinatesAroundRoom = this.GetPointsAround(roomCoordinate);

                foreach(var coordinateToLink in coordinatesAroundRoom)
                {
                    var room = this.RoomsWithPoints[roomCoordinate];
                    var roomToLink = this.RoomsWithPoints[coordinateToLink.Key];

                    room.ConnectRoom(roomToLink, coordinateToLink.Value);
                }
            }
        }

        public void SpawnItem()
        {
            if(this.items.Count == 5)
            {
                return;
            }

            var emptyRooms = this.itemSpawnableRooms.Where(x => x.HasNoItems).ToArray();

            if(emptyRooms.Length == 0)
            {
                return;
            }

            var index = this.random.Next(0, emptyRooms.Length);

            var item = emptyRooms[index].SpawnItem(this.itemToSpawnSelector);

            this.items.Add(item);
        }

        private Dictionary<Point, Direction> GetPointsAround(Point point)
        {
            var pointList = new Dictionary<Point, Direction>();

            var north = point + new Size(0, 1);
            var east = point + new Size(1, 0);
            var south = point + new Size(0, -1);
            var west = point + new Size(-1, 0);

            if(this.RoomsWithPoints.ContainsKey(north))
            {
                pointList.Add(north, Direction.North);
            }
            if(this.RoomsWithPoints.ContainsKey(east))
            {
                pointList.Add(east, Direction.East);
            }
            if(this.RoomsWithPoints.ContainsKey(south))
            {
                pointList.Add(south, Direction.South);
            }
            if(this.RoomsWithPoints.ContainsKey(west))
            {
                pointList.Add(west, Direction.West);
            }

            return pointList;
        }

        public void Destroy(Item itemToDestroy)
        {
            if(!this.items.Contains(itemToDestroy))
            {
                throw new InvalidOperationException($"{itemToDestroy.Name} is not in building!");
            }

            this.items.Remove(itemToDestroy);

            var cousin = this.cousins.Single(x => x.CurrentItem == itemToDestroy);

            cousin.DestroyCurrentItem();
        }
    }

    internal static class RoomCoordinates
    {
        private const int edgeCoordinate = 2;

        public static IEnumerable<Point> Coordinates = coordinates;

        public static IEnumerable<Point> SpawnCoordinates = spawnCoordinates;

        public static Point BekerSpawnCoordinate = new Point(0, 0);

        private static IEnumerable<Point> coordinates
        {
            get
            {
                var points = new HashSet<Point>();

                for(var x = 0; x <= edgeCoordinate; x++)
                {
                    for(var y = 0; y <= edgeCoordinate - x; y++)
                    {
                        if(y != 0)
                        {
                            var point2 = new Point(x, -y);
                            points.Add(point2);
                        }
                        if(x != 0)
                        {
                            var point3 = new Point(-x, y);
                            points.Add(point3);
                        }
                        if(x != 0 && y != 0)
                        {
                            var point4 = new Point(-x, -y);
                            points.Add(point4);
                        }

                        var point1 = new Point(x, y);
                        points.Add(point1);
                    }
                }

                return points;
            }
        }

        private static IEnumerable<Point> spawnCoordinates
        {
            get
            {
                var points = new HashSet<Point>
                             {
                                 new Point(0, edgeCoordinate),
                                 new Point(0, -edgeCoordinate),
                                 new Point(edgeCoordinate, 0),
                                 new Point(-edgeCoordinate, 0)
                             };

                return points;
            }
        }
    }
}
