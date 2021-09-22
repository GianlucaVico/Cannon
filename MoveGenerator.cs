using System;
using System.Collections.Generic;
// The retreat move assumes that the soldier has to be adjacent to an enemy, 
// but that enemy might not able to campure it.
namespace Cannon_GUI
{
    public class MoveGenerator
    {

        public List<Move> Generate(GameState state, Tile tile) //for human player
        {
            return Generate(state, tile.Position, tile.Color);
        }

        public List<Move> Generate(GameState state, Position pos, TileColor player) // for single piece
        {
            List<Move> moves = new List<Move>(9); // TODO count moves
            Generate(state, pos, player, ref moves);
            return moves;
        }

        protected void Generate(GameState state, Position pos, TileColor player, ref List<Move> moves)
        {
            int direction = player == TileColor.Dark ? +1 : -1; //multiply to change direction

            //Captures
            GenerateCaptures(state, pos, player, direction, ref moves);

            //slide
            //shoot
            GenerateCannonMoves(state, pos, player, direction, ref moves);

            //Steps
            GenerateSteps(state, pos, player, direction, ref moves);


            //retreats
            GenerateRetreats(state, pos, player, direction, ref moves);
        }


        public List<Move> GenerateAll(GameState state, TileColor player)
        {
            Position town = player == TileColor.Dark ? state.DarkTown : state.LightTown;
            List<Move> moves = new List<Move>();
            if (town == Constants.NotPlaced)
            {
                moves = GenerateTownPlacements(state, player);
            }
            else if (town != Constants.Removed)
            {
                Position p;
                for (int i = 0; i < Constants.Size; i++)
                {
                    for (int j = 0; j < Constants.Size; j++)
                    {
                        p = new Position(i, j);
                        if (state.IsFriendly(p, player) && !state.IsTown(p)) // cannot move town :'(
                        {
                            Generate(state, p, player, ref moves);
                        }
                    }
                }
            }
            if (moves.Count == 0) // This player does not have any move
                state.ExternalWinner = Utils.SwitchColor(player);
            return moves;
        }

        public List<Move> GenerateTownPlacements(GameState state, TileColor player)
        {
            List<Move> moves = new List<Move>(Constants.Size);
            int y = player == TileColor.Dark ? 0 : Constants.Size - 1;
            for (int x = 1; x < Constants.Size - 1; x++) //exclude corners
            {
                moves.Add(new Move(Constants.NotPlaced, new Position(x, y), MoveType.placeTown));
            }
            return moves;
        }

        #region MoveGenerators
        protected void GenerateSteps(GameState state, Position pos, TileColor player, int direction, ref List<Move> currentMoves)
        {
            Position dest;

            //step
            for (int i = -1; i < 2; i++) //3 positions in front
            {
                dest = new Position(pos.x + i, pos.y + direction);
                if (GameState.IsValid(dest) && state.IsFree(dest))
                {
                    currentMoves.Add(new Move(pos, dest, MoveType.step));
                }
            }
        }

        protected void GenerateCaptures(GameState state, Position pos, TileColor player, int direction, ref List<Move> currentMoves)
        {
            Position dest;
            for (int i = -1; i < 2; i++) // 3 positions in front
            {
                for (int j = 0; j < 2; j++) //0 -> -1, 1 -> 1, left or right
                {
                    //implicitly exclude the position pos
                    dest = new Position(pos.x + i, pos.y + direction * j); //same line or in front
                    if (GameState.IsValid(dest) && state.IsEnemy(dest, player))
                    {
                        currentMoves.Add(new Move(pos, dest, MoveType.capture));
                    }
                }
            }
        }

        protected void GenerateRetreats(GameState state, Position pos, TileColor player, int direction, ref List<Move> currentMoves)
        {
            Position dest, intermediate;
            if (state.AdjacentEnemy(pos, player)) // condition to retreat
            {
                for (int i = -1; i < 2; i++)
                { // 3 possible destinations
                    intermediate = new Position(pos.x + i, pos.y - direction); //opposite direction of the step move
                    dest = new Position(pos.x + 2 * i, pos.y - 2 * direction); //opposite direction of the step move
                    if (GameState.IsValid(intermediate) && GameState.IsValid(dest) && state.IsFree(intermediate) && state.IsFree(dest))
                    {
                        currentMoves.Add(new Move(pos, dest, MoveType.retreat));
                    }
                }
            }

        }

        protected void GenerateCannonMoves(GameState state, Position pos, TileColor player, int direction, ref List<Move> currentMoves)
        {
            Position intermediate;
            if (state.IsInCannon(pos, out List<Cannon> cannons))
            {
                foreach (Cannon c in cannons)
                {
                    // TODO ugly code
                    GetCannonDirection(c, pos, direction, out int dx, out int dy);
                    intermediate = new Position(pos.x + dx * direction, pos.y + dy * direction);
                    //shooting
                    if (GameState.IsValid(intermediate) && state.IsFree(intermediate))
                    {
                        intermediate = new Position(pos.x + 2 * dx * direction, pos.y + 2 * dy * direction);

                        if (GameState.IsValid(intermediate))
                        {
                            if (state.IsEnemy(intermediate, player)) // short shoot
                            {
                                currentMoves.Add(new Move(pos, intermediate, MoveType.shoot));
                            }
                            else if (state.IsFree(intermediate)) // we can do a long shoot
                            {
                                intermediate = new Position(pos.x + 3 * dx * direction, pos.y + 3 * dy * direction);
                                if (GameState.IsValid(intermediate) && state.IsEnemy(intermediate, player))
                                {
                                    currentMoves.Add(new Move(pos, intermediate, MoveType.shoot));
                                }
                            }
                        }
                    }

                    //sliding
                    intermediate = new Position(pos.x - 3 * dx * direction, pos.y - 3 * dy * direction);
                    if (GameState.IsValid(intermediate) && state.IsFree(intermediate))
                    {
                        currentMoves.Add(new Move(pos, intermediate, MoveType.slide));
                    }
                }
            }
        }

        protected void GetCannonDirection(Cannon c, Position p, int direction, out int dx, out int dy)
        {
            //Get the direction of the cannon
            //The direction is from p, facing out
            // *: other piece in the cannon, p: piece
            // * * p -> 
            // <- p * * 
            //*
            // *
            //  p
            //   \
            //    v
            // Assume p is actually in c

            // Values: -1 0 +1
            dx = Math.Sign(c.head2.x - c.head1.x) * direction;
            dy = Math.Sign(c.head2.y - c.head1.y) * direction;

            //p is head 1: swap direction
            if (c.head1 == p)
            {
                //Console.WriteLine("Bottom head");
                dx *= -1;
                dy *= -1;
            }
            //Console.WriteLine(c);
            //Console.WriteLine($"Direction: {dx} {dy}");
        }
        #endregion
    }

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


    //MoveType is sorted by how strong I think the move is
    public enum MoveType
    {
        none, // for debugging
        placeTown,
        step,
        retreat,
        slide,
        capture,
        shoot,
    }
}
