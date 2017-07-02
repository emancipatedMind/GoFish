namespace GoFish {
    using ToolkitNFW4.XAML;
    using System.Linq;
    public class PlayerViewModel : EntityBase {

        Player _player;

        public PlayerViewModel(Player player) {
            _player = player;
            _player.Cards.CollectionAddedTo += CollectionChanged;
            _player.Cards.CollectionCleared += CollectionChanged;
        }

        private void CollectionChanged(object sender, ToolkitNFW4.EventArgs.GenericCollectionEventArgs<PlayingCards.Card> e) {
            OnPropertyChanged(nameof(Count));
        }

        public string Name { get => _player.Name; set => _player.Name = value; }
        public int Count => _player.Cards.Count();

    }
}