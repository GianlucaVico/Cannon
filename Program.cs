using System;
using Gtk;
//Tile size: 70px
using Gdk;
namespace Cannon_GUI
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }

    }
}
