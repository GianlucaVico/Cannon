using System;
using System.Collections.Generic;
using System.Linq;

namespace Cannon_GUI
{
    public interface MoveOrdering
    {
        List<Move> Sort(List<Move> moves);
        List<Move> Sort(List<Move> moves, GameState state);
    }

    public class RandomOrder : MoveOrdering
    {
        protected Random rnd;

        public RandomOrder()
        {
            rnd = new Random();
        }

        public RandomOrder(int seed)
        {
            rnd = new Random(seed);
        }

        public List<Move> Sort(List<Move> moves)
        {
            return moves.OrderBy(i => rnd.Next()).ToList();
        }

        public List<Move> Sort(List<Move> moves, GameState state)
        {
            return Sort(moves);
        }
    }

    public class TypeOrder : MoveOrdering
    {
        public List<Move> Sort(List<Move> moves)    // In place
        {
            //First larger: 1 - Same: 0 - Second larger: -1
            moves.Sort((x, y) => x.Type.CompareTo(y.Type));
            return moves;
        }

        public List<Move> Sort(List<Move> moves, GameState state)
        {
            //First larger: 1 - Same: 0 - Second larger: -1
            moves.Sort((x, y) =>
            {
                if (x.To == state.DarkTown || x.To == state.LightTown) //We like capturing towns
                    return 1;
                if (y.To == state.DarkTown || y.To == state.LightTown) //We like capturing towns
                    return -1;
                return x.Type.CompareTo(y.Type);
            });
            return moves;
        }
    }
}
