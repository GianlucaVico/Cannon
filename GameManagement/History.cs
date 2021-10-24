using System;
using System.Collections.Generic;

namespace Cannon_GUI
{
    /*
     * List of game states occured during the game.
     * 
     * Used to check if a state has been repeated 3+ times (to be implemented)
     * and for the undo button.
     */
    public class History
    {
        protected List<Move> moves; // In case we want to undo some move
        protected List<GameState> states; //Only this states can be repeated
        protected List<TimeSpan> aiTimes;
        protected Dictionary<int, int> hashCounts; // count how many times we saw a state
        protected bool threeRepetitions;

        public bool ThreeRepetitions => threeRepetitions;

        public History()
        {
            moves = new List<Move>();
            states = new List<GameState>();
            aiTimes = new List<TimeSpan>();
            threeRepetitions = false;
            hashCounts = new Dictionary<int, int>();
        }

        /*
         * Add a new state to the history
         * 
         * Args:
         *  m (Move): move performed
         *  state (GameState): new current states
         *  time (TimeSpan): time from the beginning of the game (for the agent)        
         */
        public void Push(Move m, GameState state, TimeSpan time)
        {
            if (!threeRepetitions)
            {
                moves.Add(m); // add at the end
                states.Add(state);
                aiTimes.Add(time);
                if ((m.Type == MoveType.capture) || (m.Type == MoveType.shoot))
                {
                    hashCounts.Clear();
                }
                else
                {
                    if (hashCounts.ContainsKey(state.PrimaryHash))
                    {
                        hashCounts[state.PrimaryHash]++;
                    }
                    else
                    {
                        hashCounts[state.PrimaryHash] = 1;
                    }
                }
                if (hashCounts.TryGetValue(state.PrimaryHash, out int i))
                    if (i >= 3)
                        threeRepetitions = true;
            }
        }

        /*
         * Add a new state to the history
         * 
         * Args:
         *  m (Move): move performed
         *  state (GameState): new current states
         */
        public void Push(Move m, GameState state)
        {
            Push(m, state, aiTimes.Count == 0 ? TimeSpan.Zero : aiTimes[aiTimes.Count - 1]);
        }

        /*
         * Undo the last move. It simply change the game state to the previous 
         * one instead of using GameState.Undo and the last move.        
         */
        public void Undo(TileManager manager, Agent ai, GameClock clock)
        {
            if ((states.Count != 0) && (aiTimes.Count != 0))  // allow undo only when human player is playing
            {
                //Console.WriteLine($"History: {states.Count}");
                threeRepetitions = false;
                manager.FromGameState(states[states.Count - 2]);
                manager.Update();
                clock.SetTime(aiTimes[aiTimes.Count - 2]);
                manager.playing = !manager.playing;

                states.RemoveAt(states.Count - 1);
                aiTimes.RemoveAt(aiTimes.Count - 1);
                states.RemoveAt(states.Count - 1);
                aiTimes.RemoveAt(aiTimes.Count - 1);
            }
        }
    }
}
