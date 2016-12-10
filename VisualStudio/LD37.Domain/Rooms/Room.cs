using System;
using System.Collections.Generic;
using LD37.Domain.Cousins;
using LD37.Domain.Items;
using LD37.Domain.Movement;

namespace LD37.Domain.Rooms
{
    public class Room
    {
        private readonly Dictionary<Direction, Door> doors;
        protected readonly HashSet<Item> items;
        private readonly HashSet<Cousin> cousinsInRoom;


        public Room()
        {
            this.doors = new Dictionary<Direction, Door>();
            this.items = new HashSet<Item>();
            this.cousinsInRoom = new HashSet<Cousin>();
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

        internal void MoveCousin(Cousin cousin, Direction direction)
        {
            if(!this.doors.ContainsKey(direction))
            {
                throw new InvalidOperationException($"Cannot move in {direction}");
            }

            if(!this.cousinsInRoom.Contains(cousin))
            {
                throw new InvalidOperationException($"{cousin.Name} is not in this room");
            }

            this.cousinsInRoom.Remove(cousin);

            var door = this.doors[direction];

            door.MoveCousin(cousin);
        }

        internal void MoveInto(Cousin cousin)
        {
            this.cousinsInRoom.Add(cousin);
        }
    }
}
