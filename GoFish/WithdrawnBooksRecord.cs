namespace GoFish {
    using PlayingCards;
    public struct WithdrawnBooksRecord {
        public IPlayer Player { get; }
        public Values Value { get; }

        public WithdrawnBooksRecord(IPlayer player, Values value) : this() {
            Player = player;
            Value = value;
        } 
    }
}