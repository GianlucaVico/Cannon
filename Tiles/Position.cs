namespace Cannon_GUI
{
    /*
     * A position on the board.
     */
    public struct Position
    {
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Position(Position p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        public readonly int x, y;
        public override string ToString() => $"Position ({(char)(x + Constants.AlphabetStart)}-{y + 1})";

        public static bool operator ==(Position p1, Position p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Position p1, Position p2)
        {
            return !(p1 == p2);
        }
    }
}
