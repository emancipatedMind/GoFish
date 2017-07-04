namespace GoFish {
    using System.Collections.ObjectModel;
    using PlayingCards;
    public class UserViewModel {

        private Player _user;

        public UserViewModel(Player user) {
            _user = user;
            _user.Cards.CollectionAddedTo += Cards_CollectionAddedTo;
            _user.Cards.CollectionCleared += Cards_CollectionCleared;
        }

        public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();
        public string Name { get => _user.Name; set => _user.Name = value; }

        private void Cards_CollectionCleared(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<Card> e) {
            Cards.Clear();
        }

        private void Cards_CollectionAddedTo(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<Card> e) {
            e.Data.ForEach(c => Cards.Add(c));
        }

    }
}