namespace LD37.Domain.Items
{
    public abstract class Item
    {
        public string Name { get; }

        protected Item(string name)
        {
            this.Name = name;
        }
    }

    public sealed class PopGun : Item
    {
        internal PopGun() : base("PopGun") {}
    }

    public sealed class Bomb : Item
    {
        internal Bomb() : base("Bomb") {}
    }
}
