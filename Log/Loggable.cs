namespace Cannon_GUI
{
    /*
     * Abstract class for objects that use ILog objects
     */
    public abstract class Loggable
    {
        protected static ILog _logger = new ConsoleLog();
        protected ILog logger = _logger;

        public void SetLogger(ILog l)
        {
            logger = l;
        }
    }
}
