using LD37.Domain.Cousins;

namespace LD37.Domain.Rooms
{
    public sealed class SpawnRoom : Room
    {
        private readonly Cousin cousin;

        internal SpawnRoom(Cousin cousin) : base()
        {
            this.cousin = cousin;
            this.cousinsInRoom.Add(this.cousin);
        }
    }
}