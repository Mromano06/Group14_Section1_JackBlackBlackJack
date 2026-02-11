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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
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
            backgroundBitmap.UriSource = new Uri("/Resources/MainMenu.jpg", UriKind.Relative);

            // DO THIS TO SAVE MEMORY
            backgroundBitmap.DecodePixelHeight = 720;
            backgroundBitmap.EndInit();

            // common type for images in WPF
            return new ImageBrush(backgroundBitmap)
            {
                Stretch = Stretch.UniformToFill
            };
        }

        private void RulesClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Implement the rules display logic here
            MessageBox.Show("Rules, to be implemented.");
        }

        private void PlayClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Implement the game/UI logic here
            MessageBox.Show("Play, coming soon!");
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Add any necessary cleanup code here before exiting the application
            Console.WriteLine("Application shutting down...");
            System.Environment.Exit(0);
        }

    }
}