namespace GoFish {
    using System.Collections.Generic;
    using System.Linq;
    using PlayingCards;
    public class Player {

        public Player(string name) {
            Name = name;
            Cards = new Card[0];
        }

        public Player(string name, IEnumerable<Card> cards) {
            Name = name;
            Cards = cards;
        }

        public string Name { get; }
        public IEnumerable<Card> Cards { get; }

        public Player AcceptCards(IEnumerable<Card> cards) =>
            new Player(Name, cards);

        public Player ForfeitCards(IEnumerable<Card> cards) =>
            new Player(Name, Cards.Where(c => cards.Contains(c) == false));

        public Player ClearHand() =>
            new Player(Name);

        public Player ChangeName(string name) =>
            new Player(name, Cards);

    }
}