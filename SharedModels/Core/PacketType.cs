namespace SharedModels.Core
{
    /// <summary>
    /// Represents the game current game state.
    /// </summary>
    /// <remarks>
    /// This enumeration is used to indicate the current game
    /// state, it updates as the game progresses
    /// </remarks>
    public enum PacketType : byte   // set the size of enum to byte
    {
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        Error,
        /// <summary>
        /// Packet that represents a player object.
        /// </summary>
        Player,
        /// <summary>
        /// Packet that represents a player action in the game
        /// </summary>
        PlayerAction,
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        StateUpdate,
        /// <summary>
        /// Packet that represents a change in the game.
        /// </summary>
        GameUpdate,
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        CardDealt,
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        HandDealt,
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        JoinRequest,
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        EndGame,
        /// <summary>
        /// Packet that represents the client disconnecting
        /// </summary>
        Disconnect,
        /// <summary>
        /// Packet sent by the client containing a passcode and player name for authentication.
        /// </summary>
        LoginRequest,
        /// <summary>
        /// Packet sent by the server indicating whether the login was accepted or denied.
        /// </summary>
        LoginResponse,

    }
}