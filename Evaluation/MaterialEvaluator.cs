namespace Cannon_GUI
{
    /*
     * Evaluate the state of the game based on the pieces on the board.
     * 
     * Cannons have an higher weight, however horizontal cannons are not counted
     * (see GameState implementation).    
     */
    public class MaterialEvaluator : IEvaluator
    {
        protected int cannonWeight, pieceWeight;

        public MaterialEvaluator(int cannonWeight = 1, int pieceWeight = 2)
        {
            this.pieceWeight = pieceWeight;
            this.cannonWeight = cannonWeight;
        }

        public int Evaluate(GameState state, TileColor player)
        {
            state.FindCannons(); // avoid using -1 as cannon count
            Position town = player == TileColor.Dark ? state.DarkTown : state.LightTown;
            Position otherTown = player == TileColor.Light ? state.DarkTown : state.LightTown;
            if (town == Constants.Removed || town == Constants.NotPlaced)
            {
                //Debug.WriteLine($"Possible Loss - {player}");
                return -1000;
            }
            if (otherTown == Constants.Removed || town == Constants.NotPlaced)
            {
                return 1000;
            }

            int sign = player == TileColor.Dark ? 1 : -1;
            // Formula is from the Dark point of view -> switch sign for Light
            return sign * (
                (state.DarkCannons - state.LightCannons) * cannonWeight +
                (state.DarkPieces - state.LightPieces) * pieceWeight
            );
        }
    }
}
