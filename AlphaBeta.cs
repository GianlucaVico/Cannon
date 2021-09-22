using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cannon_GUI
{
    public class AlphaBeta : Timed
    {
        protected MoveGenerator generator;
        protected Evaluator eval;
        protected MoveOrdering sort;
        public int ExploredNodes;
        public int PrunedNodes;

        public AlphaBeta(MoveGenerator generator) : this(generator, new MaterialEvaluator())
        {
        }

        public AlphaBeta(MoveGenerator generator, Evaluator eval)
        {
            this.generator = generator;
            this.eval = eval;
            this.sort = new TypeOrder();
        }

        public void ResetCounters()
        {
            ExploredNodes = 0;
            PrunedNodes = 0;
        }

        public Move Search(GameState state, TileColor player, int d, int a, int b, out int bestScore)
        {
            ExploredNodes++;
            Move bestMove = Constants.NullMove;
            GameState newState;
            if (state.End() || d == 0)
            {
                bestScore = eval.Evaluate(state, player);
                return bestMove;
            }
            int score = int.MinValue;   // this node
            int value;  // child node
            List<Move> children = generator.GenerateAll(state, player);
            children = sort.Sort(children, state);
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

    public class AspirationSearch : AlphaBeta
    {
        protected int delta = 100; // Values are already from -100 to +100
        public int Delta
        {
            get => delta;
            set
            {
                if (value > 0)
                    delta = value;
            }
        }

        public AspirationSearch(MoveGenerator generator) : base(generator)
        {
        }

        public AspirationSearch(MoveGenerator generator, Evaluator eval) : base(generator, eval)
        {
        }

        public new Move Search(GameState state, TileColor player, int d, int a, int b, out int bestScore)
        {
            Console.WriteLine("Aspiration search");
            // Sort the moves generated at root, then
            Move m0 = base.Search(state, player, 1, a, b, out bestScore);
            Move m = Constants.NullMove;
            SetIterationTimeOut(Constants.MaxIteration);
            //TODO sort first level
            for (int depth = 2; !HalfIteration() && (depth < d); depth++) //We still have time, we can explore deeper
            {
                Console.WriteLine($"Depth: {depth}");
                a = bestScore - delta;
                b = bestScore + delta;
                m = base.Search(state, player, depth, a, b, out bestScore);
                // Search again for low/high fail
                if (bestScore >= b)
                {
                    Console.WriteLine("Fail -> Search Again");
                    a = bestScore;
                    b = int.MaxValue;
                    m = base.Search(state, player, depth, a, b, out bestScore);
                }
                else if (bestScore <= a)
                {
                    Console.WriteLine("Fail -> Search Again");
                    a = int.MinValue;
                    b = bestScore;
                    m = base.Search(state, player, depth, a, b, out bestScore);
                }
            }
            if (m == Constants.NullMove)
            {
                Debug.WriteLine("Aspiration search fail, move is null", "warn");
                Debug.WriteLine(m.ToString());
            }
            return (m == Constants.NullMove) ? m0 : m;
        }

    }
}
