namespace GoFish {
    using PlayingCards;
    using ToolkitNFW4.XAML;
    public interface IPlayer {
        CustomCollection<Card> Cards { get; }
        string Name { get; set; }
    }
}