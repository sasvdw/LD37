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
        private byte score;
        private int health = DEFAULT_HEALTH;

        public Room SpawnRoom => this.spawnRoom;

        public Room CurrentRoom { get; private set; }

        public Item CurrentItem { get; private set; }

        public string Name { get; }

        public event EventHandler<DiedEventArgs> Died;
        public event EventHandler<ItemDestroyedEventArgs> ItemDestroyed;
        public event EventHandler<ItemDroppedEventArgs> ItemDropped;
        public event EventHandler<ItemPickedUpEventArgs> ItemPickedUp;
        public event EventHandler<RespawnEventArgs> Respawned;
        public event EventHandler<RoomChangedEventArgs> RoomChanged;
        public event EventHandler<CousinScoreChangeEventArgs> ScoreChanged;
        public event EventHandler<CousinScoredEventArgs> CousinScored;

        private Cousin()
        {
            this.fists = new Fists();
            this.CurrentItem = this.fists;
            this.score = 0;
        }

        public Cousin(string name) : this()
        {
            this.Name = name;
            this.spawnRoom = new SpawnRoom(this);
            this.CurrentRoom = this.spawnRoom;
        }

        public bool Winner {
            get {
                return this.score == 3;
            }
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

            if(this.ItemPickedUp != null)
            {
                var itemPickedUpEventArgs = new ItemPickedUpEventArgs(item);
                this.ItemPickedUp(this, itemPickedUpEventArgs);
            }

            if(this.CurrentItem != this.fists)
            {
                this.DropItem();
            }

            this.CurrentItem = item;
        }

        public void DropItem()
        {
            var itemToDrop = this.CurrentItem;
            if(itemToDrop == this.fists)
            {
                return;
            }

            this.CurrentRoom.DropItem(this, this.CurrentItem);

            if (this.ItemDropped != null && (!this.IsInOwnSpawn || (this.IsInOwnSpawn && !(this.CurrentItem is Beker))))
            {
                var itemDroppedEventArgs = new ItemDroppedEventArgs(this.CurrentItem);
                this.ItemDropped(this, itemDroppedEventArgs);
            }

            if(this.IsInOwnSpawn && this.CurrentItem is Beker)
            {
                this.IncrementScore();
                if (this.CousinScored != null)
                {
                    var cousinScoredEventArgs = new CousinScoredEventArgs((Beker)this.CurrentItem, Winner);
                    this.CousinScored(this, cousinScoredEventArgs);
                }
            }

            this.CurrentItem = this.fists;
        }

        public bool IsInOwnSpawn => this.CurrentRoom == this.spawnRoom;

        public void Damage(int damage)
        {
            this.health -= damage;

            if(this.health > 0)
            {
                return;
            }

            this.DropItem();

            this.CurrentRoom.RemoveCousin(this);
            this.CurrentRoom = null;

            if(this.Died != null)
            {
                this.Died(this, new DiedEventArgs());
            }

            this.DecrementScore();
        }

        public void Respawn()
        {
            this.health = DEFAULT_HEALTH;
            SetCurrentRoom(this.spawnRoom);

            if(Respawned != null)
            {
                Respawned(this, new RespawnEventArgs());
            }
        }

        internal void IncrementScore()
        {
            this.score++;

            this.RaiseScoreChangedEvent();
        }

        private void RaiseScoreChangedEvent()
        {
            if(this.ScoreChanged != null)
            {
                var cousinScoreChangeEventArgs = new CousinScoreChangeEventArgs(this.score);
                this.ScoreChanged(this, cousinScoreChangeEventArgs);
            }
        }

        internal void SetCurrentRoom(Room room)
        {
            room.MoveInto(this);
            this.CurrentRoom = room;
        }

        internal void DestroyCurrentItem()
        {
            if(this.CurrentItem == this.fists)
            {
                return;
            }

            var itemToDestroy = this.CurrentItem;

            this.CurrentItem = this.fists;

            if(this.ItemDestroyed != null)
            {
                this.ItemDestroyed(this, new ItemDestroyedEventArgs(itemToDestroy));
            }
        }

        private void DecrementScore()
        {
            if(this.score == 0)
            {
                return;
            }

            this.score--;
            this.RaiseScoreChangedEvent();
        }
    }

    public class CousinScoreChangeEventArgs : EventArgs
    {
        public int NewScore { get; }

        public CousinScoreChangeEventArgs(int newScore)
        {
            this.NewScore = newScore;
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
        public Item Item { get; }

        public ItemPickedUpEventArgs(Item item)
        {
            this.Item = item;
        }
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

    public class DiedEventArgs : EventArgs {}

    public class RespawnEventArgs : EventArgs { }

    public class CousinScoredEventArgs : EventArgs
    {
        public Beker Beker { get; }
        public bool Won { get; }

        public CousinScoredEventArgs(Beker beker, bool won)
        {
            this.Beker = beker;
            this.Won = won;
        }
    }
}
