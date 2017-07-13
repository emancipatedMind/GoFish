namespace GoFish {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PlayingCards;
    using ToolkitNFW4.XAML;

    public class ComputerPlayer : IAutomatedPlayer {

        readonly Dictionary<Values, IPlayer> _memory = new Dictionary<Values, IPlayer>();
        private IPlayer _player;

        Random randomizer = new Random();

        public ComputerPlayer(IPlayer player) => _player = player;

        public CustomCollection<Card> Cards => _player.Cards;
        public string Name { get => _player.Name; set => _player.Name = value; }

        public void ClearMemory() {
            _memory.Clear();
        }

        public void CommitRoundToMemory(IEnumerable<CardRequestResult> results) {
            results
                .Where(r => r.Requester != this)
                .Select(r => new { Value = r.Rank, Player = r.Requester })
                .ToList()
                .ForEach(x => _memory[x.Value] = x.Player);
        }

        public CardRequest MakeRequest(IEnumerable<IPlayer> players) {
            Values rank;
            IPlayer requestee;

            var values = Cards.GroupBy(c => c.Value).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).Select(x => x.Value);

            if (_memory.Any() && values.Any(v => _memory.ContainsKey(v))) {
                rank = values.First(v => _memory.ContainsKey(v));
                requestee = _memory[rank];
                _memory.Remove(rank);
            }
            else {
                IPlayer[] otherPlayers = players.Where(pl => pl != this && pl.Cards.Any()).ToArray();

                requestee = otherPlayers[randomizer.Next(otherPlayers.Length)];
                rank = Cards.ElementAt(randomizer.Next(Cards.Count)).Value;
            }


            return new CardRequest(this, requestee, rank);
        }

    }
}