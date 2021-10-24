using System;
using System.Collections.Generic;

namespace Cannon_GUI
{
    /*
     * Base alpha-beta search
     */
    public class AlphaBeta : Timed
    {
        protected MoveGenerator generator;
        protected IEvaluator eval;
        protected IMoveOrdering sort;
        public int ExploredNodes;
        public int PrunedNodes;

        public AlphaBeta(MoveGenerator generator) : this(generator, new MaterialEvaluator())
        {
        }

        public AlphaBeta(MoveGenerator generator, IEvaluator eval)
        {
            this.generator = generator;
            this.eval = eval;
            this.sort = new TypeOrdering();
        }

        public void ResetCounters()
        {
            ExploredNodes = 0;
            PrunedNodes = 0;
        }

        /*
         * Search the best move
         * 
         * Args:
         *  state (GameState): game state to investigate
         *  player (TileColor): player to move
         *  d (int): depth
         *  a (int): alpha
         *  b (int): beta
         *  out bestScore (int): score of the game state
         * Returns
         *  Move: best move
         */
        public Move Search(GameState state, TileColor player, int d, int a, int b, out int bestScore)
        {
            ExploredNodes++;
            Move bestMove = Constants.NullMove;
            GameState newState;
            if (state.End() || d == 0) // Leaf node
            {
                bestScore = eval.Evaluate(state, player);
                return bestMove;
            }
            int score = int.MinValue;   // this node
            int value;  // child node
            List<Move> children = generator.GenerateAll(state, player);
            children = sort.Sort(children, state, d);
            foreach (Move m in children)
            {
                newState = state.Apply(m);
                Search(newState, Utils.SwitchColor(player), d - 1, -b, -a, out value); //Ignore best grandchild
                value *= -1; //Change sign of the value 
                if (value > score)
                {
                    score = value;
                    bestMove = m; // keep best move
                }
                if (score > a)
                    a = score;  // better lower bound
                if (score >= b)
                {
                    PrunedNodes++;
                    break;      // prune
                }
            }
            bestScore = score;
            return bestMove;
        }
    }
}
