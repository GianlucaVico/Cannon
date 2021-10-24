using System;
using System.Diagnostics;

namespace Cannon_GUI
{
    /*
     * Use iterative deepening and aspiration search to find the best move.
     * 
     * It search 1 ply deep to find an initial value and then it search deeper
     * with a window of (-delta, +delta).
     * 
     * A large delta makes this search algorithm similar to a simpler iterative
     * deeping alpha-beta search.
     */
    public class AspirationSearch : AlphaBetaTT
    {
        protected int delta = 50;
        public int Delta
        {
            get => delta;
            set
            {
                if (value > 0)
                    delta = value;
            }
        }
        // TODO add base search obj
        public AspirationSearch(MoveGenerator generator) : base(generator)
        {
        }

        public AspirationSearch(MoveGenerator generator, IEvaluator eval) : base(generator, eval)
        {
        }

        public new Move Search(GameState state, TileColor player, int d, int a, int b, out int bestScore)
        {
            Console.WriteLine("Aspiration search");
            // Sort the moves generated at root, then
            Move m0 = base.Search(state, player, 1, a, b, out bestScore);
            Console.WriteLine($"1-ply: {m0} - {bestScore}");
            Move m = m0;
            SetIterationTimeOut(Constants.MaxIteration);

            for (int depth = 2; !HalfIteration() && (depth < d); depth++) //We still have time, we can explore deeper
            //for (int depth = 2; (depth < d); depth++) //We still have time, we can explore deeper
            {
                Console.WriteLine($"Depth: {depth}");
                /*a = bestScore - delta;
                b = bestScore + delta;*/
                m = Search(state, player, depth, bestScore - delta, bestScore + delta, 0, out bestScore);
                // Search again for low/high fail
                if (bestScore >= b)
                {
                    Console.WriteLine($"Fail -> Search Again - {bestScore}");
                    a = bestScore;
                    b = int.MaxValue;
                    m = base.Search(state, player, depth, a, b, out bestScore);
                }
                else if (bestScore <= a)
                {
                    Console.WriteLine($"Fail -> Search Again - {bestScore}");
                    a = int.MinValue;
                    b = bestScore;
                    m = base.Search(state, player, depth, a, b, out bestScore);
                }
                Console.WriteLine(m);
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
