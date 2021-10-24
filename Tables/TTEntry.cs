namespace Cannon_GUI
{
    /*
     * Element in the TT
     */
    public struct TTEntry
    {
        public int value; //value of the game state
        public TTType type; // type of the value
        public Move bestMove; // which move we should perform
        public int depth; // depth of the node
        public int hashKey; // secondary key

        public TTEntry(TTType type, Move bestMove, int value, int depth, int hashKey)
        {
            this.type = type;
            this.value = value;
            this.depth = depth;
            this.hashKey = hashKey;
            this.bestMove = bestMove;
        }

        public static bool operator ==(TTEntry t, TTEntry other)
        {
            return t.value == other.value &&
                   t.type == other.type &&
                   (t.bestMove == other.bestMove) &&
                   t.depth == other.depth &&
                   t.hashKey == other.hashKey;
        }

        public static bool operator !=(TTEntry t, TTEntry other)
        {
            return t.value != other.value ||
                   t.type != other.type ||
                   (t.bestMove != other.bestMove) ||
                   t.depth != other.depth ||
                   t.hashKey != other.hashKey;
        }
    }
}
