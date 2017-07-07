namespace GoFish {
    using System.Collections.Generic;
    using PlayingCards;
    using ToolkitNFW4.XAML;

    public class ComputerPlayer : IPlayer {
        private IPlayer _player;

        public ComputerPlayer(IPlayer player) => _player = player;

        public Dictionary<Values, Player> Memory { get; } = new Dictionary<Values, Player>();
        public CustomCollection<Card> Cards => _player.Cards;
        public string Name { get => _player.Name; set => _player.Name = value; }

    }
}