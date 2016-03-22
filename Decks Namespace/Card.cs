using System;

namespace Decks {
    class Card {
        public Suits Suit { get; set; }
        public Values Value { get; set; }

        public string Name {
            get { return Value.ToString() + " of " + Suit.ToString(); }
        } 

        public Card(Suits suit, Values value) {
            Suit = suit;
            Value = value;
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
