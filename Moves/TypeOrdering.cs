using System.Collections.Generic;

namespace Cannon_GUI
{
    /*
     * Sor the moves only by type. Moves whose target is a town are considered
     * the strongest.    
     */
    public class TypeOrdering : IMoveOrdering
    {
        public List<Move> Sort(List<Move> moves, GameState state, int depth)
        {
            //First larger: 1 - Same: 0 - Second larger: -1
            moves.Sort((x, y) =>
            {
                if (x.To == state.DarkTown || x.To == state.LightTown) //We like capturing towns
                    return -1;
                if (y.To == state.DarkTown || y.To == state.LightTown) //We like capturing towns
                    return 1;
                return x.Type.CompareTo(y.Type);
            });
            return moves;
        }
    }


}
