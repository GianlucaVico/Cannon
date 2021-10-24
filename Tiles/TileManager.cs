using System;
using System.Collections.Generic;

namespace Cannon_GUI
{
    /*
     * Class that managet the GUI board and the interaction with the human player.
     */
    public class TileManager : Loggable, IPlayer
    {
        protected Tile[,] tiles = new Tile[Constants.Size, Constants.Size]; // Matrix of tiles
        protected Tile selected; // Selected tile
        protected List<Move> targets; // Store the moves that player can perform when a tile is selected
        protected TileColor player; // Human player
        protected MoveGenerator generator;

        protected GameState lastState;
        protected bool stop = false;
        // false when the ai is playing, true, when the human is playing
        public bool playing = true;
        public History history;

        public GameState LastState { get => lastState; }

        public TileColor Player => player;

        public TileManager(TileColor player, Gtk.Fixed parent, MoveGenerator generator, Agent opponent)
        {
            this.generator = generator;
            this.player = player; // check which tiles we can select
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    tiles[i, j] = new Tile(i, j, parent, this);

                    tiles[i, j].Show();
                }
            }
            //this.opponent = opponent;
            InitialPosition();
        }

        public TileManager(TileColor player, Gtk.Fixed parent, MoveGenerator generator, Agent opponent, ILog l) : this(player, parent, generator, opponent)
        {
            logger = l;
            lastState.SetLogger(l);
        }

        public void InitialPosition()
        {
            lastState = null;
            logger.Log("Board to initial position");
            stop = false;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (j >= 1 && j <= 3 && i % 2 == 0) // dark piece
                    {
                        tiles[i, j].ToDarkPiece();
                    }
                    else if (j >= 6 && j <= 8 & i % 2 == 1)
                    { //light piece
                        tiles[i, j].ToLightPiece();
                    }
                    else
                    {
                        tiles[i, j].ToEmpy();
                    }
                    tiles[i, j].Show();
                }
            }

            //tiles[3, 0].ToDarkTown();
            //tiles[3, 9].ToLightTown();
            targets = new List<Move>();
            stop = false;
            playing = (player == TileColor.Dark);
            Update();
            lastState = GetGameState();
        }

        /*
         * Update the look of all the tiles
         */
        public void Update()
        {
            foreach (Tile t in tiles)
            {
                t.Update();
            }
        }

        /*
         * Click event. Respond to the user interaction
         */
        public void OnClick(Tile tile)
        {
            if (!stop && playing)  // We can and we are playing
            {
                if (!(selected is null)) // Deselect a piece if it is not a target
                {
                    if (
                        (tile.Type == TileType.Piece && tile.Selected) ||
                        (!tile.Target))
                    {
                        Deselect(tile);
                    }
                    else if (tile.Target)   // Perform a move from the selected piece to the target
                    {
                        GameState newState = ApplyMove(FindMoveFromTarget(tile.Position));
                        if (lastState.End())
                        {
                            logger.Log("Winner", lastState.Winner().ToString());
                            stop = true;
                        }
                        lastState = newState;
                        playing = false;
                    }
                }
                else if (tile.Type == TileType.Piece && tile.Color == player) // Select a piece
                {
                    Select(tile);
                }
            }
            Update();
        }

        protected void Select(Tile tile)
        {
            tile.Selected = true;
            selected = tile;
            ClearTargets();
            UpdateTargets();
        }

        protected void Deselect(Tile tile)
        {
            selected.Selected = false;
            selected = null;
            tile.Selected = false;
            ClearTargets();
        }

        public void ChangePlayer(TileColor player)
        {
            //Set human player
            logger.Log("Human player", player.ToString());
            this.player = player;
        }

        /*
         * Set a GUI board from a given game state
         */
        public void FromGameState(GameState state)
        {
            selected = null;

            for (int i = 0; i < Constants.Size; i++)
            {
                for (int j = 0; j < Constants.Size; j++)
                {
                    switch (state.Occupied[i, j])
                    {
                        case TileColor.No:
                            tiles[i, j].ToEmpy();
                            break;
                        case TileColor.Dark:
                            tiles[i, j].ToDarkPiece();
                            break;
                        case TileColor.Light:
                            tiles[i, j].ToLightPiece();
                            break;
                    }
                }
            }
            if (GameState.IsValidTown(state.DarkTown))
                tiles[state.DarkTown.x, state.DarkTown.y].ToDarkTown();
            if (GameState.IsValidTown(state.LightTown))
                tiles[state.LightTown.x, state.LightTown.y].ToLightTown();
            lastState = state;
        }

        /*
         * Get the current game state of the board
         */
        public GameState GetGameState()
        {
            TileColor[,] board = new TileColor[Constants.Size, Constants.Size];
            Position darkTown = Constants.NotPlaced;
            Position lightTown = Constants.NotPlaced;
            if (!(lastState is null))
            {
                // Keep the removed state
                if (lastState.DarkTown == Constants.Removed)
                    darkTown = Constants.Removed;
                if (lastState.LightTown == Constants.Removed)
                    lightTown = Constants.Removed;
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    board[i, j] = tiles[i, j].Color;
                    if (tiles[i, j].Type == TileType.Town)
                    {
                        if (tiles[i, j].Color == TileColor.Dark)
                            darkTown = tiles[i, j].Position;
                        if (tiles[i, j].Color == TileColor.Light)
                            lightTown = tiles[i, j].Position;
                    }
                }
            }
            lastState = new GameState(board, darkTown, lightTown, logger);
            return lastState;
        }

        protected void UpdateTargets()
        {
            Position town = (player == TileColor.Dark) ? lastState.DarkTown : lastState.LightTown;
            List<Move> moves;
            if (town == Constants.NotPlaced)
            {
                moves = generator.GenerateTownPlacements(GetGameState(), player);
            }
            else
            {
                moves = generator.Generate(GetGameState(), selected);
            }
            foreach (Move m in moves) //TODO reuse previous game state
            {
                tiles[m.To.x, m.To.y].Target = true;
                targets.Add(m);
            }
        }

        protected void ClearTargets()
        {
            Position dest;
            foreach (Move m in targets)
            {
                dest = m.To;
                tiles[dest.x, dest.y].Target = false;
            }
            targets.Clear();
        }

        protected Move FindMoveFromTarget(Position p)
        {
            bool found = false;
            int i = 0;
            Move res = new Move(new Position(-1, -1), new Position(-1, -1), MoveType.none);
            while (!found && i < targets.Count)
            {
                if (targets[i].To == p)
                {
                    found = true;
                    res = targets[i];
                }
                i++;
            }
            return res;
        }

        /*
         * Apply a move and change the game state
         */
        public GameState ApplyMove(Move move)
        {
            GameState newState = lastState.Apply(move);
            logger.Log("Apply", move.ToString());
            FromGameState(newState);
            Update();
            if (!(history is null))
            {
                history.Push(move, newState);
            }
            return newState;
        }
    }
}