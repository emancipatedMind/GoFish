namespace GoFish {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PlayingCards;
    using ToolkitNFW4.XAML;

    public class User : ISortingPlayer, IManualCardRequester {
        private IPlayer _player;

        Random randomizer = new Random();

        public User(IPlayer player) => _player = player;

        public Dictionary<Values, Player> Memory { get; } = new Dictionary<Values, Player>();
        public CustomCollection<Card> Cards => _player.Cards;
        public string Name { get => _player.Name; set => _player.Name = value; }

        public CardRequest MakeRequest() {

            var cardRequestedEA = new CardRequestedEventArgs();

            CardRequested?.Invoke(this, cardRequestedEA);

            return new CardRequest(this, cardRequestedEA.Player, cardRequestedEA.Card.Value);
        }

        public void SortCards() => SortRequested?.Invoke(this, EventArgs.Empty);

        public event EventHandler<CardRequestedEventArgs> CardRequested;
        public event EventHandler SortRequested;

    }
}