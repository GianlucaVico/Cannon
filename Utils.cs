using System;
using System.Runtime.InteropServices;

namespace Cannon_GUI
{
    /*
     * Set of constants used in the program
     */
    public static class Constants
    {
        //Size of the board in tiles, assume squared board
        public const int Size = 10;

        //Distance from the boarder of the image, in pixels
        public const int BoardOffSet = 30;
        //Length size of the tile in the background image, in pixels
        public const int TileSize = 70;

        //public const int CannonLength = 3;

        public static readonly Position Removed = new Position(-1, -1);
        public static readonly Position NotPlaced = new Position(10, 10);

        public const int AlphabetStart = 65; //65 -> 'A'

        public static readonly TimeSpan MaxTime = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan MaxIteration = TimeSpan.FromSeconds(5);

        public static readonly Move NullMove = new Move(Removed, Removed, MoveType.none);

        public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static string Slash = IsWindows ? "\\" : "/";
        //Gtk# does not support svg images on windows °-° 
        public static string ImgExt = IsWindows ? "png" : "svg";

        public const int ClockResolution = 100; //The clock is ticking...
        public static int HashKeySize = IsWindows ? 28 : 29; // Size of the primary hash in bits, set to 28 on windows
        public static readonly TTEntry NullTTEntry = new TTEntry(TTType.none, NullMove, 0, -1, 0);

        public const int HistTableSize = 1000; // about 81 starting positions * 12 destinations for each position
        public const int KillerTableSize = 10; // 10 plyes should be enough

        public const int Extension = 1; // search deeper with capture moves
    }

    /*
     * Class with static methods used in the program
     */
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

}
