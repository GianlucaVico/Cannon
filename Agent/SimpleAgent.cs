using System;
using System.Diagnostics;

namespace Cannon_GUI
{
    /*
     * Agent that uses Aspiration search for selecting a moves
     */
    public class SimpleAgent : Agent
    {
        protected int maxDepth;
        protected AlphaBeta search;  // Search algorithm

        public SimpleAgent(MoveGenerator generator, GameClock clock, TileColor player, int maxDepth) : base(generator, clock, player)
        {
            this.maxDepth = maxDepth;
            //search = new AlphaBeta(generator, new RandomizedEvaluator(new MaterialEvaluator(), 5));

            /* 
             * Combine all the evaluators available. The randomized evaluation is not used at the end.
             * The weights are chosen by try and error during the tournament.
             */
            IEvaluator e = new RandomizedEvaluator(new AggregateEvaluator(new IEvaluator[] {
                new MaterialEvaluator(),
                new MobilityEvaluator(generator),
                new DistanceEvaluator() },
                new int[] { 4, 1, 2 }
            ), 0);
            /*Evaluator e = new AggregateEvaluator(new Evaluator[] {
                new MaterialEvaluator(),
                new MobilityEvaluator(generator) }
            );*/
            search = new AspirationSearch(generator, e);
            ((AspirationSearch)search).Delta = 20;
            search.SetGameClock(clock);
        }

        public SimpleAgent(MoveGenerator generator, GameClock clock, TileColor player) : this(generator, clock, player, 20)
        {

        }

        //Override the search method
        protected override Move Search(GameState state)
        {
            search.ResetCounters();

            //Find a move
            Move m = ((AspirationSearch)search).Search(state, player, maxDepth, int.MinValue, int.MaxValue, out int score);
            //Console.WriteLine($"Alpha-Beta best move: {m}");

            //Write some stats
            Console.WriteLine($"Explored nodes: {search.ExploredNodes}");
            Console.WriteLine($"Pruned nodes: {search.PrunedNodes}");
            if (search.ExploredNodes == 0)
            {
                Debug.WriteLine("No node explored");
            }
            else
            {
                Console.WriteLine($"Ratio: {(float)search.PrunedNodes / search.ExploredNodes * 100}%");
            }
            Console.WriteLine($"Alpha-Beta best score: {score}");
            return m;
        }
    }
}
