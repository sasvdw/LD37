using System.Collections.Generic;
using System.Drawing;

namespace LD37.Domain.Rooms
{
    public class Building
    {
        private readonly IDictionary<Point, Room> rooms;

        public Building()
        {
            this.rooms = new Dictionary<Point, Room>();

            foreach(var coordinate in RoomCoordinates.GetCoordinates)
            {
                this.rooms[coordinate] = new Room();
            }

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

    internal class RoomCoordinates
    {
        private readonly HashSet<Point> points;

        private RoomCoordinates()
        {
            this.points = new HashSet<Point>();

            for(var x = 0; x < 3; x++)
            {
                for(var y = 0 ; y < 3; y++)
                {
                    if(x == 3 && y != 0)
                    {
                        continue;
                    }
                    if(x != 0 && y != 0)
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
                        this.points.Add(point2);
                    }
                    if(x != 0 && y == 0)
                    {
                        var point3 = new Point(-x, y);
                        this.points.Add(point3);
                    }
                    if(x != 0 && y != 0)
                    {
                        var point4 = new Point(-x, -y);
                        this.points.Add(point4);
                    }


                    var point1 = new Point(x, y);
                    this.points.Add(point1);
                }
            }
        }

        private static readonly RoomCoordinates roomCoordinates = new RoomCoordinates();

        public static IEnumerable<Point> GetCoordinates => roomCoordinates.points;
    }
}
