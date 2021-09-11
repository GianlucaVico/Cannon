using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cannon_GUI
{
    public class GameState
    {
        // These 3 fields define a gamestate
        protected TileColor[,] occupied = new TileColor[Constants.Size, Constants.Size]; // look up table
        protected Position darkTown = new Position(3, 9); //TODO use correct values
        protected Position lightTown = new Position(3, 0);
        /// /////////////////

        //Ends of the cannons, can be selected
        //protected TileColor[,] cannonHead = new TileColor[Constants.Size, Constants.Size]; // look up table
        protected List<Cannon> cannons = new List<Cannon>();

        public TileColor[,] Occupied => occupied;
        public IList<Cannon> Cannons => cannons.AsReadOnly(); 

        public Position DarkTown { get => darkTown; set => new Position(value); }
        public Position LightTown { get => lightTown; set => new Position(value); }

        public GameState()
        {

        }

        public GameState(TileColor[,] init, Position darkTown, Position lightTown)
        {
            Debug.Assert(init.GetLength(0) == Constants.Size, "New board dim 0 is not 10");
            Debug.Assert(init.GetLength(1) == Constants.Size, "New board dim 1 is not 10");
            Array.Copy(init, occupied, Constants.Size * Constants.Size);    // 10x10
            this.darkTown = darkTown;
            this.lightTown = lightTown;
            FindCannons();
        }

        public void Update(TileColor[,] newBoard) {
            Array.Copy(newBoard, occupied, Constants.Size * Constants.Size);
            FindCannons();
        }

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
            for(int i = 0; i < Constants.Size; i++) // each x
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
                                        if (
                                            !IsValid(other) ||
                                            IsEnemy(other, GetColor(current)) || 
                                            IsFree(other) || 
                                            IsTown(other)
                                        ) //No need to investigate this 
                                        {
                                            feasible = false;
                                        }
                                        cannon--;
                                    }
                                    if(feasible)
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
            Console.WriteLine($"Cannons found: {cannons.Count}");
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
                        if(IsValid(tmp) && IsEnemy(tmp, player))
                        {
                            adj = true;
                            break;  //<- bad, TODO solve
                        }
                    }
                }
            }
            return adj;
        }

        public TileColor GetColor(Position p) {
            return occupied[p.x, p.y];
        }

        public bool IsTown(Position p)
        {
            return lightTown == p || darkTown == p;
        }

        public bool IsFriendly(Position p, TileColor player) {
            return occupied[p.x, p.y] == player;
        }

        public bool IsInCannon(Position p, out List<Cannon> o)
        {
            //check if the position is the head of a cannon
            //If so, return the list of cannons it is in
            bool found = false;
            o = new List<Cannon>();
            foreach(Cannon c in cannons)
            {
                if(IsInCannon(p, c))
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
        #endregion

        public TileColor Winner()
        {
            if (darkTown == Constants.Removed)
                return TileColor.Light;
            if (lightTown == Constants.Removed)
                return TileColor.Dark;
            return TileColor.No;
        }

        public GameState Apply(Move m) {
            Console.WriteLine("Applying action");
            Console.WriteLine(m);
            if(m.Type == MoveType.none)
            {
                return this;
            }
            TileColor[,] newBoard = new TileColor[10, 10];
            Array.Copy(occupied, newBoard, 100);
            if (m.Type == MoveType.shoot)
            {
                // Destroy target
                newBoard[m.To.x, m.To.y] = TileColor.No; 
            }
            else
            {
                // Move piece and remove it from the original position
                newBoard[m.To.x, m.To.y] = occupied[m.From.x, m.From.y];
                newBoard[m.From.x, m.From.y] = TileColor.No;
            }

            darkTown = m.To == darkTown ? Constants.Removed : darkTown;
            lightTown = m.To == lightTown ? Constants.Removed : lightTown;
            return new GameState(newBoard, darkTown, lightTown);
        }

        public GameState Undo(Move m) //If the town is gone we cannot undo it -> was it a town or a normal soldier
        {
            if(lightTown == Constants.Removed || darkTown == Constants.Removed)
            {
                return this;
            }
            Console.WriteLine("Undoing action");
            Console.WriteLine(m);
            if (m.Type == MoveType.none)
            {
                return this;
            }
            TileColor[,] newBoard = new TileColor[10, 10];
            occupied.CopyTo(newBoard, 100);

            if (m.Type == MoveType.shoot)
            {
                // The target was an enemy
                newBoard[m.To.x, m.To.y] = Utils.SwitchColor(GetColor(m.From));
            }
            else if(m.Type == MoveType.capture)
            {
                // Move back and recreate enemy
                newBoard[m.To.x, m.To.y] = Utils.SwitchColor(GetColor(m.From));
                newBoard[m.From.x, m.From.y] = occupied[m.To.x, m.To.y];
            }
            else
            {
                newBoard[m.From.x, m.From.y] = occupied[m.To.x, m.To.y];
                newBoard[m.To.x, m.To.y] = TileColor.No;
            }

            return new GameState(newBoard, darkTown, lightTown);
        }
    }

}
