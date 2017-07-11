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
            Players.Add(new ComputerPlayer(new Player("Melvin")));
            Players.Add(new ComputerPlayer(new Player("John")));
            Players.Add(new ComputerPlayer(new Player("Raymond")));

            User = new UserViewModel(Players.First());
            ComputerPlayers.AddRange(Players.Skip(1).Select(p => new ComputerPlayerViewModel(p)));

            StartGame = new DelegateCommand(StartGameCallback);
            PlayRound = new DelegateCommand(PlayRoundCallback);
        }

        List<IPlayer> Players { get; } = new List<IPlayer>();

        public UserViewModel User { get; }
        public List<ComputerPlayerViewModel> ComputerPlayers { get; } = new List<ComputerPlayerViewModel>();
        public DelegateCommand RequestCard { get; }
        public DelegateCommand StartGame { get; }
        public DelegateCommand PlayRound { get; }

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

        private void StartGameCallback() {
            GameIdle = false;
            Books = "";
            _roundNumber = 0;
            Deal();
            User.SortHand();
        }

        private bool PlayerCardNeeded =>
            PlayerStillInGame &&
                (SelectedCard == null || SelectedPlayer == null);

        private bool PlayerStillInGame =>
            Players.First().Cards.Count > 0;

        private void PlayRoundCallback() {

            var allStatuses = new List<(string GameProgressString, string BooksString, string CardsDealtString)>();

            int cardWithDrawnCount = 0;

            if (PlayerCardNeeded) {
                FireNotifyEvent("Please select both a card to ask for, and the person to ask.");
                return;
            }

            GameProgress = $"********** Round #{++_roundNumber} **********\r\n";

            if (PlayerStillInGame) {
                var playerRequest = new CardRequest(Players.First(), Players.Single(p => p == SelectedPlayer.Player), SelectedCard.Value.Value);

                var playerResponse = MakeCardRequest(playerRequest);

                IEnumerable<DeckWithdrawalResult> deckWithDrawalResults = CheckIfCardsNeedToBeDrawnFromDeck(playerResponse, Cards);

                try {
                    cardWithDrawnCount = deckWithDrawalResults.Select(r => r.CardCount).Aggregate((prev, next) => prev + next);
                }
                catch(InvalidOperationException) { }

                var booksWithdrawnPlayer = WithdrawBooksFound(Players.First());

                allStatuses.Add((
                    ConstructInfoStringForCardRequest(playerResponse),
                    ConstructInfoStringForBooksFound(Players.First(), booksWithdrawnPlayer),
                    ConstructInfoStringForCardsWithdrawnFromDeck(deckWithDrawalResults)
                ));
            }

            (var deck, var status) = AutomatedPlay(Players, Cards.Skip(cardWithDrawnCount));

            Cards.Clear();
            Cards.AddRange(deck);

            allStatuses.AddRange(status);

            allStatuses.ForEach(s => {
                GameProgress += s.GameProgressString;
                GameProgress += s.BooksString;
                GameProgress += s.CardsDealtString;
                Books += s.BooksString;
            });

            GameProgress += $"\r\nStock has {Cards.Count} card{(Cards.Count == 1 ? "" : "s")} remaining.";

            SelectedCard = null;
            SelectedPlayer = null;

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

        private (IEnumerable<Card> Deck, IEnumerable<(string GameProgressString, string BooksString, string CardsDealtString)> Status) AutomatedPlay(IEnumerable<IPlayer> players, IEnumerable<Card> deck) {

            IEnumerable<IPlayer> playersWithCards = players.Where(p => p.Cards.Count() != 0);
            List<IAutomatedPlayer> automatedPlayers = playersWithCards.OfType<IAutomatedPlayer>().ToList();

            var status = new List<(string, string, string)>();

            int skipAmount = 0;
            bool moreCardsToBeDealt = true;

            automatedPlayers.ForEach(p => {
                if (p.Cards.Count == 0) return;

                CardRequest request = p.MakeRequest(playersWithCards);
                var result = MakeCardRequest(request);

                var booksWithdrawn = WithdrawBooksFound(p);

                var deckWithDrawalResults = CheckIfCardsNeedToBeDrawnFromDeck(result, deck.Skip(skipAmount).ToArray());

                if (moreCardsToBeDealt) {
                // These two lines are different. They should have the name changed to avoid confusion.
                    if (request.Requestee.Cards.Count == 0) skipAmount += 5;
                    if (request.Requester.Cards.Count == 0) skipAmount += 5;
                    if (result.ExchangeCount == 0) skipAmount += 1;
                    moreCardsToBeDealt = skipAmount < deck.Count();
                }

                status.Add((
                    ConstructInfoStringForCardRequest(result),
                    ConstructInfoStringForBooksFound(p, booksWithdrawn),
                    ConstructInfoStringForCardsWithdrawnFromDeck(deckWithDrawalResults)
                ));

            });

            return (deck.Skip(skipAmount).ToArray(), status);
        }

        private IEnumerable<DeckWithdrawalResult> CheckIfCardsNeedToBeDrawnFromDeck(CardRequestResult request, IEnumerable<Card> deck) {
            if (deck.Count() == 0) return new DeckWithdrawalResult[0];

            var results = new List<DeckWithdrawalResult>();
            if (request.Requestee.Cards.Count == 0) {
                request.Requestee.Cards.AddRange(deck.Take(5));
                results.Add(new DeckWithdrawalResult(request.Requestee, 5));
            }

            if (request.ExchangeCount == 0) {
                request.Requester.Cards.AddRange(deck.Take(1));
                results.Add(new DeckWithdrawalResult(request.Requester, 1));
            }
            else if (request.Requester.Cards.Count == 0) {
                request.Requester.Cards.AddRange(deck.Take(5));
                results.Add(new DeckWithdrawalResult(request.Requester, 5));
            }
            return results;
        }

        private CardRequestResult MakeCardRequest(CardRequest request) {
            int cardCount = request.Requestee.Cards.Count(c => c.Value == request.Rank);

            if (cardCount != 0) {
                Card[] cardsToHandOver = request.Requestee.Cards.Where(c => c.Value == request.Rank).ToArray();
                Card[] cardsToKeep = request.Requestee.Cards.Where(c => c.Value != request.Rank).ToArray();
                request.Requestee.Cards.Clear();
                request.Requestee.Cards.AddRange(cardsToKeep);
                request.Requester.Cards.AddRange(cardsToHandOver);
            }

            return new CardRequestResult(request, cardCount);
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

        private string ConstructInfoStringForCardRequest(CardRequestResult result) {
            var sb = new StringBuilder();
            string pluralRankText = Card.Plural(result.Rank);
            sb.AppendLine();
            sb.AppendLine($"{result.Requester.Name} says, \"Hey {result.Requestee.Name}... Do you have any {pluralRankText}?\"");
            if (result.ExchangeCount != 0) {
                sb.AppendLine($"{result.Requestee.Name} hands over {result.ExchangeCount} {(result.ExchangeCount == 1 ? result.Rank.ToString() : pluralRankText)}.");
            }
            else {
                sb.AppendLine($"{result.Requestee.Name} says, \"Go fish.\"");
            }
            return sb.ToString();
        }

        private string ConstructInfoStringForBooksFound(IPlayer player, IEnumerable<Values> books) {
            if (books.Count() == 0) return "";

            return string.Join("\r\n", books.Select(b => $"{player.Name} lays down book of {Card.Plural(b)}.")) + "\r\n";
        }

        private string ConstructInfoStringForCardsWithdrawnFromDeck(IEnumerable<DeckWithdrawalResult> results) {
            if (results.Count() == 0) return "";

            return string.Join("\r\n",
                results.Select(r =>
                    $"{r.Player.Name} draws {r.CardCount} card{(r.CardCount == 1 ? "" : "s")} from deck."
                )) + "\r\n";

        }

    }
}