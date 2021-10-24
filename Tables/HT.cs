using System.Collections.Concurrent;

namespace Cannon_GUI
{
    /*
     * History table. It counts how many times we have seen a move.
     */
    public class HT
    {
        protected ConcurrentDictionary<Move, int> table;

        public HT(int size)
        {
            table = new ConcurrentDictionary<Move, int>(2, size);
        }

        public HT()
        {
            table = new ConcurrentDictionary<Move, int>();
        }

        /*
         * Get count for a move
         */
        public int Query(Move m)
        {
            if (table.TryGetValue(m, out int value))
            {
                return value;
            }
            return 0;
        }

        /*
         * Update counter for a move
         */
        public void Update(Move m)
        {
            if (table.TryGetValue(m, out int value))
            {
                table[m]++; // in table
            }
            else
            {
                table[m] = 1;
            }
        }
    }
}
