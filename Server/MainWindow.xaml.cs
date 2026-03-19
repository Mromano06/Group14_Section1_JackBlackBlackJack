using Server.Networking;
using System.Diagnostics;
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
    public partial class MainWindow : Window
    {
        private NetworkServer _server;
        public MainWindow()
        {
            InitializeComponent();


            StartServer();
            _server.OnLog += serverOnLog; // sibscribe to Logger
        }

        private async void StartServer()
        {
            Debug.WriteLine("Starting Server");
            _server = new NetworkServer();

            // avoid blocking UI
            _ = Task.Run(() => _server.Start(27000));
        }

        // function to invoke when using the Log Action
        private void serverOnLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogListBox.Items.Add($"{DateTime.Now:HH:mm:ss} - {message}"); // connects to the MainWindow.xaml 
            });
        }

    }
}