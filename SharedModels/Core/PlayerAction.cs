namespace SharedModels.Core
{
    /// <summary>
    /// Represents the action taken by a player during thier turn.
    /// </summary>
    /// <remarks>
    /// This enumeration is used to indicate what the player did
    /// on their turn.
    /// </remarks>
    public enum PlayerAction
    {
        /// <summary>
        /// Represents the action of a player placing a bet.
        /// </summary>
        Bet,
        /// <summary>
        /// Represents the action of a player hitting and gaining a card.
        /// </summary>
        Hit,
        /// <summary>
        /// Represents the action of a player ending their turn.
        /// </summary>
        Stand,
        /// <summary>
        /// Unused in this implementation.
        /// </summary>
        Split,
        /// <summary>
        /// Represents the action of a player "doubling down". This results
        /// in them gaining a card, doubling thier bet and ending their turn.
        /// </summary>
        Double,
        /// <summary>
        /// Represents the action of a player taking insurance againt the 
        /// dealer incase they have a blackjack.
        /// </summary>
        Insure
    }
}
