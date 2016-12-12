using System;

namespace LD37.Domain.Items
{
    public class ItemToSpawnSelector
    {
        private readonly Func<Item>[] itemFactoryMethods;
        private readonly Random random;

        public ItemToSpawnSelector()
        {
            this.random = new Random();
        }
        public ItemToSpawnSelector(params Func<Item>[] itemFactoryMethods) : this()
        {
            this.itemFactoryMethods = itemFactoryMethods;
        }

        internal Item SpawnRandomItem()
        {
            var index = this.random.Next(0, this.itemFactoryMethods.Length);

            return this.itemFactoryMethods[index]();
        }
    }
}
