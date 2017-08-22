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

            var game = (Game)Application.Current.Resources["game"];

            game.Notify += (s, e) =>
                Application.Current.Dispatcher.Invoke(new Action(() => MessageBox.Show(e.Data)));
        }

        public void Exit_Click(object sender, RoutedEventArgs e) => Close();
    }
}