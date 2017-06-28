namespace GoFish {
    using System;
    using ToolkitNFW4.XAML;
    using System.Collections.Generic;
    using System.Linq;
    public class Game : EntityBase {

        List<Card> _stock = new List<Card>();
        IEnumerable<Card> randomDeck;

        List<Player> _players = new List<Player>();
        int _cardsToDeal = 5;

        public Game() {
            var suits = (IEnumerable<Suits>) Enum.GetValues(typeof(Suits));
            var values = (Values[]) Enum.GetValues(typeof(Values));
            var randomizer = new Random();

            randomDeck = suits
                .SelectMany(s => values.Select(v => new Card(s, v)))
                .Select(c => new { Card = c, Order = randomizer.Next() })
                .OrderBy(x => x.Order)
                .Select(x => x.Card);

        }

        void Deal() {
            _stock.Clear();
            Stack<Card> newDeck = new Stack<Card>(randomDeck);
            _players.ForEach(p => p.ForfeitCards());

            int length = _cardsToDeal * _players.Count;

            for (int i = 0; i < length; i++) {
                int playerIndex = i % _players.Count;
                _players[playerIndex].AcceptCard(newDeck.Pop());
            }
            _stock.AddRange(newDeck);
        }

    }
}