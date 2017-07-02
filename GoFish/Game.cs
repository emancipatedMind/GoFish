namespace GoFish {
    using PlayingCards;
    using System.Collections.Generic;
    using System.Linq;
    using ToolkitNFW4.XAML;
    public class Game : EntityBase {

        public Game() {
            Players.Add(new Player("Peter"));
            Players.Add(new Player("Melvin"));
            Players.Add(new Player("John"));
            Players.Add(new Player("Raymond"));
        }

        List<Player> Players { get; } = new List<Player>();

        Player User => Players.First();
        IEnumerable<Player> ComputerPlayers => Players.Skip(1);

        int DealAmount { get; set; } = 5;
        List<Card> Cards { get; } = new List<Card>();

        private void Deal() {
            Cards.Clear();
            IEnumerable<Card> cards = Deck.NewShuffled.ToArray();
            for (int i = 0; i < Players.Count; i++) {
                Players[i].Cards.Clear();
                Players[i].Cards.AddRange(cards.Skip(i * DealAmount).Take(DealAmount));
            }
            Cards.AddRange(cards.Skip(Players.Count * DealAmount));
        }

    }
}