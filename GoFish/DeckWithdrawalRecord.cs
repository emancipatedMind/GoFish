namespace GoFish {
    using PlayingCards;
    using System.Collections.Generic;
    using System.Linq;
    public struct DeckWithdrawalRecord {

        public IPlayer Player { get; }
        public Card[] CardsWithDrawn { get; }

        public DeckWithdrawalRecord(IPlayer player, IEnumerable<Card> cards) : this() {
            Player = player;
            CardsWithDrawn = cards.ToArray();
        }
    }
}