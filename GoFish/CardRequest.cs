namespace GoFish {
    using PlayingCards;
    public struct CardRequest {

        public CardRequest(IPlayer requester, IPlayer requestee, Values rank) : this() {
            Requester = requester;
            Requestee = requestee;
            Rank = rank;
        }

        public IPlayer Requester { get; }
        public IPlayer Requestee { get; }
        public Values Rank { get; }

    }
}