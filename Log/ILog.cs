namespace Cannon_GUI
{
    /*
     * Interface for log objects 
     */
    public interface ILog
    {
        void Log(string action, string obj);
        void Log(string s);
    }
}
