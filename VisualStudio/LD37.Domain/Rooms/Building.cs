﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LD37.Domain.Cousins;
using LD37.Domain.Movement;

namespace LD37.Domain.Rooms
{
    public class Building
    {
        private readonly IDictionary<Point, Room> rooms;
        public List<Room> roomList { get; private set; }

        public Building(CousinsToAddToBuilding cousinsToAddToBuilding)
        {
            roomList = new List<Room>();
            var chance = 4;
            this.rooms = new Dictionary<Point, Room>();

            var spawnCoordinates = RoomCoordinates.SpawnCoordinates.ToList();

            var random = new Random();

            foreach(var coordinate in RoomCoordinates.Coordinates)
            {
                var randomNum = random.Next(0, chance + 1 - cousinsToAddToBuilding.CousinsLeftCount);
                var shouldSpawn = randomNum == 0;

                if(spawnCoordinates.Contains(coordinate))
                {
                    if(shouldSpawn)
                    {
                        var cousin = cousinsToAddToBuilding.GetRandomAndRemoveCousin();

                        this.rooms[coordinate] = cousin.SpawnRoom;
                    }

                    this.rooms[coordinate] = new Room();
                    chance--;
                    continue;
                }

                if(RoomCoordinates.BekerSpawnCoordinate == coordinate)
                {
                    this.rooms[coordinate] = new BekerRoom();
                }

                this.rooms[coordinate] = new Room();

                this.roomList.Add(this.rooms[coordinate]);
            }

            /*if(cousinsToAddToBuilding.CousinsLeftCount > 0)
            {
                throw new Exception("Not all cousins have been placed inside the building!");
            }*/

            foreach(var roomCoordinate in this.rooms.Keys)
            {
                var coordinatesAroundRoom = this.GetPointsAround(roomCoordinate);

                foreach(var coordinateToLink in coordinatesAroundRoom)
                {
                    var room = this.rooms[roomCoordinate];
                    var roomToLink = this.rooms[coordinateToLink.Key];

                    room.ConnectRoom(roomToLink, coordinateToLink.Value);
                }
            }
        }

        private Dictionary<Point, Direction> GetPointsAround(Point point)
        {
            var pointList = new Dictionary<Point, Direction>();

            var north = point + new Size(0, 1);
            var east = point + new Size(-1, 0);
            var south = point + new Size(0, -1);
            var west = point + new Size(1, 0);

            if(this.rooms.ContainsKey(north))
            {
                pointList.Add(north, Direction.North);
            }
            if(this.rooms.ContainsKey(east))
            {
                pointList.Add(east, Direction.East);
            }
            if(this.rooms.ContainsKey(south))
            {
                pointList.Add(south, Direction.South);
            }
            if(this.rooms.ContainsKey(west))
            {
                pointList.Add(west, Direction.West);
            }

            return pointList;
        }
    }

    internal static class RoomCoordinates
    {
        private const int edgeCoordinate = 3;

        public static IEnumerable<Point> Coordinates = coordinates;

        public static IEnumerable<Point> SpawnCoordinates = spawnCoordinates;

        public static Point BekerSpawnCoordinate = new Point(0, 0);

        private static IEnumerable<Point> coordinates
        {
            get
            {
                var points = new HashSet<Point>();

                for(var x = 0; x < edgeCoordinate; x++)
                {
                    for(var y = 0; y < edgeCoordinate; y++)
                    {
                        if(x == edgeCoordinate && y != 0)
                        {
                            continue;
                        }
                        if(x != 0 && y == edgeCoordinate)
                        {
                            continue;
                        }

                        if((x > 1 && y > 2) || (x > 2 && y > 1))
                        {
                            continue;
                        }

                        if(x == 0 && y != 0)
                        {
                            var point2 = new Point(x, -y);
                            points.Add(point2);
                        }
                        if(x != 0 && y == 0)
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