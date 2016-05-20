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

        Game game;
        Random rnd = new Random();

        public MainWindow() {
            InitializeComponent();
            game = FindResource("game") as Game;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e) {
            if(String.IsNullOrEmpty(textName.Text)) {
                MessageBox.Show("Please enter your name", "Can't start the game yet");
                return;
            }
            game.StartGame();
        }

        private void buttonAsk_Click(object sender, RoutedEventArgs e) {
            if (listHand.SelectedIndex < 0) {
                listHand.SelectedIndex = rnd.Next(listHand.Items.Count);
            }
            game.PlayOneRound(listHand.SelectedIndex);
        }

    }
}
