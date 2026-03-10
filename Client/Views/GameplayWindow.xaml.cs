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
    public partial class GameplayWindow : UserControl
    {
        private Blackjack _mainWindow;

        public GameplayWindow(Blackjack mainWindow)
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

        // TODO: Properly implement buttons and thier actions
        private void HitClicked(object sender, RoutedEventArgs e)
        {
            // Give the player another card (match the card read from the server)
            // Use dynamic routing to match the card with the key
        }

        private void StandClicked(object sender, RoutedEventArgs e)
        {
            // End the round for the player
            // Compare and handle winnings/losings
            // Send the user to the next round/main screem
        }

        private void DoubleClicked(object sender, RoutedEventArgs e)
        {
            // Give the player one more card and dissallow them to hit again
            // Acts as a final hit+stand combined into one turn
        }

        private void SplitClicked(object sender, RoutedEventArgs e)
        {
            // Split players hand into seperate ones
            // Server handles the actual logic jsut keep proper hands together
            // Allow the user to now play two hands one after another
        }
    }
}
