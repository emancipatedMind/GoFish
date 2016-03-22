using System;
using System.Collections.Generic;

namespace Decks {
    class Deck {
        private List<Card> cards;
        static private Random random = new Random();

        public int Count { get { return cards.Count; } }

        public Deck() {
            cards = new List<Card>();
            foreach(Suits suit in Enum.GetValues(typeof(Suits))) {
                foreach(Values value in Enum.GetValues(typeof(Values))) {
                    cards.Add(new Card(suit, value));
                }
            }
        }

        public Deck(IEnumerable<Card> initialCards) {
            cards = new List<Card>(initialCards);
        }

        public void Add(Card cardToAdd) {
            cards.Add(cardToAdd);
        }

        public Card Deal(int index) {
            Card CardToDeal = cards[index];
            cards.RemoveAt(index);
            return CardToDeal;
        }

        public Card Deal() {
            return Deal(0);
        }

        public void Shuffle() {
            List<Card> newCards = new List<Card>();
            while (cards.Count > 0) {
                int cardToMove = random.Next(cards.Count);
                newCards.Add(cards[cardToMove]);
                cards.RemoveAt(cardToMove);
            }
            cards = newCards;
        }

        public IEnumerable<string> GetCardNames() {
            string[] CardNames = new string[cards.Count];
            for ( int i = 0 ; i < cards.Count ; i++ ) CardNames[i] = cards[i].Name;
            return CardNames;
        }

        public void Sort() {
            cards.Sort(new CardComparer() {SortCriteria = SortCardBy.ValueThenSuit} );
        }

        public void Sort(SortCardBy sortCriteria) {
            cards.Sort(new CardComparer() {SortCriteria = sortCriteria} );
        }

        public Card Peek(int cardNumber) {
            return cards[cardNumber];
        }

        public bool ContainsValue(Values value) {
            foreach (Card card in cards)
                if (card.Value == value) return true;
            return false;
        }

        public Deck PullOutValues(Values value) {
            Deck deckToReturn = new Deck(new Card[] { } );
            for (int i = cards.Count - 1; i >= 0; i--)
                if (cards[i].Value == value) deckToReturn.Add(Deal(i));
            return deckToReturn;
        }
    }
}
