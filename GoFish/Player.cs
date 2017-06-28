namespace GoFish {
    using System.Collections.Generic;
    using System.Linq;
    public class Player {

        List<Card> _cards = new List<Card>();

        public string Name { get; private set; }
        public int CardCount => _cards.Count();

        public Player(string name) {
            Name = name;
        }

        public void AcceptCard(Card card) => _cards.Add(card);
        public void ForfeitCards() => _cards.Clear();

    }
}