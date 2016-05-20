using System;
using System.Linq;
using System.Collections.Generic;

namespace Decks {
    class Deck {
        private List<Card> cards;
        static private Random random = new Random();

        public int Count { get { return cards.Count; } }

        public int CountOfRedCards { get { return GetCountOf(Suits.Diamonds) + GetCountOf(Suits.Hearts); } }

        public int CountOfBlackCards { get { return GetCountOf(Suits.Spades) + GetCountOf(Suits.Clubs); } }

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

        public Card Deal(int index) {
            Card CardToDeal = cards[index];
            cards.RemoveAt(index);
            return CardToDeal;
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

        public Deck PullOutValues(Values value) {
            Deck deckToReturn = new Deck(cards.Where(c => c.Value == value));
            cards = cards.Where(c => c.Value != value).ToList();
            return deckToReturn;
        }

        public int GetCountOf(Values value) => cards.Where(c => c.Value == value).Count();

        public int GetCountOf(Suits suit) => cards.Where(c => c.Suit == suit).Count();

        public Card Deal() => Deal(0);

        public void Add(Card cardToAdd) => cards.Add(cardToAdd);

        public IEnumerable<string> GetCardNames() => cards.Select(c => c.Name);

        public void Sort() => cards.Sort(new CardComparer() {SortCriteria = SortCardBy.ValueThenSuit} );

        public void Sort(SortCardBy sortCriteria) => cards.Sort(new CardComparer() {SortCriteria = sortCriteria} );

        public Card Peek(int cardNumber) => cards[cardNumber];

        public bool ContainsValue(Values value) => GetCountOf(value) != 0;
    }
}