using System.Collections.Generic;

namespace Cannon_GUI
{
    /*
     * Sort the moves
     */
    public interface IMoveOrdering
    {
        List<Move> Sort(List<Move> moves, GameState state, int depth);
    }
}
