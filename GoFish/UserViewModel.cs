namespace GoFish {
    using System.Collections.ObjectModel;
    using PlayingCards;
    using System;
    using System.Linq;
    using System.Threading;
    public class UserViewModel {

        private ISortingPlayer _user;
        private SynchronizationContext _context;

        public UserViewModel(ISortingPlayer user) {
            _user = user;
            _user.Cards.CollectionAddedTo += Cards_CollectionAddedTo;
            _user.Cards.CollectionCleared += Cards_CollectionCleared;
            _user.SortRequested += (s, e) => SortHand();
            _context = SynchronizationContext.Current;
        }

        public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();
        public string Name { get => _user.Name; set => _user.Name = value; }

        public void SortHand() {
            UseContext(_ => {
                var sortedCards = Cards.OrderBy(c => c.Value).ToList();
                Cards.Clear();
                sortedCards.ForEach(c => Cards.Add(c));
            });
        }

        private void Cards_CollectionCleared(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<Card> e) {
            UseContext(_ => Cards.Clear());
        }

        private void Cards_CollectionAddedTo(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<Card> e) {
            UseContext(_ => e.Data.ForEach(c => Cards.Add(c)));
        }

        private void UseContext(Action<object> action) {
            _context.Send(o => action(o) , null);
        } 

    }
}