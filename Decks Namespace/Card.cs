using System;

namespace Decks {
    class Card {

        private Suits _suit;
        private Values _value;

        public Suits Suit { get { return _suit; } }
        public Values Value { get { return _value; } }
        public string Name { get { return Value.ToString() + " of " + Suit.ToString(); } } 

        public Card(Suits suit, Values value) {
            _suit = suit;
            _value = value;
        }

        public override string ToString() => Name;

        static public string Plural(Values value) {
            if (value == Values.Six) return "Sixes";
            else return value.ToString() + "s";
        } 

    } 
} 
