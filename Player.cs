using Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFish {
    class Player {

        private Deck cards = new Deck(new Card[] { });
        private Random random;
        private string name;
        private Game mainGame;

        public string Name { get { return name; } }
        public int CardCount { get { return cards.Count; } }

        public Player(string name, Random random, Game mainGame) {
            this.name = name;
            this.random = random;
            this.mainGame = mainGame;

            mainGame.AddProgress(Name + " has just joined the game");
        }

        public IEnumerable<Values> PullOutBooks() {
            List<Values> books = new List<Values>();
            foreach(Values value in Enum.GetValues(typeof(Values))) {
                int howMany = 0;
                for (int card = 0; card < cards.Count; card++)
                    if (cards.Peek(card).Value == value) howMany++;
                if (howMany == 4) {
                    books.Add(value);
                    cards.PullOutValues(value);
                }
            }
            return books;
        }

        public Values GetRandomValue() {
            return cards.Peek(random.Next(cards.Count)).Value;
        }

        public Deck DoYouHaveAny(Values value) {
            Deck deckOfValues = cards.PullOutValues(value);
            mainGame.AddProgress(String.Format("{0} has {1} {2}", Name, deckOfValues.Count, Card.Plural(value)));
            return deckOfValues;
        }

        public void AskForACard(List<Player> players, int myIndex, Deck stock) {
            if (stock.Count > 0) {
                if (cards.Count == 0) cards.Add(stock.Deal());
                Values randomValue = GetRandomValue();
                AskForACard(players, myIndex, stock, randomValue);
                if (stock.Count > 0 && players[0].CardCount == 0) players[0].cards.Add(stock.Deal());
            }
        }

        public void AskForACard(List<Player> players, int myIndex, Deck stock, Values value) {
            mainGame.AddProgress(Name + " asks if anyone has a " + value);
            int totalCardsGiven = 0;
            for (int i = 0; i < players.Count; i++) {
                if (i != myIndex) {
                    Player player = players[i];
                    Deck CardsGiven = player.DoYouHaveAny(value);
                    totalCardsGiven += CardsGiven.Count;
                    while (CardsGiven.Count > 0) cards.Add(CardsGiven.Deal());
                }
            }
            if (totalCardsGiven == 0 && stock.Count > 0) {
                mainGame.AddProgress(Name + " must draw from the stock.");
                cards.Add(stock.Deal());
            }
        }

        public void TakeCard(Card card) { cards.Add(card); }

        public IEnumerable<string> GetCardNames() { return cards.GetCardNames(); }

        public Card Peek(int cardNumber) { return cards.Peek(cardNumber); }

        public void SortHand() { cards.Sort(SortCardBy.Value); }
    }
}
