using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoFish {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private Game game;

        private void buttonStart_Click(object sender, RoutedEventArgs e) {
            if(String.IsNullOrEmpty(textName.Text)) {
                MessageBox.Show("Please enter your name", "Can't start the game yet");
                return;
            }
            textProgress.Content = String.Empty;
            game = new Game(textName.Text, new List<string> { "Joe", "Bob" }, textProgress);
            buttonStart.IsEnabled = false;
            textName.IsEnabled = false;
            buttonAsk.IsEnabled = true;
            listHand.IsEnabled = true;
            UpdateForm(false);
        }

        private void UpdateForm(bool skipDescribePlayerHands) {
            listHand.Items.Clear();
            foreach (String cardName in game.GetPlayerCardNames()) listHand.Items.Add(cardName);
            textBooks.Content = game.DescribeBooks();
            textProgress.Content += "------------------------------------------------------------" + Environment.NewLine;
            if (!skipDescribePlayerHands) textProgress.Content += game.DescribePlayerHands();
        }

        private void buttonAsk_Click(object sender, RoutedEventArgs e) {
            textProgress.Content = "";
            if (listHand.SelectedIndex < 0) {
                MessageBox.Show("Please select a card");
                return;
            }
            bool winnerFound = game.PlayOneRound(listHand.SelectedIndex);
            UpdateForm(winnerFound);
            if (winnerFound) {
                textProgress.Content += "The winner is... " + game.GetWinnerName();
                buttonAsk.IsEnabled = false;
                listHand.IsEnabled = false;
                buttonStart.IsEnabled = true;
            }
        }
    }
}
