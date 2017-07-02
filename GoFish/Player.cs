namespace GoFish {
    using System.Collections.Generic;
    using PlayingCards;
    public class Player {

        public Player(string name) {
            Name = name;
        }

        public Player(string name, List<Card> cards) {
            Name = name;
            Cards.AddRange(cards);
        }

        public string Name { get; set; }
        public List<Card> Cards { get; } = new List<Card>();

    }
}