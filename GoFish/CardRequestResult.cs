namespace GoFish {
    using PlayingCards;
    public struct CardRequestResult {

        CardRequest _request;

        public CardRequestResult(CardRequest request, int exchangeCount) : this() {
            _request = request;
            ExchangeCount = exchangeCount;
        }

        public IPlayer Requester => _request.Requester;
        public IPlayer Requestee => _request.Requestee;
        public Values Rank => _request.Rank;
        public int ExchangeCount { get; }

    }
}