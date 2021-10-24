// The retreat move assumes that the soldier has to be adjacent to an enemy, 
// but that enemy might not able to campure it.
namespace Cannon_GUI
{
    /* 
     * Type of the move.
     *     
     * MoveType is sorted by how strong I think the move is, from the strongest
     * to the weakest.
     */
    public enum MoveType : byte
    {
        none = 0, // for debugging
        placeTown,
        /*step,
        retreat,
        slide,
        capture,
        shoot,*/
        shoot,
        capture,
        slide,
        retreat,
        step
    }
}
