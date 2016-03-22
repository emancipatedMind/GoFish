using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Decks;

namespace GoFish {
    class Player {

        static Random random = new Random();

        private Deck cards = new Deck(new Card[] { });
        private string name;
        private TextBox textBoxOnForm;

        public string Name { get { return name; } }
        public int CardCount { get { return cards.Count; } }

        public Player(string name, TextBox textBoxOnForm) {
            this.name = name;
            this.textBoxOnForm = textBoxOnForm;
            this.textBoxOnForm.Text += name + " has just joined the game" + Environment.NewLine;
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
        }

        public Deck DoYouHaveAny(Values value) {
        }

        public void AskForACard(List<Player> players, int myIndex, Deck stock) {
        }

        public void AskForACard(List<Player> players, int myIndex, Deck stock, Values value) {
        }

        public void TakeCard(Card card) { cards.Add(card); }

        public IEnumerable<string> GetCardNames() { return cards.GetCardNames(); }

        public Card Peek(int cardNumber) { return cards.Peek(cardNumber); }

        public void SortHand() { cards.Sort(SortCardBy.Value); }
    }
}