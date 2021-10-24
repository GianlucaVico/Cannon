using System;
using System.Collections.Generic;
using System.Linq;

namespace Cannon_GUI
{
    /*
     * Order the moves randomly
     */
    public class RandomOrdering : IMoveOrdering
    {
        protected Random rnd;

        public RandomOrdering()
        {
            rnd = new Random();
        }

        public RandomOrdering(int seed)
        {
            rnd = new Random(seed);
        }

        public List<Move> Sort(List<Move> moves, GameState state, int depth)
        {
            return moves.OrderBy(i => rnd.Next()).ToList();
        }
    }


}
