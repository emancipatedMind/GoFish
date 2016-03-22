using System;
using System.Collections.Generic;

namespace Decks {
    class CardComparer : IComparer<Card> {

    private SortCardBy sortCriteria = SortCardBy.Suit;
    public SortCardBy SortCriteria {
        get { return sortCriteria; }
        set { sortCriteria = value; }
    }

        public int Compare(Card x, Card y) {

        int sortIndex;

            switch (SortCriteria) {
                case SortCardBy.Suit:
                    return SortBySuit(x, y);
                case SortCardBy.Value:
                    return SortByValue(x, y);
                case SortCardBy.SuitThenValue:
                    sortIndex = SortBySuit(x, y);
                    if (sortIndex != 0) return sortIndex;
                    else return SortByValue(x, y);
                case SortCardBy.ValueThenSuit:
                    sortIndex = SortByValue(x, y);
                    if (sortIndex != 0) return sortIndex;
                    else return SortBySuit(x, y);
                default:
                    return 0;
            }
        }

        private int SortBySuit(Card x, Card y) {
            if (x.Suit > y.Suit) return 1;
            if (x.Suit < y.Suit) return -1;
            return 0;
        }

        private int SortByValue(Card x, Card y) {
            if (x.Value > y.Value) return 1;
            if (x.Value < y.Value) return -1;
            return 0;
        }
    }
}
