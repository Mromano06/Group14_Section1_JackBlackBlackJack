using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace GameLogic.Core
{
    /// <summary>
    /// Represents the state machine managing the current state of a game session.
    /// </summary>
    /// <remarks>
    /// Tracks and transitions between game states such as idle, playing, and end round
    /// using <see cref="GameStateEnum"/> values.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class GameState
    {
        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        public GameStateEnum State { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        /// <remarks>
        /// Sets the initial state to <see cref="GameStateEnum.IDLE"/>.
        /// </remarks>
        public GameState()
        {
            State = GameStateEnum.IDLE;
        }

        /// <summary>
        /// Transitions the game to the specified state.
        /// </summary>
        /// <param name="newState">The new state to transition to.</param>
        public void TransitionTo(GameStateEnum newState)
        {
            State = newState;
        }
    }
}