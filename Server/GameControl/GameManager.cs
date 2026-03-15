using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using GameLogic.Core;
using Server.Networking;
using Jables_Protocol.DTOs;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.GameControl
{
    public class GameManager
    {
        // create game
        private readonly ClientConnection _connection;
        private Game _game;


        public GameManager(ClientConnection connection)
        {
            _connection = connection;

            // initialize the new game
            _game = new Game();

        }

        // this is the callback function that will run when message received from client
        // we need to use the Jables_Protocol to deserialize the message here 
        public void OnMessageReceived(ClientConnection sender, byte[] data)
        {
            ///TODO: deserialize the message using custom protocol
            /// and then 
            
            Debug.WriteLine("Client meassage received, size of: " +  data.Length + " bytes.");

        }

        public void HandleMesssage(ClientConnection sender, byte[] data)
        {
            ///TODO: Create the logic that will manage and change the game based on messages from client


            Debug.WriteLine("test");
        }

    }
}
