namespace GoFish {
    using System.Collections.Generic;
    using PlayingCards;
    using System.Linq;
    public static class StatelessModel {

        public static (IEnumerable<Player> players, IEnumerable<Card> stock) Deal(IEnumerable<Player> players, IEnumerable<Card> deck, int dealAmount) =>
            (
                players.Zip(
                    Enumerable.Range(0, players.Count()).Select(x => x * dealAmount),
                    (p, v) => p.AcceptCards(deck.Skip(v).Take(dealAmount))
                ),
                deck.Skip(players.Count() * dealAmount)
            );

    }
}