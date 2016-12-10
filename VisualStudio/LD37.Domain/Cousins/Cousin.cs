using LD37.Domain.Items;
using LD37.Domain.Movement;
using LD37.Domain.Rooms;

namespace LD37.Domain.Cousins
{
    public class Cousin
    {
        private readonly SpawnRoom spawnRoom;
        private Room currentRoom;
        private Item currentItem;

        public static Cousin Sas => new Cousin("Sas");
        public static Cousin Matt => new Cousin("Matt");
        public static Cousin Lida => new Cousin("Lida");
        public static Cousin Tharina => new Cousin("Tharina");
        public static Cousin Gallie => new Cousin("Gallie");
        public static Cousin Sias => new Cousin("Sias");
        public static Cousin Pieter => new Cousin("Pieter");

        public static Cousin[] all = { Sas, Matt, Lida, Tharina, Gallie, Sias, Pieter };

        public Room SpawnRoom => this.spawnRoom;
        public string Name { get; }

        private Cousin()
        {
            this.currentItem = Item.Default;
        }
        public Cousin(string name) : this()
        {
            this.Name = name;
            this.spawnRoom = new SpawnRoom(this);
            this.currentRoom = this.spawnRoom;
        }

        public void Move(Direction direction)
        {
            this.currentRoom.MoveCousin(this, direction);
        }

        internal void SetCurrentRoom(Room room)
        {
            room.MoveInto(this);
            this.currentRoom = room;
        }
    }
}
