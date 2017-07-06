namespace GoFish {
    using System.Collections.ObjectModel;
    using PlayingCards;
    using System.Linq;
    public class UserViewModel {

        private IPlayer _user;

        public UserViewModel(IPlayer user) {
            _user = user;
            _user.Cards.CollectionAddedTo += Cards_CollectionAddedTo;
            _user.Cards.CollectionCleared += Cards_CollectionCleared;
        }

        public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();
        public string Name { get => _user.Name; set => _user.Name = value; }

        public void SortHand() {
            var sortedCards = Cards.OrderBy(c => c.Value).ToList();
            Cards.Clear();
            sortedCards.ForEach(c => Cards.Add(c));
        }

        private void Cards_CollectionCleared(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<Card> e) {
            Cards.Clear();
        }

        private void Cards_CollectionAddedTo(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<Card> e) {
            e.Data.ForEach(c => Cards.Add(c));
        }

    }
}