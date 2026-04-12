using System;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Data transfer object sent by the client when requesting to join a game session.
    /// Contains the player's chosen name and the server passcode.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// The passcode the player entered on the main menu.
        /// </summary>
        public int Passcode { get; set; }

        /// <summary>
        /// The player's chosen display name.
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;
    }
}
