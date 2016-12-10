namespace LD37.Domain.Rooms
{
    public class Door
    {
        internal Door(Room toRoom)
        {
            this.ToRoom = toRoom;
        }

        public Room ToRoom { get; }
    }
}
