using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using GameLogic.Core;
using Server.Networking;
using Jables_Protocol.DTOs;
using System.Diagnostics;

namespace Server.GameControl
{
    public class GameManager
    {
        // create game
        private Game _game = new Game();
        private List<ClientConnection> _clients = new();

        public void AddClient(ClientConnection connection)
        {
            _clients.Add(connection);
        }

        public void HandleMessage(ClientConnection sender, byte[] data)
        {
            ///TODO: Create the logic that will manage and change the game based on messages from client


            Debug.WriteLine("test");
        }

    }
}
