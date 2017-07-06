namespace GoFish {
    using PlayingCards;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ToolkitNFW4.XAML;
    public class Game : EntityBase {

        Random randomizer = new Random();
        Card? _selectedCard;
        ComputerPlayerViewModel _selectedPlayer;
        bool _gameIdle = true;
        string _gameProgress = "";
        string _books = "";

        public Game() {
            Players.Add(new Player("Peter"));
            Players.Add(new Player("Melvin"));
            Players.Add(new Player("John"));
            Players.Add(new Player("Raymond"));

            User = new UserViewModel(Players.First());
            ComputerPlayers.AddRange(Players.Skip(1).Select(p => new ComputerPlayerViewModel(p)));

            StartGame = new DelegateCommand(StartGameCallback);
            RequestCard = new DelegateCommand(RequestCardCallback);
        }

        List<Player> Players { get; } = new List<Player>();

        public UserViewModel User { get; }
        public List<ComputerPlayerViewModel> ComputerPlayers { get; } = new List<ComputerPlayerViewModel>();
        public DelegateCommand StartGame { get; }
        public DelegateCommand RequestCard { get; }

        public Card? SelectedCard {
            get => _selectedCard;
            set {
                _selectedCard = value;
                OnPropertyChanged(nameof(SelectedCard));
            }
        }

        public ComputerPlayerViewModel SelectedPlayer {
            get => _selectedPlayer;
            set {
                _selectedPlayer = value;
                OnPropertyChanged(nameof(SelectedPlayer));
            }
        }

        public bool GameIdle {
            get => _gameIdle;
            set {
                if (_gameIdle == value) return;
                _gameIdle = value;
                OnPropertyChanged(nameof(GameIdle));
            }
        }

        int DealAmount { get; set; } = 5;
        List<Card> Cards { get; } = new List<Card>();

        public string GameProgress {
            get => _gameProgress;
            set {
                if (_gameProgress == value) return;
                _gameProgress = value;
                OnPropertyChanged(nameof(GameProgress));
            }
        }

        public string Books {
            get => _books;
            set {
                if (_books == value) return;
                _books = value;
                OnPropertyChanged(nameof(Books));
            }
        }

        private void RequestCardCallback() {

            if (SelectedCard == null || SelectedPlayer == null) {
                FireNotifyEvent("Please select both a card to ask for, and the person to ask.");
                return;
            }

            GameProgress = "";

            AskForCard(Players.First(), Players.Single(p => p == SelectedPlayer.Player), SelectedCard.Value);

            int playerCountDecremented = Players.Count - 1;

            Players.Skip(1).ToList().ForEach(p => {
                AskForCard(p, Players.Where(pl => pl != p).ElementAt(randomizer.Next(playerCountDecremented)), p.Cards.ElementAt(randomizer.Next(p.Cards.Count)));
            });

            SelectedCard = null;
            SelectedPlayer = null;

            User.SortHand();
        }

        private void StartGameCallback() {
            GameIdle = false;
            Deal();
            User.SortHand();
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

        private void AskForCard(Player askingPlayer, Player playerBeingAsked, Card card) {
            var sb = new StringBuilder();
            string pluralText = Card.Plural(card.Value);
            sb.AppendLine($"{askingPlayer.Name} says, \"Hey {playerBeingAsked.Name}... Do you have any {pluralText}?\"");
            int cardCount = playerBeingAsked.Cards.Count(c => c.Value == card.Value);
            if (cardCount == 0) {
                sb.AppendLine($"{playerBeingAsked.Name} says, \"Go fish.\"");
                Card cardToTake = Cards.ElementAt(0);
                Cards.RemoveAt(0);
                askingPlayer.Cards.AddRange(new Card[] { cardToTake });
                sb.AppendLine($"{askingPlayer.Name} takes one card from deck.");
            }
            else {
                Card[] cardsToHandOver = playerBeingAsked.Cards.Where(c => c.Value == card.Value).ToArray();
                Card[] cardsToKeep = playerBeingAsked.Cards.Where(c => c.Value != card.Value).ToArray();
                playerBeingAsked.Cards.Clear();
                playerBeingAsked.Cards.AddRange(cardsToKeep);
                askingPlayer.Cards.AddRange(cardsToHandOver);
                sb.AppendLine($"{playerBeingAsked.Name} hands over {cardCount} {(cardCount == 1 ? card.Value.ToString() : pluralText)}.");
            }
            sb.AppendLine();
            GameProgress += sb.ToString();
        }

    }
}