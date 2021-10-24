namespace Cannon_GUI
{
    /*
     * Interface for players, both human and AI
     */
    public interface IPlayer
    {
        void ChangePlayer(TileColor player);
        TileColor Player { get; }
    }
}
