using System;
using System.Collections.Generic;

namespace Cannon_GUI
{
    public class History
    {
        protected List<Move> moves; // In case we want to undo some move
        protected List<GameState> quietStates; //Only this states can be repeated
        protected List<TimeSpan> aiTimes;
        public History()
        {
            moves = new List<Move>();
            quietStates = new List<GameState>();
            aiTimes = new List<TimeSpan>();
        }

        public void Push(Move m, GameState oldState, TimeSpan time)
        {

        }
    }
}
