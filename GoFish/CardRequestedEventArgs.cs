namespace GoFish {
    using System;
    using PlayingCards;
    public class CardRequestedEventArgs : EventArgs {
        public Card Card { get; set; }
        public IPlayer Player { get; set; }
    }
}