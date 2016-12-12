using LD37.Domain.Items;

namespace LD37.Domain.Rooms
{
    public sealed class BekerRoom : Room
    {
        public BekerRoom()
        {
            this.items.Add(Beker.Instance);
        }

        public void Move(Beker beker)
        {
            this.items.Add(beker);
        }
    }
}