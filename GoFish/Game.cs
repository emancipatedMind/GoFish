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
        int _roundNumber = 0;

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

        List<IPlayer> Players { get; } = new List<IPlayer>();

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

            GameProgress = $"********** Round #{++_roundNumber} **********\r\n";

            var playerRequest = new CardRequest(Players.First(), Players.Single(p => p == SelectedPlayer.Player), SelectedCard.Value.Value);

            (CardRequestResult playerResponse, var deck) = MakeCardRequest(playerRequest, Cards);
            GameProgress += ConstructInfoStringForCardRequest(playerResponse, Cards.Count);

            var booksWithdrawnPlayer = WithdrawBooksFound(Players.First());
            string booksWithdrawnStringPlayer = ConstructInfoStringForBooksFound(Players.First(), booksWithdrawnPlayer);
            GameProgress += booksWithdrawnStringPlayer;
            Books += booksWithdrawnStringPlayer;

            int playerCountDecremented = Players.Count - 1;

            Players.Skip(1).ToList().ForEach(p => {
                if (p.Cards.Count() == 0) return;

                IPlayer requestee = Players.Where(pl => pl != p).Where(pl => pl.Cards.Count() != 0).ElementAt(randomizer.Next(playerCountDecremented));
                Values rank = p.Cards.ElementAt(randomizer.Next(p.Cards.Count)).Value;

                var request = new CardRequest(p, requestee, rank);
                (CardRequestResult response, var newDeck) = MakeCardRequest(request, deck);
                GameProgress += ConstructInfoStringForCardRequest(response, deck.Count());
                deck = newDeck.ToArray();

                var booksWithdrawn = WithdrawBooksFound(p);
                string booksWithdrawnString = ConstructInfoStringForBooksFound(p, booksWithdrawn);
                GameProgress += booksWithdrawnString;
                Books += booksWithdrawnString;
            });

            Cards.Clear();
            Cards.AddRange(deck);

            GameProgress += $"\r\nThe deck has {(Cards.Count == 1 ? "just 1 card" : $"{Cards.Count} cards") } left.";

            SelectedCard = null;
            SelectedPlayer = null;

            User.SortHand();
        }

        private void StartGameCallback() {
            GameIdle = false;
            Books = "";
            _roundNumber = 0;
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

        private (CardRequestResult result, IEnumerable<Card> newDeck) MakeCardRequest(CardRequest request, IEnumerable<Card> deck) {
            int cardCount = request.Requestee.Cards.Count(c => c.Value == request.Rank);

            if (cardCount == 0) {
                if (deck.Count() == 0) return (new CardRequestResult(request, cardCount), new Card[0]);
                request.Requester.Cards.AddRange(deck.Take(1));
                return (new CardRequestResult(request, cardCount), deck.Skip(1));
            }

            Card[] cardsToHandOver = request.Requestee.Cards.Where(c => c.Value == request.Rank).ToArray();
            Card[] cardsToKeep = request.Requestee.Cards.Where(c => c.Value != request.Rank).ToArray();
            request.Requestee.Cards.Clear();
            request.Requestee.Cards.AddRange(cardsToKeep);
            request.Requester.Cards.AddRange(cardsToHandOver);
            return (new CardRequestResult(request, cardCount), deck);
        }

        private IEnumerable<Values> WithdrawBooksFound(IPlayer player) {
            var booksFound = new List<Values>();
            var valuesInHand = player.Cards.Select(c => c.Value).Distinct().ToList();
            valuesInHand.ForEach(v => {
                if (player.Cards.Count(c => c.Value == v) == 4) {
                    var cardsToKeep = player.Cards.Where(c => c.Value != v).ToArray();
                    player.Cards.Clear();
                    player.Cards.AddRange(cardsToKeep);
                    booksFound.Add(v);
                }
            });
            return booksFound;
        }

        private string ConstructInfoStringForCardRequest(CardRequestResult result, int deckCountBeforeRequest) {
            var sb = new StringBuilder();
            string pluralRankText = Card.Plural(result.Rank);
            sb.AppendLine();
            sb.AppendLine($"{result.Requester.Name} says, \"Hey {result.Requestee.Name}... Do you have any {pluralRankText}?\"");
            if (result.ExchangeCount != 0) {
                sb.AppendLine($"{result.Requestee.Name} hands over {result.ExchangeCount} {(result.ExchangeCount == 1 ? result.Rank.ToString() : pluralRankText)}.");
            }
            else {
                sb.AppendLine($"{result.Requestee.Name} says, \"Go fish.\"");
                if (deckCountBeforeRequest != 0) sb.AppendLine($"{result.Requester.Name} takes one card from deck.");
            }
            return sb.ToString();
        }

        private string ConstructInfoStringForBooksFound(IPlayer player, IEnumerable<Values> books) {
            if (books.Count() == 0) return "";
            books.Select(b => $"{player.Name} lays down book of {Card.Plural(b)}.");
            return string.Join("\r\n", books.Select(b => $"{player.Name} lays down book of {Card.Plural(b)}.")) + "\r\n";
        }

    }
}