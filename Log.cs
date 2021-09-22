using System;
using Gtk;
namespace Cannon_GUI
{
    public interface Log
    {
        void Log(string action, string obj);
        void Log(string s);
    }

    public abstract class Loggable
    {
        protected static Log _logger = new ConsoleLog();
        protected Log logger = _logger;

        public void SetLogger(Log l)
        {
            logger = l;
        }
    }

    public class ConsoleLog : Log
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

    public class GUILog : Log
    {
        protected TextView view;
        public GUILog(TextView view)
        {
            this.view = view;
        }

        public void Log(string action, string obj)
        {
            /*TextIter end = view.Buffer.EndIter;
            view.Buffer.Insert(ref end, $"> {action} : {obj}\n");
            ScrollEnd();*/

            Gtk.Application.Invoke((o, e) => _log($"> {action} : {obj}\n"));
        }

        public void Log(string s)
        {
            /*TextIter end = view.Buffer.EndIter;
            view.Buffer.Insert(ref end, $"> {s}\n");
            ScrollEnd();*/
            Gtk.Application.Invoke((o, e) => _log($"> {s}\n"));
        }

        protected void _log(string s)
        {
            TextIter end = view.Buffer.EndIter;
            view.Buffer.Insert(ref end, s);
            ScrollEnd();
        }
        protected void ScrollEnd()
        {
            view.ScrollToIter(view.Buffer.EndIter, 0, true, 0, 0);
        }
    }
}
