using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// Matthew Romano - Feb 11th, 2026 - Main Menu for Server Application

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main_Menu : Window
    {
        public Main_Menu()
        {
            InitializeComponent();
        }

        private void rules_clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rules, To Be Implemented.");
        }

        private void play_clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Play, To Be Implemented.");
        }

        private void exit_clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exit, To Be Implemented.");
        }
    }
}