namespace Cannon_GUI
{
    /*
     * Linear combination of multiple evaluators
     */
    public class AggregateEvaluator : IEvaluator
    {
        protected IEvaluator[] evals; // Evaluators
        protected int[] weights; // Weight of each evaluator
        public AggregateEvaluator(IEvaluator[] evals)
        {
            this.evals = evals;
            weights = new int[evals.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = 1;
            }
        }

        public AggregateEvaluator(IEvaluator[] evals, int[] weights)
        {
            this.evals = evals;
            this.weights = weights;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            int e = 0;
            for (int i = 0; i < evals.Length; i++)
            {
                e += evals[i].Evaluate(state, player) * weights[i];
            }
            return e;
        }
    }
}
