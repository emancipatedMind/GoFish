using System;

namespace Decks {
    class Card {

        private Suits suit;
        private Values value;

        public Suits Suit { get { return suit; } }
        public Values Value { get { return value; } }

        public string Name {
            get { return Value.ToString() + " of " + Suit.ToString(); }
        } 

        public Card(Suits suit, Values value) {
            this.suit = suit;
            this.value = value;
        }

        public override string ToString() {
            return Name;
        }

        static public string Plural(Values value) {
            if (value == Values.Six) return "Sixes";
            else return value.ToString() + "s";
        } 

    } 
} 
