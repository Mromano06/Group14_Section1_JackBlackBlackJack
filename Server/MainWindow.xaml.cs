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

// Brodie Arkell & Matthew Romano - Feb 11th, 2026 - Main Menu for Server Application

namespace Server
{
    /// <summary>
    /// Represents the main window for the server application.
    /// </summary>
    /// <remarks>
    /// This window is responsible for initializing the server,
    /// and displaying log messages in the UI as server events occur.
    /// </remarks>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The network server instance used by the application.
        /// </summary>
        private NetworkServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the UI components, starts the server,
        /// and subscribes to the server logging event.
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();


            StartServer();
            _server.OnLog += serverOnLog; // sibscribe to Logger
        }

        /// <summary>
        /// Starts the network server as a background task.
        /// </summary>
        /// <remarks>
        /// The server is started asynchronously it listens
        /// for incoming TCP client connections on port 27000.
        /// </remarks>
        private async void StartServer()
        {
            Debug.WriteLine("Starting Server");
            _server = new NetworkServer();

            // avoid blocking UI
            _ = Task.Run(() => _server.Start(27000));
        }

        /// <summary>
        /// Handles log messages received from the server.
        /// </summary>
        /// <param name="message">The log message to display in the UI.</param>
        /// <remarks>
        /// This method  updates the UI thread using the dispatcher, appending
        /// each new message to the log list.
        /// </remarks>
        private void serverOnLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
            LogListBox.Items.Add($"{DateTime.Now:HH:mm:ss} - {message}"); // connects to the MainWindow.xaml 
            LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            });
        }

    }
}