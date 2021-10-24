namespace Cannon_GUI
{
    /*
     * Interface for an evaluator.
     */
    public interface IEvaluator
    {
        int Evaluate(GameState state, TileColor player);
    }
}
