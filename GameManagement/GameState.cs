using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cannon_GUI
{
    /*
     * Represent a state of the game.
     * 
     * Contains the matrix representation of the board and the list of cannons.
     * The town are stored separately but they are also in the matrix.    
     */
    public class GameState : Loggable
    {
        // These 3 fields define a gamestate
        protected TileColor[,] occupied = new TileColor[Constants.Size, Constants.Size]; // look up table
        protected Position darkTown = new Position(3, 9); //TODO use correct values
        protected Position lightTown = new Position(3, 0);
        /// /////////////////

        //Ends of the cannons, can be selected
        protected List<Cannon> cannons = new List<Cannon>();
        protected int darkPieces, lightPieces, darkCannons, lightCannons; // counters
        protected int primHash, secmHash; // hash

        public TileColor[,] Occupied => occupied;
        public IList<Cannon> Cannons => cannons.AsReadOnly();

        public Position DarkTown { get => darkTown; set => new Position(value); }
        public Position LightTown { get => lightTown; set => new Position(value); }

        public int DarkPieces { get => darkPieces; }
        public int LightPieces { get => lightPieces; }
        public int DarkCannons { get => darkCannons; }
        public int LightCannons { get => lightCannons; }

        public int PrimaryHash => primHash;
        public int SecondaryHash => secmHash;

        // When the winner with without destroying the town
        // E.g. timeout, state repetition, etc...
        public TileColor ExternalWinner = TileColor.No;

        public GameState(TileColor[,] init, Position darkTown, Position lightTown)
        {
            Debug.Assert(init.GetLength(0) == Constants.Size, $"New board dim 0 is not {Constants.Size}");
            Debug.Assert(init.GetLength(1) == Constants.Size, $"New board dim 1 is not {Constants.Size}");
            //Array.Copy(init, occupied, Constants.Size * Constants.Size);    // 10x10
            occupied = init;
            this.darkTown = darkTown;
            this.lightTown = lightTown;
            //FindCannons();
            ComputeHash(); //TODO avoid computing hash from scratch

            // Pieces counters
            darkPieces = 0;
            lightPieces = 0;
            darkCannons = -1;
            lightCannons = -1;
            foreach (TileColor t in occupied)
            {
                if (t == TileColor.Dark)
                {
                    darkPieces++;
                }
                else if (t == TileColor.Light)
                {
                    lightPieces++;
                }
            }
        }

        public GameState(TileColor[,] init, Position darkTown, Position lightTown, ILog l) : this(init, darkTown, lightTown)
        {
            logger = l;
        }

        /*
         * Count how many cannons and update the list of cannons.
         * The cannon counters are -1 by default.   
         * 
         * Horizontal cannons are not counted (see MaterialEvaluator).
         */
        public void FindCannons()
        {
            //if (darkCannons == -1) //first time looking for cannons
            if (cannons.Count == 0)
            {
                for (int i = 0; i < Constants.Size; i++) // each x
                {
                    for (int j = 0; j < Constants.Size; j++) // each y
                    {
                        TileColor c = GetColor(i, j);
                        if (c != TileColor.No && !IsTown(i, j))
                        {
                            // Cannon: 3 pieces of the same player in line that are not towns

                            //Horizontal
                            if (IsValid(i, j + 2))
                            {
                                if (IsFriendly(i, j + 1, c) &&
                                    IsFriendly(i, j + 2, c) &&
                                    !(IsTown(i, j + 1) || IsTown(i, j + 2)))
                                {
                                    cannons.Add(new Cannon(new Position(i, j), new Position(i, j + 2)));
                                    /*if (c == TileColor.Dark)
                                        darkCannons++;
                                    else
                                        lightCannons++;*/
                                }
                            }

                            //Vertical
                            if (IsValid(i + 2, j))
                            {
                                if (IsFriendly(i + 1, j, c) &&
                                    IsFriendly(i + 2, j, c) &&
                                    !(IsTown(i + 1, j) || IsTown(i + 2, j)))
                                {
                                    cannons.Add(new Cannon(new Position(i, j), new Position(i + 2, j)));
                                    if (c == TileColor.Dark)
                                        darkCannons += 1;
                                    else
                                        lightCannons += 1;
                                }
                            }

                            //diagonal up
                            if (IsValid(i + 2, j + 2))
                            {
                                if (IsFriendly(i + 1, j + 1, c) &&
                                    IsFriendly(i + 2, j + 2, c) &&
                                    !(IsTown(i + 1, j + 1) || IsTown(i + 2, j + 2)))
                                {
                                    cannons.Add(new Cannon(new Position(i, j), new Position(i + 2, j + 2)));
                                    if (c == TileColor.Dark)
                                        darkCannons += 1;
                                    else
                                        lightCannons += 1;
                                }
                            }

                            //diagonal down
                            if (IsValid(i + 2, j - 2))
                            {
                                if (IsFriendly(i + 1, j - 1, c) &&
                                    IsFriendly(i + 2, j - 2, c) &&
                                    !(IsTown(i + 1, j - 1) || IsTown(i + 2, j - 2)))
                                {
                                    cannons.Add(new Cannon(new Position(i, j), new Position(i + 2, j - 2)));
                                    if (c == TileColor.Dark)
                                        darkCannons += 1;
                                    else
                                        lightCannons += 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         * Methods to get information about the current game state
         */
        #region query
        public bool IsFree(Position p)
        {
            return IsFree(p.x, p.y);
        }

        public bool IsFree(int x, int y)
        {
            return occupied[x, y] == TileColor.No;
        }

        public bool IsEnemy(int x, int y, TileColor player)
        {
            return occupied[x, y] == Utils.SwitchColor(player);
        }

        public bool IsEnemy(Position p, TileColor player)
        {
            return IsEnemy(p.x, p.y, player);
        }

        public static bool IsValid(int x, int y)
        {
            return x >= 0 && x < Constants.Size && y >= 0 && y < Constants.Size;
        }

        public static bool IsValid(Position p)
        {
            return IsValid(p.x, p.y);
        }

        public bool AdjacentEnemy(Position p, TileColor player)
        {
            bool adj = false;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (IsValid(p.x + i, p.y + j) && IsEnemy(p.x + i, p.y + j, player))
                    {
                        adj = true;
                        break;  //<- bad, TODO solve
                    }
                }
            }
            return adj;
        }

        public bool AdjacentEnemy(int x, int y, TileColor player)
        {
            return AdjacentEnemy(new Position(x, y), player);
        }

        public TileColor GetColor(Position p)
        {
            return GetColor(p.x, p.y);
        }

        public TileColor GetColor(int x, int y)
        {
            return occupied[x, y];
        }

        public bool IsTown(Position p)
        {
            return lightTown == p || darkTown == p;
        }

        public bool IsTown(int x, int y)
        {
            return IsTown(new Position(x, y));
        }

        public bool IsFriendly(Position p, TileColor player)
        {
            return IsFriendly(p.x, p.y, player);
        }

        public bool IsFriendly(int x, int y, TileColor player)
        {
            return occupied[x, y] == player;
        }

        public bool IsInCannon(Position p, out List<Cannon> o)
        {
            //check if the position is the head of a cannon
            //If so, return the list of cannons it is in
            bool found = false;
            o = new List<Cannon>();
            foreach (Cannon c in cannons)
            {
                if (IsInCannon(p, c))
                {
                    o.Add(c);
                    found = true;
                }
            }
            return found;
        }

        public bool IsInCannon(Position p, Cannon c)
        {
            //whether the given piece is the head of the given cannon
            return (c.head1 == p) || (c.head2 == p);
        }

        public static bool IsValidTown(Position p)
        {
            return p.x >= 1 && p.x < Constants.Size - 1 && (p.y == 0 || p.y == Constants.Size - 1);
        }
        #endregion

        public TileColor Winner()
        {
            if (ExternalWinner != TileColor.No)
                return ExternalWinner;
            if (darkTown == Constants.Removed)
                return TileColor.Light;
            if (lightTown == Constants.Removed)
                return TileColor.Dark;
            return TileColor.No;
        }

        public bool End()
        {
            // TODO handle draw/no moves

            return Winner() != TileColor.No;
        }

        /*
         * Apply a move to the current game state and return a new state.
         * The current state remains unchanged.        
         */
        public GameState Apply(Move m)
        {
            // If the game is over of if it is a null move the state is unchanged
            if (End())
            {
                return this;
            }
            if (m.Type == MoveType.none)
            {
                return this;
            }

            // Make a new matrix
            TileColor[,] newBoard = new TileColor[10, 10];
            Array.Copy(occupied, newBoard, 100);
            Position newDarkTown = darkTown;
            Position newLightTown = lightTown;
            switch (m.Type)
            {
                case MoveType.shoot:    // remove target piece
                    newBoard[m.To.x, m.To.y] = TileColor.No;
                    break;
                case MoveType.capture:
                case MoveType.retreat:
                case MoveType.slide:
                case MoveType.step:     // remove starting piece and overwrited destination piece
                    newBoard[m.To.x, m.To.y] = occupied[m.From.x, m.From.y];
                    newBoard[m.From.x, m.From.y] = TileColor.No;
                    break;
                case MoveType.placeTown:
                    //TODO find better way
                    if (m.To.y > Constants.Size / 2)
                    {
                        newBoard[m.To.x, m.To.y] = TileColor.Light;
                        newLightTown = m.To;
                    }
                    else
                    {
                        newBoard[m.To.x, m.To.y] = TileColor.Dark;
                        newDarkTown = m.To;
                    }
                    break;
            }

            if (m.Type != MoveType.placeTown) // do we destroy the town?
            {
                newDarkTown = m.To == newDarkTown ? Constants.Removed : newDarkTown;
                newLightTown = m.To == newLightTown ? Constants.Removed : newLightTown;
            }
            return new GameState(newBoard, newDarkTown, newLightTown, logger);
        }

        // This method is not used
        /*
         * Undo a move (see GameState.Apply)
         */
        public GameState Undo(Move m) //If the town is gone we cannot undo it -> was it a town or a normal soldier
        {
            logger.Log("Undo", m.ToString());

            if (lightTown == Constants.Removed || darkTown == Constants.Removed)
            {
                return this;
            }
            if (m.Type == MoveType.none)
            {
                return this;
            }
            TileColor[,] newBoard = new TileColor[10, 10];
            occupied.CopyTo(newBoard, 100);

            switch (m.Type)
            {
                case MoveType.shoot:
                    newBoard[m.To.x, m.To.y] = Utils.SwitchColor(GetColor(m.From));
                    break;
                case MoveType.capture:
                    newBoard[m.To.x, m.To.y] = Utils.SwitchColor(GetColor(m.From));
                    newBoard[m.From.x, m.From.y] = occupied[m.To.x, m.To.y];
                    break;
                case MoveType.retreat:
                case MoveType.slide:
                case MoveType.step:
                    newBoard[m.From.x, m.From.y] = occupied[m.To.x, m.To.y];
                    newBoard[m.To.x, m.To.y] = TileColor.No;
                    break;
                case MoveType.placeTown:
                    newBoard[m.To.x, m.To.y] = TileColor.No;
                    if (m.To.y > Constants.Size / 2)
                        lightTown = Constants.NotPlaced;
                    else
                        darkTown = Constants.NotPlaced;
                    break;
            }

            return new GameState(newBoard, darkTown, lightTown, logger);
        }

        protected void ComputeHash()
        {
            int h = 0;
            for (int i = 0; i < Constants.Size; i++)
            {
                for (int j = 0; j < Constants.Size; j++)
                {
                    h ^= Utils.hashBase[(Constants.Size * i + j) * (int)(occupied[i, j] + 1)];
                }
            }
            primHash = h >> (sizeof(int) * 8 - Constants.HashKeySize); //TODO what if hashkey size is larger than the int in th
            if (primHash == 0)
            {
                Console.WriteLine(this.ToString());
            }
            secmHash = h ^ (primHash << (sizeof(int) * 8 - Constants.HashKeySize)); //just the remainder //TODO TODO fix 0 division

        }
        #region Operators

        // Assume comparision between states of the same game -> towns in the same position
        public static bool operator ==(GameState state, GameState other)
        {
            if (other is null)
            {
                return state is null;
            }
            for (int i = 0; i < Constants.Size; i++)
            {
                for (int j = 0; j < Constants.Size; j++)
                {
                    if (state.occupied[i, j] != other.occupied[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator !=(GameState state, GameState other)
        {
            return !(state == other);
        }
        #endregion

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(new string('-', Constants.Size * 2 + 2));
            sb.Append('\n');
            for (int i = 0; i < Constants.Size; i++)
            {
                sb.Append('|');
                for (int j = 0; j < Constants.Size; j++)
                {
                    switch (occupied[j, i])
                    {
                        case TileColor.Dark:
                            sb.Append('d');
                            break;
                        case TileColor.Light:
                            sb.Append('l');
                            break;
                        default:
                            sb.Append('.');
                            break;
                    }
                    sb.Append(' ');
                }
                sb.Append('|');
                sb.Append('\n');
            }
            sb.Append(new string('-', Constants.Size * 2 + 2));

            return sb.ToString();
        }
    }

}
