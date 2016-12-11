using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool HasNoItems => !this.items.Any();

        public static event EventHandler<ItemSpawnedEventArgs> ItemSpawned; 

        public Room()
        {
            this.doors = new Dictionary<Direction, Door>();
            this.items = new HashSet<Item>();
            this.cousinsInRoom = new HashSet<Cousin>();
        }

        public void ConnectRoom(Room room, Direction directionToRoom) // TODO: Make internal again
        {
            if(this.doors.ContainsKey(directionToRoom) && this.doors[directionToRoom].ToRoom != room)
            {
                throw new InvalidOperationException($"Door to {directionToRoom} direction has already been added");
            }

            if(this.doors.ContainsKey(directionToRoom) && (this.doors[directionToRoom].ToRoom == room))
            {
                return;
            }

            var door = new Door(room);
            this.doors.Add(directionToRoom, door);

            room.ConnectRoom(this, directionToRoom.Opposite);
        }

        public bool HasDoor(Direction direction)
        {
            return doors.ContainsKey(direction);
        }

        internal void MoveCousin(Cousin cousin, Direction direction)
        {
            if(!this.doors.ContainsKey(direction))
            {
                throw new InvalidOperationException($"Cannot move in {direction}");
            }

            this.GuardAgainstInvalidCousinOperations(cousin);

            this.cousinsInRoom.Remove(cousin);

            var door = this.doors[direction];

            door.MoveCousin(cousin);
        }

        internal void RemoveCousin(Cousin cousin)
        {
            this.GuardAgainstInvalidCousinOperations(cousin);

            this.cousinsInRoom.Remove(cousin);
        }

        internal void MoveInto(Cousin cousin)
        {
            this.cousinsInRoom.Add(cousin);
        }

        internal Item SpawnItem(ItemToSpawnSelector itemToSpawnSelector)
        {
            var item = itemToSpawnSelector.SpawnRandomItem();
            this.items.Add(item);

            if(ItemSpawned != null)
            {
                var itemSpawnedEventArgs = new ItemSpawnedEventArgs(item);
                ItemSpawned(this, itemSpawnedEventArgs);
            }

            return item;
        }

        internal void CousinPickUpItem(Cousin cousin, Item item)
        {
            this.GuardAgainstInvalidCousinOperations(cousin);

            if(!this.items.Contains(item))
            {
                throw new InvalidOperationException($"Cannot pick up {item.Name} as it's not in this room");
            }

            this.items.Remove(item);
        }

        internal void DropItem(Cousin cousin, Item item)
        {
            this.GuardAgainstInvalidCousinOperations(cousin);

            if(this.items.Contains(item))
            {
                throw new InvalidOperationException($"Cannot drop {item.Name} that's already in this room");
            }

            this.items.Add(item);
        }

        private void GuardAgainstInvalidCousinOperations(Cousin cousin)
        {
            if(!this.cousinsInRoom.Contains(cousin))
            {
                throw new InvalidOperationException($"{cousin.Name} is not in this room");
            }
        }
    }

    public class ItemSpawnedEventArgs : EventArgs
    {
        public Item Item { get; }

        public ItemSpawnedEventArgs(Item item)
        {
            this.Item = item;
        }
    }
}
