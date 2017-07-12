namespace GoFish {
    public struct DeckWithdrawalRecord {

        public IPlayer Player { get; }
        public int CardCount { get; }

        public DeckWithdrawalRecord(IPlayer player, int cardCount) : this() {
            Player = player;
            CardCount = cardCount;
        }
    }
}