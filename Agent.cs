using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cannon_GUI
{
    public interface Player
    {
        void ChangePlayer(TileColor player);
        TileColor Player { get; }
    }

    public abstract class Agent : Timed, Player
    {
        protected MoveOrdering sorting;
        protected MoveGenerator generator;
        protected TileColor player;
        protected Task<Move> task;
        protected bool searching;

        public Agent(MoveGenerator generator, GameClock clock, TileColor player)
        {
            SetGameClock(clock);
            this.player = player;
            this.generator = generator;
        }

        public TileColor Player => player;

        public void ChangePlayer(TileColor player)
        {
            this.player = player;
        }

        public bool DoMove(GameState state, out Move move)
        {
            //Search for the move without blocking the GUI
            bool found = false;
            move = Constants.NullMove;
            if (searching)
            {
                if (task.IsCompleted)
                {
                    searching = false;
                    move = task.Result;
                    found = true;
                    clock.Stop();
                }
            }
            else
            {
                Console.WriteLine("New Task");
                task = Task.Run(() => Search(state));
                searching = true;
                clock.Start();
            }
            return found;
        }

        protected abstract Move Search(GameState state); // Function that actually search the move
    }

    public class RandomAgent : Agent
    {
        public RandomAgent(MoveGenerator generator, GameClock clock, TileColor player) : base(generator, clock, player)
        {
            sorting = new RandomOrder();
            searching = false;
        }

        protected override Move Search(GameState state)
        {
            Move bestMove = Constants.NullMove;
            clock.Start();
            //clock.SetIterationTimeOut(Constants.MaxIteration);
            List<Move> moves = generator.GenerateAll(state, player);
            moves = sorting.Sort(moves);
            if (moves.Count > 0)
            {
                bestMove = moves[0];
            }
            Thread.Sleep(10);
            clock.Stop();
            return bestMove;
        }
    }

    public class SimpleAgent : Agent
    {
        protected int maxDepth;
        protected AlphaBeta search;

        public SimpleAgent(MoveGenerator generator, GameClock clock, TileColor player, int maxDepth) : base(generator, clock, player)
        {
            this.maxDepth = maxDepth;
            //search = new AlphaBeta(generator, new RandomizedEvaluator(new MaterialEvaluator(), 5));
            Evaluator e = new AggregateEvaluator(new Evaluator[] {
                new MaterialEvaluator(),
                new MobilityEvaluator(generator) }
            );
            search = new AspirationSearch(generator, new MaterialEvaluator());
            ((AspirationSearch)search).Delta = 20;
            search.SetGameClock(clock);
        }

        public SimpleAgent(MoveGenerator generator, GameClock clock, TileColor player) : this(generator, clock, player, 6)
        {

        }

        protected override Move Search(GameState state)
        {
            int score;
            search.ResetCounters();
            Console.WriteLine("Searching...");
            Move m = ((AspirationSearch)search).Search(state, player, maxDepth, int.MinValue, int.MaxValue, out score);
            //Console.WriteLine($"Alpha-Beta best move: {m}");
            Console.WriteLine($"Explored nodes: {search.ExploredNodes}");
            Console.WriteLine($"Pruned nodes: {search.PrunedNodes}");
            Console.WriteLine($"Ratio: {(float)search.PrunedNodes / search.ExploredNodes * 100}%");
            Console.WriteLine($"Alpha-Beta best score: {score}");
            return m;
        }
    }
}
