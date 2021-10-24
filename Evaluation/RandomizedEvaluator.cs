using System;
using System.Diagnostics;
namespace Cannon_GUI
{
    /*
     * Add a random bias to another evaluator 
     */
    public class RandomizedEvaluator : IEvaluator
    {
        protected Random rnd;
        protected IEvaluator baseEvaluator;
        protected int halfRange;  // the bias is in the range [-halfRange, +halfRange]

        public RandomizedEvaluator() : this(new ConstantEvaluator(), 5, 0) { }

        public RandomizedEvaluator(IEvaluator baseEvaluator, int halfRange) : this(baseEvaluator, halfRange, 0) { }

        public RandomizedEvaluator(IEvaluator baseEvaluator, int halfRange, int seed)
        {
            Debug.Assert(halfRange >= 0, "The half range must be non-negative");
            rnd = new Random(seed);
            this.baseEvaluator = baseEvaluator;
            this.halfRange = halfRange;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            return baseEvaluator.Evaluate(state, player) + rnd.Next(-halfRange, halfRange); ;
        }
    }
}
