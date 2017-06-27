namespace GoFish {
    using System;
    [Serializable]
    public struct Card {

        public Card(Suits suit, Values value) : this() {
            Suit = suit;
            Value = value;
        }

        public Suits Suit { get; private set; }
        public Values Value { get; private set; }
        public string Name => Value.ToString() + " of " + Suit.ToString();

        public override string ToString() => Name;
        public override bool Equals(object obj) => obj is Card && ((Card)obj) == this;
        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(Card a, Card b) => a.Suit == b.Suit && a.Value == b.Value;
        public static bool operator !=(Card a, Card b) => !(a == b);
        static public string Plural(Values value) {
            if (value == Values.Six) return "Sixes";
            else return value.ToString() + "s";
        }

    }
}