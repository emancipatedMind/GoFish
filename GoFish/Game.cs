namespace GoFish {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ToolkitNFW4.XAML;
    public class Game : EntityBase {

        IEnumerable<Card> randomDeck;
        bool _gameInProgress = false;

        public Game() {
            var suits = (IEnumerable<Suits>)Enum.GetValues(typeof(Suits));
            var values = (Values[])Enum.GetValues(typeof(Values));
            var randomizer = new Random();
            Card[] newDeck = suits
                .SelectMany(s => values.Select(v => new Card(s, v)))
                .ToArray();

            randomDeck = newDeck
                .Select(c => new { Card = c, Order = randomizer.Next() })
                .OrderBy(x => x.Order)
                .Select(x => x.Card);

            Player = new Player("Peter");
            ComputerPlayers = new Player[] {
                new Player("Joe"),
                new Player("Andrew"),
                new Player("Jason"),
            };

            GameStart = new DelegateCommand(GameStartCallback);
        }

        public IEnumerable<Player> ComputerPlayers { get; private set; }
        public Player Player { get; private set; }
        public IEnumerable<Card> RemainingCards { get; private set; }
        public int DealAmount { get; set; } = 5;
        public bool GameInProgress {
            get => _gameInProgress;
            set {
                if (_gameInProgress == value) return;
                _gameInProgress = value;
                OnPropertyChanged(nameof(GameInProgress));
            }
        }
        public DelegateCommand GameStart { get; }

        void GameStartCallback(object o) {
            Player = Player.ChangeName(o.ToString());
            GameInProgress = true;
            Deal();
        }

        private void Deal() {
            var allPlayers = new List<Player>();
            allPlayers.Add(Player);
            allPlayers.AddRange(ComputerPlayers);

            int skipAmount = allPlayers.Count * DealAmount;

            IEnumerable<Card> shuffledDeck = randomDeck.ToArray();

            var dealedPlayers = allPlayers.Zip(
                Enumerable.Range(0, allPlayers.Count).Select(x => x * DealAmount),
                (p, v) => p.AcceptCards(shuffledDeck.Skip(v).Take(DealAmount))
            );

            RemainingCards = shuffledDeck.Skip(skipAmount);
            Player = dealedPlayers.First();
            ComputerPlayers = dealedPlayers.Skip(1);
        }

    }
}