using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cannon_GUI
{
    public class GameState : Loggable
    {
        // These 3 fields define a gamestate
        protected TileColor[,] occupied = new TileColor[Constants.Size, Constants.Size]; // look up table
        protected Position darkTown = new Position(3, 9); //TODO use correct values
        protected Position lightTown = new Position(3, 0);
        /// /////////////////

        //Ends of the cannons, can be selected
        //protected TileColor[,] cannonHead = new TileColor[Constants.Size, Constants.Size]; // look up table
        protected List<Cannon> cannons = new List<Cannon>();
        protected int darkPieces, lightPieces, darkCannons, lightCannons; // counters
        protected int hashKey;

        public TileColor[,] Occupied => occupied;
        public IList<Cannon> Cannons => cannons.AsReadOnly();

        public Position DarkTown { get => darkTown; set => new Position(value); }
        public Position LightTown { get => lightTown; set => new Position(value); }

        public int DarkPieces { get => darkPieces; }
        public int LightPieces { get => lightPieces; }
        public int DarkCannons { get => darkCannons; }
        public int LightCannons { get => lightCannons; }

        public int HashKey => hashKey;

        // When the winner with without destroying the town
        // E.g. timeout, state repetition, etc...
        public TileColor ExternalWinner = TileColor.No;

        public GameState(TileColor[,] init, Position darkTown, Position lightTown)
        {
            Debug.Assert(init.GetLength(0) == Constants.Size, "New board dim 0 is not 10");
            Debug.Assert(init.GetLength(1) == Constants.Size, "New board dim 1 is not 10");
            Array.Copy(init, occupied, Constants.Size * Constants.Size);    // 10x10
            this.darkTown = darkTown;
            this.lightTown = lightTown;
            FindCannons();
            ComputeHash();

            // Pieces counters
            darkPieces = 0;
            lightPieces = 0;
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

            // Cannons counters
            darkCannons = 0;
            lightCannons = 0;
            foreach (Cannon c in cannons)
            {
                if (occupied[c.head1.x, c.head1.y] == TileColor.Dark)
                {
                    darkCannons++;
                }
                else if (occupied[c.head1.x, c.head1.y] == TileColor.Light)
                {
                    lightCannons++;
                }
            }
        }

        public GameState(TileColor[,] init, Position darkTown, Position lightTown, Log l) : this(init, darkTown, lightTown)
        {
            logger = l;
        }

        /*public void Update(TileColor[,] newBoard)
        {
            Array.Copy(newBoard, occupied, Constants.Size * Constants.Size);
            FindCannons();
        }*/

        protected void FindCannons()
        {
            // for each soldier check the neightbours
            // it is enogh to check the RHS size of each soldier -> cannons from the other side have already been checked
            // |   /     x      
            // |  /       \
            // x x   x--   \
            // x: current soldier, |/-\: others
            // Check only for the cannon displayed above
            Position other, current;
            int cannon = Constants.CannonLength; // check all the elements in the cannon
            bool feasible = true;   // we can still make a cannon
            //TODO stop earler to exclude pieces that are no part of a cannon (e.g. in the corner)
            for (int i = 0; i < Constants.Size; i++) // each x
            {
                for (int j = 0; j < Constants.Size; j++) // each y
                {
                    current = new Position(i, j);
                    if (!IsFree(current))
                    { // start from a piece
                        for (int dx = 0; dx <= 1; dx++) // orientation on x
                        {
                            for (int dy = -1; dy <= 1; dy++) // orientation on y
                            {
                                if (!(dx == 0 && dy == 0) && !(dx == 0 && dy == -1)) // excluse some directions
                                {
                                    cannon = Constants.CannonLength - 1; // check other end of the cannon
                                    feasible = true; // we can still make this cannon
                                    while (cannon > 0 && feasible)
                                    {
                                        other = new Position(i + dx * cannon, j + dy * cannon);
                                        //the other position is not ok
                                        /*if (
                                            !IsValid(other) ||
                                            IsEnemy(other, GetColor(current)) ||
                                            IsFree(other) ||
                                            IsTown(other)
                                        )*/
                                        if (!(IsValid(other) && IsFriendly(other, GetColor(current)) && !IsTown(other))) //No need to investigate this 
                                        {
                                            feasible = false;
                                        }
                                        cannon--;
                                    }
                                    if (feasible)
                                    {
                                        other = new Position(i + dx * (Constants.CannonLength - 1), j + dy * (Constants.CannonLength - 1));
                                        cannons.Add(new Cannon(current, other));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Console.WriteLine($"Cannons found: {cannons.Count}");
        }

        #region query
        public bool IsFree(Position p)
        {
            return occupied[p.x, p.y] == TileColor.No;
        }

        public bool IsEnemy(Position p, TileColor player)
        {
            return occupied[p.x, p.y] == Utils.SwitchColor(player);
        }
        public static bool IsValid(Position p)
        {
            return p.x >= 0 && p.x < Constants.Size && p.y >= 0 && p.y < Constants.Size;
        }

        public bool AdjacentEnemy(Position p, TileColor player)
        {
            bool adj = false;
            Position tmp;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (j != 0 && i != 0)  //could skip -> we are looking for the other color
                    {
                        tmp = new Position(p.x + i, p.y + j);
                        if (IsValid(tmp) && IsEnemy(tmp, player))
                        {
                            adj = true;
                            break;  //<- bad, TODO solve
                        }
                    }
                }
            }
            return adj;
        }

        public TileColor GetColor(Position p)
        {
            return occupied[p.x, p.y];
        }

        public bool IsTown(Position p)
        {
            return lightTown == p || darkTown == p;
        }

        public bool IsFriendly(Position p, TileColor player)
        {
            return occupied[p.x, p.y] == player;
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

        public GameState Apply(Move m)
        {
            if (End())
                return this;
            //logger.Log("Apply", m.ToString());
            if (m.Type == MoveType.none)
            {
                return this;
            }
            TileColor[,] newBoard = new TileColor[10, 10];
            Array.Copy(occupied, newBoard, 100);

            switch (m.Type)
            {
                case MoveType.shoot:
                    newBoard[m.To.x, m.To.y] = TileColor.No;
                    break;
                case MoveType.capture:
                case MoveType.retreat:
                case MoveType.slide:
                case MoveType.step:
                    newBoard[m.To.x, m.To.y] = occupied[m.From.x, m.From.y];
                    newBoard[m.From.x, m.From.y] = TileColor.No;
                    break;
                case MoveType.placeTown:
                    //TODO find better way
                    if (m.To.y > Constants.Size / 2)
                    {
                        newBoard[m.To.x, m.To.y] = TileColor.Light;
                        lightTown = m.To;
                    }
                    else
                    {
                        newBoard[m.To.x, m.To.y] = TileColor.Dark;
                        darkTown = m.To;
                    }
                    break;
            }

            if (m.Type != MoveType.placeTown) // do we destroy the town?
            {
                darkTown = m.To == darkTown ? Constants.Removed : darkTown;
                lightTown = m.To == lightTown ? Constants.Removed : lightTown;
            }
            //FindCannons();
            return new GameState(newBoard, darkTown, lightTown, logger);
        }

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
            hashKey = (h >> 32); //TODO use most significant bits

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
    }

}
