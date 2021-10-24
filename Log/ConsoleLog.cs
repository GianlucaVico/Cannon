using System;
namespace Cannon_GUI
{
    /*
     * Write in the console
     */
    public class ConsoleLog : ILog
    {
        public void Log(string s)
        {
            Console.WriteLine(s);
        }

        public void Log(string action, string obj)
        {
            Console.WriteLine($"{action} : {obj}");
        }
    }
}
