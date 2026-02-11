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
            // TODO: Implement the rules display logic here
            MessageBox.Show("Rules, to be implemented.");
        }

        private void play_clicked(object sender, RoutedEventArgs e)
        {
            // TODO: Implement the game/UI logic here
            MessageBox.Show("Play, coming soon!");
        }

        private void exit_clicked(object sender, RoutedEventArgs e)
        {
            // TODO: Add any necessary cleanup code here before exiting the application
            Console.WriteLine("Application shutting down...");
            System.Environment.Exit(0);
        }
    }
}