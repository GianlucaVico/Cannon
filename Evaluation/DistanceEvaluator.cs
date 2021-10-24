using System;
namespace Cannon_GUI
{
    /*
     * Having a piece close to the opponent town is good for the player, but 
     * having an opponent piece close to its own town is bad.
     *
     * This evaluator compute the difference between the distance of the these
     * pieces and the towns.    
     * 
     * It uses the Mannhattan distance instead of the Euclidean distance.
     * This is because it is faster and because I assume there player/agent has
     * to defend to other attacks and "waste" some moves.
     * 
     * The distance from the town is particularly useful in endgame, where the 
     * agent has to run to the opponent town to win.    
     */
    public class DistanceEvaluator : IEvaluator
    {
        public int Evaluate(GameState state, TileColor player)
        {
            // if player dark : opponent wants to reach dark town
            int o = MinMannhattanDistance(state, (player == TileColor.Dark) ? state.DarkTown : state.LightTown, Utils.SwitchColor(player));
            int p = MinMannhattanDistance(state, (player == TileColor.Dark) ? state.LightTown : state.DarkTown, player);
            //Console.WriteLine($"Distance diff: {o - p}");
            return o - p; //minimize player distance, max opponent distance
        }

        protected int MinMannhattanDistance(GameState state, Position town, TileColor player)
        {
            int min = int.MaxValue;
            for (int i = 0; i < Constants.Size; i++)
            {
                for (int j = 0; j < Constants.Size; j++)
                {
                    if (state.Occupied[i, j] == player)
                    {
                        min = Math.Min(Math.Abs(town.x - i) + Math.Abs(town.y - j), min);
                    }
                }
            }
            if (min == int.MaxValue)
            {
                min = 0;
            }

            return min;
        }
    }
}
