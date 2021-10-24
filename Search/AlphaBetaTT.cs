using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cannon_GUI
{
    /*
     * Alpha-beta search with TT, killer moves, history tables and quiescence search
     * 
     * This works similarly to the AlphaBeta class, but it uses a different 
     * IMoveOrdering and it updates the tables.    
     */
    public class AlphaBetaTT : AlphaBeta
    {
        protected TT tt;
        protected HT ht;
        protected KT kt;

        public AlphaBetaTT(MoveGenerator generator) : this(generator, new MaterialEvaluator())
        {
        }

        public AlphaBetaTT(MoveGenerator generator, IEvaluator eval) : base(generator, eval)
        {
            tt = new TT((int)Math.Pow(2, Constants.HashKeySize));
            ht = new HT(Constants.HistTableSize);
            kt = new KT(Constants.KillerTableSize);
            sort = new TableOrdering(ht, kt);
        }

        public new void ResetCounters()
        {
            ExploredNodes = 0;
            PrunedNodes = 0;
            tt.Clear();
        }

        /*
         * Search the best move.
         * 
         * The quiescence search is used only on leaf node and it investigate 
         * only capture (no shooting) moves.        
         * 
         * Args:
         *  state (GameState): game state to investigate
         *  player (TileColor): player to move
         *  d (int): depth
         *  a (int): alpha
         *  b (int): beta
         *  ext (int): extension for the quiescience search (search deeper for capture)        
         *  out bestScore (int): score of the game state
         * Returns
         *  Move: best move
         */
        public Move Search(GameState state, TileColor player, int d, int a, int b, int ext, out int bestScore)
        {
            /*if (ext != 0)
            {
                Console.WriteLine($"Extension: {ext}");
            }*/
            ExploredNodes++;
            Move bestMove = Constants.NullMove;
            GameState newState;

            //TT look up
            int olda = a;
            bool found = tt.Query(state, out TTEntry n);

            int sign = (n.depth % 2 == d % 2) ? 1 : -1;
            //int sign = 1;
            //if (found && n.depth >= d && ((n.depth % 2) == (d % 2))) // Scores for the correct player
            if (found && n.depth >= d) // Scores for the correct player
            {
                //Console.WriteLine("TT");
                if (n.type == TTType.exact)
                {
                    bestScore = n.value * sign;
                    PrunedNodes++;
                    return n.bestMove;
                }
                else if (n.type == TTType.lower)
                {
                    a = Math.Max(a, n.value * sign);
                }
                else if (n.type == TTType.upper)
                {
                    b = Math.Min(b, n.value * sign);
                }
                if (a > b)
                {
                    bestScore = n.value * sign;
                    PrunedNodes++;
                    return n.bestMove;
                }
            }
            //

            if (state.End() || (d + ext) == 0)
            {
                bestScore = eval.Evaluate(state, player);
                return bestMove;
            }
            int score = int.MinValue;   // this node
            int value;  // child node
            List<Move> children;

            if (ext == 0) // Quiescence search: only captures, no need to sort
            {
                children = generator.GenerateAll(state, player);
                children = sort.Sort(children, state, d);
            }
            else
            {
                children = generator.GenerateAllCaptures(state, player);
                //Console.WriteLine(children.Count);
            }


            foreach (Move m in children)
            {
                newState = state.Apply(m);
                //quiescent search only when at leaf nodes
                Search(newState, Utils.SwitchColor(player), d - 1, -b, -a, (m.Type == MoveType.capture && d == 0) ? ext + Constants.Extension : 0, out value); //Ignore best grandchild
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
                    // KT update
                    kt.Update(bestMove, d);
                    //
                    break;      // prune
                }
            }

            // TT update
            TTType t = TTType.exact;
            if (score <= olda)
                t = TTType.upper;
            else if (score >= b)
                t = TTType.lower;
            tt.Push(state, t, bestMove, score, d);
            //

            // HT update
            //ht.Update(bestMove);
            //

            bestScore = score;
            return bestMove;
        }
    }
}
