namespace GoFish {
    using System.Threading;
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
        AutoResetEvent _autoWaitHandle = new AutoResetEvent(false);

        public Game() {

            var user = new User(new Player("Peter"));

            user.CardRequested += (s, e) => {
                while (true) {
                    _autoWaitHandle.WaitOne();
                    if (SelectedCard == null || SelectedPlayer == null)
                        FireNotifyEvent("Please select both a card to ask for, and the person to ask.");
                    else {
                        e.Card = SelectedCard.Value;
                        e.Player = SelectedPlayer.Player;
                        SelectedCard = null;
                        SelectedPlayer = null;
                        break;
                    }
                }
            };

            Players.Add(user);
            Players.Add(new ComputerPlayer(new Player("Melvin")));
            Players.Add(new ComputerPlayer(new Player("John")));
            Players.Add(new ComputerPlayer(new Player("Raymond")));

            User = new UserViewModel(Players.First());
            ComputerPlayers.AddRange(Players.Skip(1).Select(p => new ComputerPlayerViewModel(p)));

            StartGame = new DelegateCommand(StartGameCallback);
            PlayRound = new DelegateCommand(PlayRoundCallback);
            RequestCard = new DelegateCommand(RequestCardCallback);
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

        private void RequestCardCallback() {
            _autoWaitHandle.Set();
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

            GameProgress = $"********** Round #{++_roundNumber} **********\r\n";

            var results = AutomatedPlay(Players, Cards);

            if (results.SelectMany(r=> r.StockWithdrawalRecords).Any()) {
                int cardsDealtCount = results.SelectMany(r => r.StockWithdrawalRecords).Select(r => r.CardCount).Aggregate((prev, next) => prev + next);

                var cards = Cards.Skip(cardsDealtCount).ToArray();
                Cards.Clear();
                Cards.AddRange(cards);
            }

            GameProgress += $"\r\nStock has {Cards.Count} card{(Cards.Count == 1 ? "" : "s")} remaining.";

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

        private (CardRequestResult RequestResult, IEnumerable<WithdrawnBooksRecord> Books, IEnumerable<DeckWithdrawalRecord> StockWithdrawalRecords)[] AutomatedPlay(IEnumerable<IPlayer> players, IEnumerable<Card> deck) {

            return players.Select(p => {
                (var crr, var wbr, var dwr) = PlayHand(p, players, deck);
                Log((crr, wbr, dwr));
                return (crr, wbr, dwr);
            }).ToArray();

        }

        private (CardRequestResult, IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>) PlayHand(IPlayer player, IEnumerable<IPlayer> allPlayers, IEnumerable<Card> deck) {
            CardRequest request;
            if (typeof(IAutomatedPlayer).IsAssignableFrom(player.GetType())) {
                request = ((IAutomatedPlayer)player).MakeRequest(allPlayers.Where(p => p.Cards.Any()));
            }
            else if (typeof(IManualPlayer).IsAssignableFrom(player.GetType())) {
                Log("It is your turn.");
                request = ((IManualPlayer)player).MakeRequest();
            }
            else throw new ArgumentException("Player is of unknown type. Does not implement necessary interface.");

            var result = MakeCardRequest(request);

            (var books, var deckWithdrawalResults) = PostRequestActions(result, deck);

            return (result, books, deckWithdrawalResults);
        }

        private (IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>) PostRequestActions(CardRequestResult result, IEnumerable<Card> deck) {

            var deckWithdrawalResults = CheckIfCardsNeedToBeDrawnFromDeck(result, deck.ToArray());

            var booksWithdrawn =
                WithdrawBooksFound(new IPlayer[] {
                    result.Requester,
                    result.Requestee,
                });

            if (booksWithdrawn.Any() && deckWithdrawalResults.Any()) {

                (var newBooks, var newResults) =
                    PostRequestActions(result, deck.Skip(
                        deckWithdrawalResults.Select(r => r.CardCount).Aggregate((prev, next) => prev + next)
                    ));
                return (booksWithdrawn.Concat(newBooks), deckWithdrawalResults.Concat(newResults));
            }
            return (booksWithdrawn, deckWithdrawalResults);
        }

        // Problematic. This could potentially serve the same cards.
        private List<DeckWithdrawalRecord> CheckIfCardsNeedToBeDrawnFromDeck(CardRequestResult request, IEnumerable<Card> deck) {
            var results = new List<DeckWithdrawalRecord>();

            if (deck.Any()) {
                int skipAmount = 0;

                if (request.Requestee.Cards.Count == 0) {
                    request.Requestee.Cards.AddRange(deck.Take(5));
                    results.Add(new DeckWithdrawalRecord(request.Requestee, 5));
                    skipAmount = 5;
                }

                if (request.ExchangeCount == 0) {
                    request.Requester.Cards.AddRange(deck.Skip(skipAmount).Take(1));
                    results.Add(new DeckWithdrawalRecord(request.Requester, 1));
                }
                else if (request.Requester.Cards.Count == 0) {
                    request.Requester.Cards.AddRange(deck.Skip(skipAmount).Take(5));
                    results.Add(new DeckWithdrawalRecord(request.Requester, 5));
                }
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

        private WithdrawnBooksRecord[] WithdrawBooksFound(IEnumerable<IPlayer> players) {
            return players.SelectMany(p => {
                var valuesToBeRemoved = p
                    .Cards
                    .GroupBy(c => c.Value)
                    .Where(g => g.Count() == 4)
                    .Select(g => g.Key)
                    .ToList();

                valuesToBeRemoved
                    .ForEach(v => {
                        var cardsToKeep = p.Cards.Where(c => c.Value != v).ToArray();
                        p.Cards.Clear();
                        p.Cards.AddRange(cardsToKeep);
                    });

                return valuesToBeRemoved.Select(b => new WithdrawnBooksRecord(p, b)).ToArray();
            })
            .ToArray();
        }

        #region Log
        private void Log((CardRequestResult, IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>) info) {
            Log(info.Item1);
            Log(info.Item2);
            Log(info.Item3);
        }

        private void Log(CardRequestResult result) {
            GameProgress += ConstructLogString(result);
        }

        private void Log(IEnumerable<WithdrawnBooksRecord> booksRecord) {
            string booksRecordString = ConstructLogString(booksRecord);
            GameProgress += booksRecordString;
            Books += booksRecordString;
        }

        private void Log(IEnumerable<DeckWithdrawalRecord> deckWithdrawalResults) {
            GameProgress += ConstructLogString(deckWithdrawalResults);
        }

        private void Log(string text) {
            GameProgress += text + "\r\n";
        }

        private string ConstructLogString(CardRequestResult result) {
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

        private string ConstructLogString(IEnumerable<WithdrawnBooksRecord> booksRecord) {
            if (booksRecord.Count() == 0) return "";

            return string.Join("\r\n",
                booksRecord.Select(b =>
                    $"{b.Player.Name} lays down book of {Card.Plural(b.Value)}.")
                ) + "\r\n";
        }

        private string ConstructLogString(IEnumerable<DeckWithdrawalRecord> results) {
            if (results.Count() == 0) return "";

            return string.Join("\r\n",
                results.Select(r =>
                    $"{r.Player.Name} draws {r.CardCount} card{(r.CardCount == 1 ? "" : "s")} from deck.")
                ) + "\r\n";
        }
        #endregion

    }
}