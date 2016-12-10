using LD37.Domain.Items;
using LD37.Domain.Movement;
using LD37.Domain.Rooms;
using System;

namespace LD37.Domain.Cousins
{
    public class Cousin
    {
        private readonly SpawnRoom spawnRoom;
        private readonly Fists fists;
        private Room currentRoom;
        private Item currentItem;

        public EventHandler<RoomChangedEventArgs> RoomChanged;

        public static Cousin Sas => new Cousin("Sas");
        public static Cousin Matt => new Cousin("Matt");
        public static Cousin Lida => new Cousin("Lida");
        public static Cousin Tharina => new Cousin("Tharina");
        public static Cousin Gallie => new Cousin("Gallie");
        public static Cousin Sias => new Cousin("Sias");
        public static Cousin Pieter => new Cousin("Pieter");

        public static Cousin[] all = { Sas, Matt, Lida, Tharina, Gallie, Sias, Pieter };

        public Room SpawnRoom => this.spawnRoom;

        public Room CurrentRoom => this.currentRoom;

        public string Name { get; }

        private Cousin()
        {
            this.fists = new Fists();
            this.currentItem = this.fists;
        }

        public Cousin(string name) : this()
        {
            this.Name = name;
            this.spawnRoom = new SpawnRoom(this);
            this.currentRoom = this.spawnRoom;
        }

        public void Move(Direction direction)
        {
            RoomChangedEventArgs args = new RoomChangedEventArgs(this, this.currentRoom, direction.Opposite);

            this.currentRoom.MoveCousin(this, direction);

            if (RoomChanged != null) {
                RoomChanged(this, args);
            }
        }

        public void PickUp(Item item)
        {
            if(this.currentItem == this.fists)
            {
                this.DropItem();
            }

            this.currentRoom.CousinPickUpItem(this, item);

            this.currentItem = item;
        }

        public void DropItem()
        {
            if(this.currentItem == this.fists)
            {
                return;
            }

            this.currentRoom.DropItem(this, this.currentItem);

            this.currentItem = this.fists;
        }

        internal void SetCurrentRoom(Room room)
        {
            room.MoveInto(this);
            this.currentRoom = room;
        }
    }

    public class RoomChangedEventArgs : EventArgs {

        public Cousin Cousin { get; }
        public Room OldRoom { get; }
        public Direction SpawnDirection { get; }

        public RoomChangedEventArgs(Cousin cousin, Room oldRoom, Direction spawnDirection) {
            this.Cousin = cousin;
            this.OldRoom = oldRoom;
            this.SpawnDirection = spawnDirection;
        }
    }
}
