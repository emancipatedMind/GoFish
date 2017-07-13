namespace GoFish {
    using System;
    public interface ICardSorter {
        void SortCards();
        event EventHandler SortRequested;
    }
}