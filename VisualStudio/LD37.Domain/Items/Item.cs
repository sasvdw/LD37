namespace LD37.Domain.Items
{
    public abstract class Item
    {
        public string Name { get; }

        protected Item(string name)
        {
            this.Name = name;
        }

        public static Item Default => new Fists();
    }
}
