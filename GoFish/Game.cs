﻿#define AutomatedPlay
namespace GoFish {
    using System.Threading;
    using PlayingCards;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ToolkitNFW4.XAML;
    public class Game : EntityBase {

        readonly List<Card> _stock = new List<Card>();
        readonly Random randomizer = new Random();
        readonly AutoResetEvent _autoWaitHandle = new AutoResetEvent(false);
        readonly SynchronizationContext _context;

        Card? _selectedCard;
        ComputerPlayerViewModel _selectedPlayer;
        bool _gameIdle = true;
        bool _roundInProgress = false;
        string _gameProgress = "";
        int _roundNumber = 0;

        public Game() {

#if AutomatedPlay
            Players.Add(new ComputerPlayer(new Player("Peter")));
#else
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
#endif
            Players.Add(new ComputerPlayer(new Player("Melvin")));
            Players.Add(new ComputerPlayer(new Player("John")));
            Players.Add(new ComputerPlayer(new Player("Raymond")));

#if AutomatedPlay
#else
            User = new UserViewModel(user);
#endif
            ComputerPlayers.AddRange(Players.Where(p => typeof(IAutomatedPlayer).IsAssignableFrom(p.GetType())).Select(p => new ComputerPlayerViewModel(p)));

            StartGame = new DelegateCommand(StartGameCallback);
            PlayRound = new AwaitableDelegateCommand(() => Task.Factory.StartNew(PlayRoundCallback));
            RequestCard = new DelegateCommand(RequestCardCallback);
            Reset = new DelegateCommand(ResetCallback);

            _context = SynchronizationContext.Current;
        }

        List<IPlayer> Players { get; } = new List<IPlayer>();

        public UserViewModel User { get; }
        public List<ComputerPlayerViewModel> ComputerPlayers { get; } = new List<ComputerPlayerViewModel>();
        public DelegateCommand RequestCard { get; }
        public DelegateCommand StartGame { get; }
        public DelegateCommand Reset { get; }
        public AwaitableDelegateCommand PlayRound { get; }


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

        public bool RoundInProgress {
            get => _roundInProgress;
            set {
                if (_roundInProgress == value) return;
                _roundInProgress = value;
                OnPropertyChanged(nameof(RoundInProgress));
            }
        }

        int DealAmount { get; set; } = 5;

        public string GameProgress {
            get => _gameProgress;
            set {
                if (_gameProgress == value) return;
                _gameProgress = value;
                OnPropertyChanged(nameof(GameProgress));
            }
        }

        public ObservableCollection<WithdrawnBooksRecord> Books { get; } = new ObservableCollection<WithdrawnBooksRecord>();

        private void RequestCardCallback() {
            _autoWaitHandle.Set();
        }

        private void ResetCallback() {
            UpdateGameProgress("\r\nGame has been reset..." );
            GameIdle = true;
            RoundInProgress = false;
        }

        private void StartGameCallback() {
            Players
                .OfType<IAutomatedPlayer>()
                .ToList()
                .ForEach(p => p.ClearMemory());
            GameProgress = "";
            GameIdle = false;
            Books.Clear();
            _roundNumber = 0;
            Deal();
            User?.SortHand();
        }

        private bool PlayerCardNeeded =>
            PlayerStillInGame &&
                (SelectedCard == null || SelectedPlayer == null);

        private bool PlayerStillInGame =>
            Players.First().Cards.Count > 0;

        private void PlayRoundCallback() {

            RoundInProgress = true;

            GameProgress = $"********** Round #{++_roundNumber} **********\r\n";

            var results = Play(Players, 0, _stock);

            var withdrawalRecords = results.SelectMany(r => r.SelectMany(re => re.StockWithdrawalRecords));

            if (withdrawalRecords.Any()) {
                var cards = _stock.Skip(GetSkipCount(withdrawalRecords)).ToArray();
                _stock.Clear();
                _stock.AddRange(cards);
            }

            GameProgress += $"\r\nStock has {_stock.Count} card{(_stock.Count == 1 ? "" : "s")} remaining.";

            RoundInProgress = false;
            if (Books.Count == 13) {
                GameIdle = true;
                var sortedWinnerList = Books
                    .GroupBy(b => b.Player)
                    .OrderBy(b => b.Count())
                    .Select(b => new { Player = b.Key, Count = b.Count() });

                var winner = sortedWinnerList.Last();

                UpdateGameProgress($"\r\n\r\n{winner.Player.Name} is our winner with {winner.Count} books.");


                var firstPlayer = Players.Take(1).ToArray();
                var restOfPlayers = Players.Skip(1).ToArray();
                Players.Clear();
                Players.AddRange(restOfPlayers.Concat(firstPlayer));
            }
        }

        private void Deal() {
            _stock.Clear();
            IEnumerable<Card> cards = Deck.NewShuffled.ToArray();
            for (int i = 0; i < Players.Count; i++) {
                Players[i].Cards.Clear();
                Players[i].Cards.AddRange(cards.Skip(i * DealAmount).Take(DealAmount));
            }
            _stock.AddRange(cards.Skip(Players.Count * DealAmount));
        }

        private List<List<(CardRequestResult RequestResult, IEnumerable<WithdrawnBooksRecord> Books, IEnumerable<DeckWithdrawalRecord> StockWithdrawalRecords)>> Play(IEnumerable<IPlayer> allPlayers, int currPlayerIndex, IEnumerable<Card> deck) {
            IPlayer player = allPlayers.ElementAt(currPlayerIndex);
            int nextPlayerIndex = currPlayerIndex + 1;

            var playerResults = PlayerActions(player, allPlayers, deck);

            int skipAmount = GetSkipCount(playerResults.SelectMany(r => r.StockWithdrawalRecords).ToArray());

            var resultList = new List<List<(CardRequestResult, IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>)>> {
                playerResults
            };

            allPlayers
                .OfType<IAutomatedPlayer>()
                .Where(p => p.Cards.Any())
                .ToList()
                .ForEach(p => p.CommitRoundToMemory(playerResults.Select(r => r.RequestResult).ToArray()));

            if (nextPlayerIndex < allPlayers.Count()) {
                resultList.AddRange(Play(allPlayers, nextPlayerIndex, deck.Skip(skipAmount).ToArray()));
            }

            return resultList;
        }

        private List<(CardRequestResult RequestResult, IEnumerable<WithdrawnBooksRecord> Books, IEnumerable<DeckWithdrawalRecord> StockWithdrawalRecords)> PlayerActions(IPlayer player, IEnumerable<IPlayer> allPlayers, IEnumerable<Card> deck) {
            var resultList = new List<(CardRequestResult, IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>)>();

            if(player.Cards.Any()) {
                CardRequest request;
                if (typeof(IAutomatedPlayer).IsAssignableFrom(player.GetType())) {
                    request = ((IAutomatedPlayer)player).MakeRequest(allPlayers);
                }
                else if (typeof(IManualCardRequester).IsAssignableFrom(player.GetType())) {
                    UpdateGameProgress("\r\nIt is your turn. Select a card, and player.");
                    request = ((IManualCardRequester)player).MakeRequest();
                }
                else throw new ArgumentException("Player is of unknown type. Does not implement necessary interface.");

                var result = MakeCardRequest(request);
                (var books, var deckWithdrawalResults) = PostRequestActions(result, deck);
                resultList.Add((result, books, deckWithdrawalResults));

                new IPlayer[] {
                    result.Requestee,
                    result.Requester
                }
                .OfType<ISortingPlayer>()
                .ToList()
                .ForEach(p => p.SortCards());

                UpdateGameProgress((result, books, deckWithdrawalResults));

                if (result.ExchangeCount != 0) {
                    int skipAmount = GetSkipCount(deckWithdrawalResults);
                    resultList.AddRange(PlayerActions(player, allPlayers, deck.Skip(skipAmount).ToArray()));
                }
            }

            return resultList;
        }

        private (IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>) PostRequestActions(CardRequestResult result, IEnumerable<Card> deck, bool requiredDraw = true) {

            var deckWithdrawalResults = CheckIfCardsNeedToBeDrawnFromDeck(result, deck.ToArray(), requiredDraw);

            var booksWithdrawn =
                WithdrawBooksFound(new IPlayer[] {
                    result.Requester,
                    result.Requestee,
                });

            if (booksWithdrawn.Any() || deckWithdrawalResults.Any()) {

                (var newBooks, var newResults) =
                    PostRequestActions(result, deck.Skip(GetSkipCount(deckWithdrawalResults)), false);
                return (booksWithdrawn.Concat(newBooks), deckWithdrawalResults.Concat(newResults));
            }
            return (booksWithdrawn, deckWithdrawalResults);
        }

        private List<DeckWithdrawalRecord> CheckIfCardsNeedToBeDrawnFromDeck(CardRequestResult request, IEnumerable<Card> deck, bool requiredDraw) {
            var results = new List<DeckWithdrawalRecord>();

            if (deck.Any()) {
                int skipAmount = 0;

                if (request.Requestee.Cards.Count == 0) {
                    request.Requestee.Cards.AddRange(deck.Take(DealAmount));
                    results.Add(new DeckWithdrawalRecord(request.Requestee, deck.Take(DealAmount)));
                    skipAmount = DealAmount;
                }

                if (deck.Skip(skipAmount).Any()) {
                    if (request.ExchangeCount == 0 && requiredDraw) {
                        var cardsDrawn = deck.Skip(skipAmount).Take(1);
                        request.Requester.Cards.AddRange(cardsDrawn);
                        results.Add(new DeckWithdrawalRecord(request.Requester, cardsDrawn));
                    }
                    else if (request.Requester.Cards.Count == 0) {
                        request.Requester.Cards.AddRange(deck.Skip(skipAmount).Take(DealAmount));
                        results.Add(new DeckWithdrawalRecord(request.Requester, deck.Take(DealAmount)));
                    }
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

        private int GetSkipCount(IEnumerable<DeckWithdrawalRecord> records) =>
                records.Any() ?
                records.Select(r => r.CardsWithDrawn.Length).Aggregate((prev, next) => prev + next) :
                0;

        private void UseContext(Action<object> action) =>
            _context.Send(o => action(o) , null);

        #region UpdateGameProgress
        private void UpdateGameProgress((CardRequestResult, IEnumerable<WithdrawnBooksRecord>, IEnumerable<DeckWithdrawalRecord>) info) {
            UpdateGameProgress(info.Item1);
            UpdateGameProgress(info.Item3);
            UpdateGameProgress(info.Item2);
        }

        private void UpdateGameProgress(CardRequestResult result) {
            GameProgress += ConstructUpdateGameProgressString(result);
        }

        private void UpdateGameProgress(IEnumerable<WithdrawnBooksRecord> booksRecord) {
            string booksRecordString = ConstructUpdateGameProgressString(booksRecord);
            GameProgress += booksRecordString;
            UseContext(_ =>
                booksRecord
                    .ToList()
                    .ForEach(b => Books.Add(b))
            );
        }

        private void UpdateGameProgress(IEnumerable<DeckWithdrawalRecord> deckWithdrawalResults) {
            GameProgress += ConstructUpdateGameProgressString(deckWithdrawalResults);
        }

        private void UpdateGameProgress(string text) {
            GameProgress += text + "\r\n";
        }

        private string ConstructUpdateGameProgressString(CardRequestResult result) {
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

        private string ConstructUpdateGameProgressString(IEnumerable<WithdrawnBooksRecord> booksRecord) {
            if (booksRecord.Count() == 0) return "";

            return string.Join("\r\n",
                booksRecord.Select(b =>
                    $"{b.Player.Name} lays down book of {Card.Plural(b.Value)}.")
                ) + "\r\n";
        }

        private string ConstructUpdateGameProgressString(IEnumerable<DeckWithdrawalRecord> results) {
            if (results.Count() == 0) return "";

            return string.Join("\r\n",
                results.Select(r =>
                    $"{r.Player.Name} draws {r.CardsWithDrawn.Length} card{(r.CardsWithDrawn.Length == 1 ? "" : "s")} from deck.")
                ) + "\r\n";
        }
        #endregion

    }
}