using System;
using System.Collections.Generic;
using LD37.Domain.Items;
using LD37.Domain.Movement;
using LD37.Domain.Rooms;

namespace LD37.Domain.Cousins
{
    public class Cousin
    {
        public static Cousin Sas = new Cousin("Sas");
        public static Cousin Matt = new Cousin("Matt");
        public static Cousin Lida = new Cousin("Lida");
        public static Cousin Tharina = new Cousin("Tharina");
        public static Cousin Gallie = new Cousin("Gallie");
        public static Cousin Sias = new Cousin("Sias");
        public static Cousin Pieter = new Cousin("Pieter");

        public static IEnumerable<Cousin> All = new List<Cousin> { Sas, Matt, Lida, Tharina, Gallie, Sias, Pieter };
        private readonly SpawnRoom spawnRoom;
        private readonly Fists fists;
        private Item currentItem;

        public EventHandler<RoomChangedEventArgs> RoomChanged;

        public Room SpawnRoom => this.spawnRoom;

        public Room CurrentRoom { get; private set; }

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
            this.CurrentRoom = this.spawnRoom;
        }

        public void Move(Direction direction)
        {
            var oldRoom = this.CurrentRoom;
            this.CurrentRoom.MoveCousin(this, direction);

            var args = new RoomChangedEventArgs(this, oldRoom, this.CurrentRoom, direction.Opposite);
            if(RoomChanged != null)
            {
                RoomChanged(this, args);
            }
        }

        public void PickUp(Item item)
        {
            if(this.currentItem == this.fists)
            {
                this.DropItem();
            }

            this.CurrentRoom.CousinPickUpItem(this, item);

            this.currentItem = item;
        }

        public void DropItem()
        {
            if(this.currentItem == this.fists)
            {
                return;
            }

            this.CurrentRoom.DropItem(this, this.currentItem);

            this.currentItem = this.fists;
        }

        internal void SetCurrentRoom(Room room)
        {
            room.MoveInto(this);
            this.CurrentRoom = room;
        }
    }

    public class RoomChangedEventArgs : EventArgs
    {
        public Cousin Cousin { get; }
        public Room OldRoom { get; }
        public Room NewRoom { get; }
        public Direction SpawnDirection { get; }

        public RoomChangedEventArgs(Cousin cousin, Room oldRoom, Room newRoom, Direction spawnDirection)
        {
            this.Cousin = cousin;
            this.OldRoom = oldRoom;
            this.SpawnDirection = spawnDirection;
            this.NewRoom = newRoom;
        }
    }
}
