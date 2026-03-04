using System;
using System.Collections.Generic;
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

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for GameplayWindow.xaml
    /// </summary>
    public partial class BetPlacingWindow : UserControl
    {
        private Blackjack _mainWindow;
        private int currentBet = 0;

        public BetPlacingWindow(Blackjack mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private static ImageBrush SetupBackground()
        {
            // Create image obj
            Image background = new Image();
            background.Width = 200;

            // Create the source so WPF can read it
            // And map it to the URI for the image
            BitmapImage backgroundBitmap = new BitmapImage();
            backgroundBitmap.BeginInit();
            backgroundBitmap.UriSource = new Uri("/Assets/Gameboard.jpg", UriKind.Relative);

            // DO THIS TO SAVE MEMORY
            backgroundBitmap.DecodePixelHeight = 720;
            backgroundBitmap.EndInit();

            // common type for images in WPF
            return new ImageBrush(backgroundBitmap)
            {
                Stretch = Stretch.UniformToFill
            };
        }

        private void IncBet(object sender, RoutedEventArgs e)
        {
            currentBet += 10;
            Bet.Text = $"Current Bet: {currentBet}"; // To display the bet amount
        }

        private void DecBet(object sender, RoutedEventArgs e)
        {
            currentBet -= 10;
            Bet.Text = $"Current Bet: {currentBet}"; // To display the bet amount
        }

        // TODO: Make a displayed money amount that acts as the upper limit for the max
        private void MaxBet(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Max to be implemented.");
        }

        // TODO: Make have this rout to and send relevent info for the actual turn screen
        private void PlaceBet(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Place Bet to be implemented.");
        }
    }
}
