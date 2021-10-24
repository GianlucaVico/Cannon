namespace Cannon_GUI
{
    /*
     * Structure representing a cannon.
     * 
     * It stores both ends of a cannon
     */
    public struct Cannon
    {
        public readonly Position head1, head2;

        public Cannon(Position h1, Position h2)
        {
            head1 = h1;
            head2 = h2;
        }

        public static bool operator ==(Cannon c1, Cannon c2)
        {
            return (c1.head1 == c2.head1 && c1.head2 == c2.head2) || (c1.head1 == c2.head2 && c1.head2 == c2.head1);
        }

        public static bool operator !=(Cannon c1, Cannon c2)
        {
            return !(c1 == c2);
        }

        public override string ToString() => $"From {head1} - To {head2}";
    }
}
