namespace GoFish {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PlayingCards;
    using ToolkitNFW4.XAML;

    public class ComputerPlayer : IAutomatedPlayer {
        private IPlayer _player;

        Random randomizer = new Random();

        public ComputerPlayer(IPlayer player) => _player = player;

        public Dictionary<Values, Player> Memory { get; } = new Dictionary<Values, Player>();
        public CustomCollection<Card> Cards => _player.Cards;
        public string Name { get => _player.Name; set => _player.Name = value; }

        public CardRequest MakeRequest(IEnumerable<IPlayer> players) {
            IPlayer[] otherPlayers = players.Where(pl => pl != this && pl.Cards.Count() != 0).ToArray();

            IPlayer requestee = otherPlayers.ElementAt(randomizer.Next(otherPlayers.Count()));
            Values rank = Cards.ElementAt(randomizer.Next(Cards.Count)).Value;

            return new CardRequest(this, requestee, rank);
        }

    }
}