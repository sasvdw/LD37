﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LD37.Domain.Cousins
{
    public class CousinsToAddToBuilding
    {
        private readonly List<Cousin> cousins;
        private readonly Random random;
        private readonly int cousinSpawnChanceModifier;

        internal IEnumerable<Cousin> Cousins => this.cousins;
        internal int CousinsLeftCount => this.cousins.Count;

        public bool CanSpawnCousin
        {
            get
            {
                var randomNum = this.random.Next(0, this.cousinSpawnChanceModifier - this.cousins.Count + 1);

                return randomNum == 0;
            }
        }

        public CousinsToAddToBuilding()
        {
            this.random = new Random();
            this.cousinSpawnChanceModifier = 4;
        }

        public CousinsToAddToBuilding(IEnumerable<Cousin> cousins) : this()
        {
            var cousinsToAdd = cousins.ToList();

            if (cousinsToAdd.Count() > 4)
            {
                throw new ArgumentException("Cousins can't be more than 4", nameof(cousins));
            }

            this.cousins = cousinsToAdd;
        }

        internal Cousin GetRandomAndRemoveCousin()
        {
            var index = this.random.Next(0, this.cousins.Count);
            var cousin = this.cousins[index];
            this.cousins.Remove(cousin);
            return cousin;
        }
    }
}
