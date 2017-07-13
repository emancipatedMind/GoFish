namespace GoFish {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using PlayingCards;
    public class CardValueConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Values v) {
                return Card.Plural(v);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}