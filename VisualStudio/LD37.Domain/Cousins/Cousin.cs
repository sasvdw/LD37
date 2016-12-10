using LD37.Domain.Rooms;

namespace LD37.Domain.Cousins
{
    public class Cousin
    {
        private readonly string name;
        private readonly SpawnRoom spawnRoom;

        public static Cousin Sas => new Cousin("Sas");
        public static Cousin Matt => new Cousin("Matt");
        public static Cousin Lida => new Cousin("Lida");
        public static Cousin Tharina => new Cousin("Tharina");
        public static Cousin Gallie => new Cousin("Gallie");
        public static Cousin Sias => new Cousin("Sias");
        public static Cousin Pieter => new Cousin("Pieter");
        public Room SpawnRoom => this.spawnRoom;

        public Cousin(string name)
        {
            this.name = name;
            this.spawnRoom = new SpawnRoom(this);
        }
    }
}
