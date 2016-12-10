using System;
using System.Collections.Generic;

namespace LD37.Domain.Rooms
{
    public class Room
    {
        private readonly Dictionary<Direction, Door> doors;

        public Room()
        {
            this.doors = new Dictionary<Direction, Door>();
        }

        internal void ConnectRoom(Room room, Direction directionToRoom)
        {
            if(this.doors[directionToRoom].ToRoom == room)
            {
                return;
            }

            if(this.doors.ContainsKey(directionToRoom))
            {
                throw new InvalidOperationException($"Door to {directionToRoom} direction has already been added");
            }

            var door = new Door(room);
            this.doors.Add(directionToRoom, door);
            var otherRoomDoor = new Door(this);
            room.doors.Add(directionToRoom.Opposite, otherRoomDoor);
        }
    }
}
