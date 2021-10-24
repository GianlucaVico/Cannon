using Gtk;
namespace Cannon_GUI
{
    /*
     * Write in the GUI and scroll to the end of the text box.   
    */
    public class GUILog : ILog
    {
        protected TextView view;
        public GUILog(TextView view)
        {
            this.view = view;
        }

        public void Log(string action, string obj)
        {
            _log($"> {action} : {obj}\n");
        }

        public void Log(string s)
        {
            _log($"> {s}\n");
        }

        protected void _log(string s)
        {
            TextIter end = view.Buffer.EndIter;
            view.Buffer.Insert(ref end, s);
            ScrollEnd();
        }

        protected void ScrollEnd()
        {
            view.ScrollToIter(view.Buffer.EndIter, 0, true, 1, 1);
        }
    }
}
