﻿namespace GoFish {
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

            ComputerPlayers.AddRange(Players.Skip(1).Select(p => new PlayerViewModel(p)));

            StartGame = new DelegateCommand(StartGameCallback);
        }

        List<Player> Players { get; } = new List<Player>();

        public Player User => Players.First();
        public List<PlayerViewModel> ComputerPlayers { get; } = new List<PlayerViewModel>();
        public DelegateCommand StartGame { get; }

        int DealAmount { get; set; } = 5;
        List<Card> Cards { get; } = new List<Card>();

        private void StartGameCallback() {
            Deal();
        }

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