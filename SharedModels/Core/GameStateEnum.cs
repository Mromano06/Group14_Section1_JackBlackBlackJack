namespace SharedModels.Core 
{
    /// <summary>
    /// Represents the game current game state.
    /// </summary>
    /// <remarks>
    /// This enumeration is used to indicate the current game
    /// state, it updates as the game progresses
    /// </remarks>
    public enum GameStateEnum 
    {
        /// <summary>
        /// Represents the state before a client connection is made.
        /// </summary>
        IDLE,
        /// <summary>
        /// Represents the state when waiting for a the connection to,
        /// be established.
        /// </summary>
        WAITING,
        /// <summary>
        /// Represents the state of when a new game is started.
        /// </summary>
        STARTINGGAME,
        /// <summary>
        /// Represents the state of when a game is currently in progress.
        /// </summary>
        PLAYING,
        /// <summary>
        /// Represents the state of when a round has ended and the server
        /// is processing the data for the next.
        /// </summary>
        PROCESSING,
        // SAVING we dont actually save anything
        /// <summary>
        /// Represents the state of when a new roud is beginning.
        /// </summary>
        RESTARTING,
        /// <summary>
        /// Represents the state of when a game is completed.
        /// </summary>
        ENDGAME,
        /// <summary>
        /// Represents the state of when a round has ended. Needed
        /// for syncronization with the UI.
        /// </summary>
        ENDROUND, // New state
        /// <summary>
        /// Represents the state of when the server is shutting down.
        /// </summary>
        EXITING
    }
}
