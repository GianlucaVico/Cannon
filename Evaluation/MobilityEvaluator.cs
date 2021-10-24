namespace Cannon_GUI
{
    /*
     * Evaluator that counts the number of moves to evaluate the state of the 
     * game.
     *    
     * The difference of moves between the player and the opponent is in 
     * general small and it does not really distinguish a good state from a bad
     * one. 
     * So this evaluator simply uses the number of moves of the opponent.
     * Few moves is good for the player.    
     */
    public class MobilityEvaluator : IEvaluator
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
            int countOpponent = generator.GenerateAll(state, Utils.SwitchColor(player)).Count;
            /*int countPlayer = generator.GenerateAll(state, player).Count;
            if (countPlayer == 0)
                return -1000;
            else if (countOpponent == 0)
                return 1000;
            else
                return countPlayer - countOpponent;*/
            return -countOpponent; //faster than diff with player
        }
    }
}
