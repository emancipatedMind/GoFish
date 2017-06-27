namespace GoFish {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class Player {
        private Random random = new Random();

        IEnumerable<Card> _cards;

        public string Name { get; private set; }
        public int CardCount => _cards.Count();

        public Player(string name) {
            Name = name;
        }

    }
}