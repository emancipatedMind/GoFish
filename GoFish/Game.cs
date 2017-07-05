namespace GoFish {
    using PlayingCards;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ToolkitNFW4.XAML;
    public class Game : EntityBase {

        Card? _selectedCard;
        ComputerPlayerViewModel _selectedPlayer;

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

        int DealAmount { get; set; } = 5;
        List<Card> Cards { get; } = new List<Card>();

        private void RequestCardCallback() {
            AskForCard(Players.First(), Players.Single(p => p == SelectedPlayer.Player), SelectedCard.Value);

            SelectedCard = null;
            SelectedPlayer = null;
        }

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

        private void AskForCard(Player askingPlayer, Player playerBeingAsked, Card card) { }

    }
}