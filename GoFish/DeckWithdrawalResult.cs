namespace GoFish {
    public struct DeckWithdrawalResult {

        public IPlayer Player { get; }
        public int CardCount { get; }

        public DeckWithdrawalResult(IPlayer player, int cardCount) : this() {
            Player = player;
            CardCount = cardCount;
        }
    }
}