namespace GoFish {
    using PlayingCards;
    using ToolkitNFW4.XAML;
    public class Player : IPlayer {

        public Player(string name) => Name = name;

        public string Name { get; set; }
        public CustomCollection<Card> Cards { get; } = new CustomCollection<Card>();

    }
}