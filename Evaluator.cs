using System;
using System.Diagnostics;
namespace Cannon_GUI
{
    public interface Evaluator
    {
        int Evaluate(GameState state, TileColor player);
    }

    public class RandomizedEvaluator : Evaluator
    {
        protected Random rnd;
        protected Evaluator baseEvaluator;
        protected int halfRange;

        public RandomizedEvaluator() : this(new ConstantEvaluator(), 5, 0) { }

        public RandomizedEvaluator(Evaluator baseEvaluator, int halfRange) : this(baseEvaluator, halfRange, 0) { }

        public RandomizedEvaluator(Evaluator baseEvaluator, int halfRange, int seed)
        {
            Debug.Assert(halfRange > 0, "The half range must be positive");
            rnd = new Random(seed);
            this.baseEvaluator = baseEvaluator;
            this.halfRange = halfRange;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            return baseEvaluator.Evaluate(state, player) + rnd.Next(-halfRange, halfRange);
        }
    }

    public class ConstantEvaluator : Evaluator //placholder
    {
        protected int value;

        public ConstantEvaluator(int value = 0)
        {
            this.value = value;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            return value;
        }
    }

    public class MaterialEvaluator : Evaluator
    {
        protected int cannonWeight, pieceWeight;

        public MaterialEvaluator(int cannonWeight = 1, int pieceWeight = 1)
        {
            this.pieceWeight = pieceWeight;
            this.cannonWeight = cannonWeight;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            Position town = player == TileColor.Dark ? state.DarkTown : state.LightTown;
            Position otherTown = player == TileColor.Light ? state.DarkTown : state.LightTown;
            if (town == Constants.Removed)
            {
                return -100;
            }
            if (otherTown == Constants.Removed)
            {
                return 100;
            }

            int sign = player == TileColor.Dark ? 1 : -1;
            // Formula is from the Dark point of view -> switch sign for Light
            return sign * (
                (state.DarkCannons - state.LightCannons) * cannonWeight +
                (state.DarkPieces - state.LightPieces) * pieceWeight
            );
        }
    }

    public class MobilityEvaluator : Evaluator
    {
        protected MoveGenerator generator;

        public MobilityEvaluator() { }

        public MobilityEvaluator(MoveGenerator generator)
        {
            this.generator = generator;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            //If the opponent has few moves it is good
            return -generator.GenerateAll(state, Utils.SwitchColor(player)).Count;
        }
    }

    public class AggregateEvaluator : Evaluator
    {
        //Assume weight = 1
        protected Evaluator[] evals;

        public AggregateEvaluator(Evaluator[] evals)
        {
            this.evals = evals;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            int i = 0;
            foreach (Evaluator e in evals)
            {
                i += e.Evaluate(state, player);
            }
            return i;
        }
    }
}
