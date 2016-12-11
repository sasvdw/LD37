using System;
using LD37.Domain.Cousins;
using LD37.Domain.Items;

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

        internal override void DropItem(Cousin cousinDroppingItem, Item beker)
        {
            base.DropItem(cousinDroppingItem, beker);

            if(this.cousin != cousinDroppingItem || !(beker is Beker))
            {
                return;
            }

            this.items.Remove(beker);
        }
    }
}