namespace GoFish {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ToolkitNFW4.XAML;
    using PlayingCards;
    public class Game : EntityBase {

        bool _gameInProgress = false;
        IEnumerable<Player> _players;

        public Game() {
            Player = new Player("Peter");
            ComputerPlayers = new Player[] {
                new Player("Joe"),
                new Player("Andrew"),
                new Player("Jason"),
            };

            GameStart = new DelegateCommand(GameStartCallback);
            ChangeName = new DelegateCommand(GameStartCallback);
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
        public DelegateCommand ChangeName { get; }

        private void GameStartCallback() {

            var allPlayers = new List<Player>();
            allPlayers.Add(Player);
            allPlayers.AddRange(ComputerPlayers); 

            GameInProgress = true;

            (var players, var cards) = StatelessModel.Deal(allPlayers, Deck.NewShuffled, DealAmount);
            Player = players.First();
            ComputerPlayers = players.Skip(1); 

        }

        private void ChangeNameCallback(object o) =>
            Player = Player.ChangeName(o.ToString());

    }
}