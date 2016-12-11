using System;
using System.Collections.Generic;
using LD37.Domain.Items;
using LD37.Domain.Movement;
using LD37.Domain.Rooms;

namespace LD37.Domain.Cousins
{
    public class Cousin
    {
        public const int DEFAULT_HEALTH = 3;

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
        private int health = DEFAULT_HEALTH;

        public Room SpawnRoom => this.spawnRoom;

        public Room CurrentRoom { get; private set; }

        public Item CurrentItem => this.currentItem;

        public string Name { get; }

        public event EventHandler<RoomChangedEventArgs> RoomChanged;
        public event EventHandler<DiedEventArgs> Died;
        public event EventHandler<RespawnEventArgs> Respawned;
        public event EventHandler<ItemDroppedEventArgs> ItemDropped;
        public event EventHandler<ItemPickedUpEventArgs> ItemPickedUp;
        public event EventHandler<ItemDestroyedEventArgs> ItemDestroyed;

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
            this.CurrentRoom.CousinPickUpItem(this, item);

            if (this.ItemPickedUp != null)
            {
                var itemPickedUpEventArgs = new ItemPickedUpEventArgs(item);
                this.ItemPickedUp(this, itemPickedUpEventArgs);
            }

            if (this.currentItem != this.fists)
            {
                this.DropItem();
            }

            this.currentItem = item;
        }

        public void DropItem()
        {
            var itemToDrop = this.currentItem;
            if(itemToDrop == this.fists)
            {
                return;
            }

            this.CurrentRoom.DropItem(this, itemToDrop);

            if (this.ItemDropped != null)
            {
                var itemDroppedEventArgs = new ItemDroppedEventArgs(itemToDrop);
                this.ItemDropped(this, itemDroppedEventArgs);
            }

            this.currentItem = this.fists;
        }

        internal void SetCurrentRoom(Room room)
        {
            room.MoveInto(this);
            this.CurrentRoom = room;
        }

        public void Damage(int damage) {
            this.health -= damage;

            if(this.health > 0)
            {
                return;
            }

            this.DropItem();

            this.CurrentRoom.RemoveCousin(this);
            this.CurrentRoom = null;

            if (this.Died != null) {
                this.Died(this, new DiedEventArgs());
            }
        }

        public void Respawn() {
            this.health = DEFAULT_HEALTH;
            SetCurrentRoom(this.spawnRoom);

            if (Respawned != null) {
                Respawned(this, new RespawnEventArgs());
            }
        }

        internal void DestroyCurrentItem()
        {
            if(this.currentItem == this.fists)
            {
                return;
            }

            var itemToDestroy = this.currentItem;

            this.currentItem = this.fists;

            if(this.ItemDestroyed != null)
            {
                this.ItemDestroyed(this, new ItemDestroyedEventArgs(itemToDestroy));
            }
        }
    }

    public class ItemDestroyedEventArgs : EventArgs
    {
        public Item Item { get; }

        public ItemDestroyedEventArgs(Item item)
        {
            this.Item = item;
        }
    }

    public class ItemPickedUpEventArgs : EventArgs
    {
        public ItemPickedUpEventArgs(Item item)
        {
            this.Item = item;
        }

        public Item Item { get; }
    }

    public class ItemDroppedEventArgs : EventArgs
    {
        public Item Item { get; }

        public ItemDroppedEventArgs(Item item)
        {
            this.Item = item;
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

    public class DiedEventArgs : EventArgs {
    }

    public class RespawnEventArgs : EventArgs {
    }
}
