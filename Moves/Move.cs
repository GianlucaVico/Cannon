// The retreat move assumes that the soldier has to be adjacent to an enemy, 
// but that enemy might not able to campure it.
namespace Cannon_GUI
{
    /*
     * Structure representin a move. 
     * It contains the type of the move, its starting position and the end position.    
     */
    public struct Move
    {
        public readonly Position From, To;
        public readonly MoveType Type;

        public Move(Move m)
        {
            From = new Position(m.From);
            To = new Position(m.To);
            Type = m.Type;
        }

        public Move(Position from, Position to, MoveType type)
        {
            From = from;
            To = to;
            Type = type;
        }

        public override string ToString()
        {
            return $"Move {Type} | {From} -> {To}";
        }

        public static bool operator ==(Move m, Move other)
        {
            return (m.Type == other.Type) && (m.From == other.From) && (m.To == other.To);
        }

        public static bool operator !=(Move m, Move other)
        {
            return !(m == other);
        }
    }
}
