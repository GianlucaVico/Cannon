using System;
using System.Collections.Generic;

namespace Cannon_GUI
{
    public static class Constants
    {
        //Size of the board in tiles, assume squared board
        public const int Size = 10;

        //Distance from the boarder of the image, in pixels
        public const int BoardOffSet = 30;
        //Length size of the tile in the background image, in pixels
        public const int TileSize = 70;

        public const int CannonLength = 3;

        public static readonly Position Removed = new Position(-1, -1);
    }

    public static class Utils
    {
        public static TileColor SwitchColor(TileColor color)
        {
            return (color == TileColor.Dark ? TileColor.Light : TileColor.Dark);
        }
    }

    public enum TileColor
    {
        No,
        Dark,
        Light,
    }

    public enum TileType
    {
        Empty,
        Piece,
        Town
    }

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
        public override string ToString() => $"Position ({x}-{y})";

        public static bool operator ==(Position p1, Position p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Position p1, Position p2)
        {
            return !(p1 == p2);
        }
    }

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
