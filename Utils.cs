using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections;

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
        public static readonly Position NotPlaced = new Position(10, 10);

        public const int AlphabetStart = 65; //65 -> 'A'

        public static readonly TimeSpan MaxTime = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan MaxIteration = TimeSpan.FromSeconds(1);

        public static readonly Move NullMove = new Move(Removed, Removed, MoveType.none);

        public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static string Slash = IsWindows ? "\\" : "/";
        //Gtk# does not support svg images on windows
        public static string ImgExt = IsWindows ? "png" : "svg";
    }

    public static class Utils
    {
        public static readonly int[] hashBase;
        private static readonly Random rnd;

        static Utils()
        {
            rnd = new Random(0);
            hashBase = new int[(Constants.Size * Constants.Size) * 3 + 2]; //100 positions * 3 values + 2 player colors
            for (int i = 0; i < hashBase.Length; i++)
            {
                hashBase[i] = rnd.Next();
            }
        }

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
