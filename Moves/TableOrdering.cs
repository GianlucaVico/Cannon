using System;
using System.Collections.Generic;

namespace Cannon_GUI
{
    /*
     * Order the moves by using killer moves and history tables.
     * 
     * At the end history tables have not  been used, because they require to
     * much time and did not give any advantage.    
     */
    public class TableOrdering : IMoveOrdering
    {
        //protected TT tt; //transposition table -> done in the search class
        protected HT ht; //history table
        protected KT kt; //killer moves

        public TableOrdering(HT ht, KT kt)
        {
            //this.tt = tt;
            this.ht = ht;
            this.kt = kt;
        }

        public List<Move> Sort(List<Move> moves, GameState state, int depth)
        {
            // Capture towns
            // (TT table) -> implicitly done by the A-B search
            // Killer moves
            // Type sorting
            // History table (between moves of the same type)

            moves.Sort((x, y) =>
            {
                if (x == y)
                    return 0;
                if (x.Type == MoveType.placeTown && y.Type == MoveType.placeTown)
                {
                    //town in the corner is stronger
                    return -(Math.Abs(x.To.x - Constants.Size / 2).CompareTo(Math.Abs(y.To.x - Constants.Size / 2)));
                }
                if (x.To == state.DarkTown || x.To == state.LightTown)
                    return -1;
                if (y.To == state.DarkTown || y.To == state.LightTown)
                    return 1;

                if (kt.Query(depth) == x)
                    return -1;
                if (kt.Query(depth) == y)
                    return 1;

                // 30 seconds extra
                /*if (x.Type == y.Type)
                {
                    return ht.Query(x).CompareTo(ht.Query(y));
                }*/

                return x.Type < y.Type ? -1 : 1;
                //return x.Type.CompareTo(y.Type);
            });
            return moves;
        }
    }


}
