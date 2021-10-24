using System.Diagnostics;
using System.Collections.Concurrent;
namespace Cannon_GUI
{
    /*
     * Transposition talbe.
     * Use primary hash of the game state as key, and the secondary to check for
     * collisions.
     * 
     * On collision, store the deepest move
     */
    public class TT // +5% pruned/explored
    {
        protected ConcurrentDictionary<int, TTEntry> table;
        protected int size;

        public int Size { get => size; }

        public TT(int size)
        {
            this.size = size;
            table = new ConcurrentDictionary<int, TTEntry>(2, size);
        }


        /*
         * Get the TTEntry for the given game state if it is in the table
         * 
         * Args:
         *  state (GameState): state to retreave
         *  out result (TTEntry): values in the table for the state. 
         *      NullTTEntry if the state is not in the table
         * Returns:
         *  bool: whether the state was in the table
         */
        public bool Query(GameState state, out TTEntry result)
        {
            bool inTable = table.TryGetValue(state.PrimaryHash, out result);

            // Is it in the table?
            if (inTable)
            {
                if (result.hashKey != state.SecondaryHash)
                {
                    result = Constants.NullTTEntry;
                    return false;
                }
                return true; // correctly retrieved
            }
            else
            {
                result = Constants.NullTTEntry;
                return false;
            }
        }

        /*
         * Add a state to the table
         * 
         * Args:
         *  state (GameState): state to add
         *  type (TTType): type of the value
         *  bestMove (Move): best move for this state
         *  value (int): value for this state
         *  depth (int): depth in the tree for this state
         * Returns:
         *  bool: wheter it was possible to add the state to the table        
         */
        public bool Push(GameState state, TTType type, Move bestMove, int value, int depth)
        {
            if (table.Count > size)
            {
                Debug.WriteLine("Full TT");
                return false;
            }
            return Push(state, new TTEntry(type, bestMove, value, depth, state.SecondaryHash)); //no collisions
        }

        protected bool Push(GameState state, TTEntry entry)
        {
            bool inTable = table.TryGetValue(state.PrimaryHash, out TTEntry tmp);

            if (inTable)
            {
                if ((tmp != entry) && (tmp.depth >= entry.depth))
                {
                    //Console.WriteLine($"OVERWRITE: {state.PrimaryHash} - {tmp.depth} >= {entry.depth} - {tmp.value} <-> {entry.value}");
                    table[state.PrimaryHash] = entry;
                }
                return false;
            }
            else
            {
                table[state.PrimaryHash] = entry;
                return true;
            }
        }

        public void Clear()
        {
            table.Clear();
        }

    }
}
