using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core {
    public class GameState 
    {
        public GameStateEnum State { get; private set; }
        public GameState() 
        {
            State = GameStateEnum.IDLE;
        }
        public void TransitionTo(GameStateEnum newState) 
        {
            // Here you could add validation logic to ensure valid state transitions
            State = newState;
        }
    }
}
