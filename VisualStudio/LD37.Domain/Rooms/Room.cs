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
        protected readonly HashSet<Cousin> cousinsInRoom;
        protected readonly HashSet<Item> items;

        public IEnumerable<Cousin> Cousins => this.cousinsInRoom;

        public Room()
        {
            this.doors = new Dictionary<Direction, Door>();
            this.items = new HashSet<Item>();
            this.cousinsInRoom = new HashSet<Cousin>();
        }

        public void ConnectRoom(Room room, Direction directionToRoom) // TODO: Make internal again
        {
            if (this.doors.ContainsKey(directionToRoom) && this.doors[directionToRoom].ToRoom != room)
            {
                throw new InvalidOperationException($"Door to {directionToRoom} direction has already been added");
            }

            if (this.doors.ContainsKey(directionToRoom) && (this.doors[directionToRoom].ToRoom == room)) {
                return;
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

        internal void RemoveCousin(Cousin cousin) {
            if (!this.cousinsInRoom.Contains(cousin)) {
                throw new InvalidOperationException($"{cousin.Name} is not in this room");
            }

            this.cousinsInRoom.Remove(cousin);
        }

        internal void MoveInto(Cousin cousin)
        {
            this.cousinsInRoom.Add(cousin);
        }

        public bool HasDoor(Direction direction) {
            return doors.ContainsKey(direction);
        }

        internal Item SpawnItem(ItemToSpawnSelector itemToSpawnSelector)
        {
            var item = itemToSpawnSelector.SpawnRandomItem();
            this.items.Add(item);

            return item;
        }

        internal void CousinPickUpItem(Cousin cousin, Item item)
        {
            this.GuardAgainstCousinItemOperations(cousin, item);

            this.items.Remove(item);
        }

        internal void DropItem(Cousin cousin, Item item)
        {
            this.GuardAgainstCousinItemOperations(cousin, item);

            this.items.Add(item);
        }

        private void GuardAgainstCousinItemOperations(Cousin cousin, Item item)
        {
            if(!this.cousinsInRoom.Contains(cousin))
            {
                throw new InvalidOperationException($"{cousin.Name} is not in this room");
            }

            if(!this.items.Contains(item))
            {
                throw new InvalidOperationException($"Cannot pick up {item.Name} as it's not in this room");
            }
        }
    }
}
