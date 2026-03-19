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
using Client.ViewModels;

// Matthew Romano - Feb 11th, 2026 - Main Menu for Server Application

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Blackjack : Window
    {
        public Blackjack()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

        }

    }
}