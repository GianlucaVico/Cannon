using System;
using System.Threading.Tasks;

namespace Cannon_GUI
{
    /*
     * Abstract class for the AI Agent.
     * Its main goal is to define a search method that does not block the GUI.
     * 
     * It uses a GameClock to manage the time.
     */
    public abstract class Agent : Timed, IPlayer
    {
        protected IMoveOrdering sorting; // Sort the moves
        protected MoveGenerator generator; // Generate the moves
        protected TileColor player;
        protected Task<Move> task;  // Search the moves
        protected bool searching;  // Whether it is done searching

        public TileColor Player => player;

        public Agent(MoveGenerator generator, GameClock clock, TileColor player)
        {
            SetGameClock(clock);
            this.player = player;
            this.generator = generator;
        }

        public void ChangePlayer(TileColor player)
        {
            this.player = player;
        }

        /*
         * Search for a move.
         * 
         * Args:
         *  state (GameState): current state of the game
         *  out move (Move): move found, Constants.NullMove if it is still searching
         * Returns:
         *  bool: whether it found a moves
         */
        public bool DoMove(GameState state, out Move move)
        {
            //Search for the move without blocking the GUI
            bool found = false;
            move = Constants.NullMove;
            if (searching)
            {
                if (task.IsCompleted)
                {
                    task.Wait();
                    searching = false;
                    move = task.Result;
                    found = true;
                    clock.Stop();
                    task.Dispose();
                    GC.Collect();
                }
            }
            else
            {
                //Console.WriteLine("New Task");
                task = Task.Run(() => Search(state));
                searching = true;
                clock.Start();
            }
            /*move = Search(state);
            bool found = true;*/
            return found;
        }

        /*
         * Run the search algorithm to find a move.
         * To be implemented in the subclasses.        
         * 
         * Args:
         *  state (GameState): current state of the game
         * Returns:
         *  Move: best move
         */
        protected abstract Move Search(GameState state); // Function that actually search the move
    }
}
