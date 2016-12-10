using System;
using System.Collections.Generic;
using LD37.Domain.Items;
using LD37.Domain.Movement;

namespace LD37.Domain.Rooms
{
    public class Room
    {
        private readonly Dictionary<Direction, Door> doors;
        protected readonly HashSet<Item> items;


        public Room()
        {
            this.doors = new Dictionary<Direction, Door>();
            this.items = new HashSet<Item>();
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

            room.ConnectRoom(this, directionToRoom.Opposite);
        }
    }
}
