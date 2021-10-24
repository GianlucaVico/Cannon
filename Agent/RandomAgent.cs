using System.Collections.Generic;
using System.Threading;

namespace Cannon_GUI
{
    /*
     * Agent that always plays a random move.
     *
     * For testing.
     */
    public class RandomAgent : Agent
    {
        public RandomAgent(MoveGenerator generator, GameClock clock, TileColor player) : base(generator, clock, player)
        {
            sorting = new RandomOrdering();
            searching = false;
        }

        protected override Move Search(GameState state)
        {
            Move bestMove = Constants.NullMove;
            clock.Start();
            //clock.SetIterationTimeOut(Constants.MaxIteration);
            List<Move> moves = generator.GenerateAll(state, player);
            moves = sorting.Sort(moves, null, 0);
            if (moves.Count > 0)
            {
                bestMove = moves[0];
            }
            //Thread.Sleep(10);
            clock.Stop();
            return bestMove;
        }
    }
}
