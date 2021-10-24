namespace Cannon_GUI
{
    /*
     * Killer moves. Store the last move that generated a prune for each layer
     * of the search tree.
     * 
     * It is used only for a predefined number of layers, but generally the 
     * search algorithm does not search that deep.
     */
    public class KT
    {
        protected Move[] moves;

        public KT(int size)
        {
            moves = new Move[size];
            for (int i = 0; i < size; i++)
            {
                moves[i] = Constants.NullMove;
            }
        }

        /*
         * Get move at depth d
         */
        public Move Query(int d)
        {
            if (d < moves.Length && d >= 0)
            {
                return moves[d];
            }
            else
            {
                return Constants.NullMove;
            }
        }

        /*
         * Store move for depth d
         */
        public void Update(Move m, int d)
        {
            //Debug.Assert(d >= 0, "Negative depth in killer table update");
            if (d < moves.Length && d >= 0) // Ignore deeper killer moves
            {
                moves[d] = m;
            }
        }
    }
}
