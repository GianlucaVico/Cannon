namespace Cannon_GUI
{
    /*
     * Evaluator that always return a constant value
     */
    public class ConstantEvaluator : IEvaluator //placeholder
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
}
