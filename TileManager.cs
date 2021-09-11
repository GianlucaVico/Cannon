using System;
using System.Collections.Generic;
namespace Cannon_GUI
{
    public class TileManager
    {
        protected Tile[,] tiles = new Tile[Constants.Size, Constants.Size];
        protected Tile selected;
        //TODO keep list of moves, the targets are the To values
        protected List<Move> targets; //store the moves that player can perform when a tile is selected
        protected TileColor player;
        protected MoveGenerator generator;

        protected GameState lastState;

        public TileManager(TileColor player, Gtk.Fixed parent, MoveGenerator generator)
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

            InitialPosition();
        }

        public void InitialPosition()
        {
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

            tiles[3, 0].ToDarkTown();
            tiles[3, 9].ToLightTown();
            targets = new List<Move>();
            Update();
            lastState = GetGameState();
        }
        public void Update() {
            foreach(Tile t in tiles) {
                t.Update();
            }
        }

        public void OnClick(Tile tile) {
            if (!(selected is null))
            {
                if (
                    (tile.Type == TileType.Piece && tile.Selected) ||
                    (!tile.Target)) //TODO <- check this condition
                {
                    selected.Selected = false;
                    selected = null;
                    tile.Selected = false;
                    ClearTargets();
                }else if(tile.Target)
                {
                    GameState newState = GetGameState().Apply(FindMoveFromTarget(tile.Position));
                    FromGameState(newState);
                }
            } else if(tile.Type == TileType.Piece && tile.Color == player){
                Console.WriteLine($"Selected {tile.Position}");
                tile.Selected = true;
                selected = tile;
                ClearTargets();
                UpdateTargets();
            }
            Update();
        }

        public void ChangePlayer(TileColor player) {
            //Set human player
            Console.WriteLine($"Human:{player}");
            this.player = player;
        }

        public void FromGameState(GameState state) {
            selected = null;

            for(int i = 0; i < Constants.Size; i++)
            {
                for (int j = 0; j < Constants.Size; j++)
                {
                    switch(state.Occupied[i, j])
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
            if (state.DarkTown != Constants.Removed)
                tiles[state.DarkTown.x, state.DarkTown.y].ToDarkTown();
            if (state.DarkTown != Constants.Removed)
                tiles[state.LightTown.x, state.LightTown.y].ToLightTown();
        }

        public GameState GetGameState()
        {
            TileColor[,] board = new TileColor[Constants.Size, Constants.Size];
            Position darkTown = new Position(-1, -1);
            Position lightTown = new Position(-1, -1);

            for(int i = 0; i < 10; i++)
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
            lastState = new GameState(board, darkTown, lightTown);
            return lastState;
        }

        protected void UpdateTargets()
        {
            //lastState = GetGameState();
           
            //targets.Capacity = 9; // at most 9 possible moves //TODO <- this crash everything
            //GameState state = GetGameState();
            foreach (Move m in generator.Generate(lastState, selected)) //TODO reuse previous game state
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

        protected Move FindMoveFromTarget(Position p) {
            bool found = false;
            int i = 0;
            Move res = new Move(new Position(-1, -1), new Position(-1, -1), MoveType.none);
            while(!found && i < targets.Count)
            {
                if(targets[i].To == p)
                {
                    found = true;
                    res = targets[i];
                }
                i++;
            }
            return res;
        }
    }
}
