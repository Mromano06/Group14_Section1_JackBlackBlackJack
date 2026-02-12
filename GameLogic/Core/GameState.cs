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
            State = newState;
        }
    }
}
