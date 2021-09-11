using System;
using Cannon_GUI;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    TileManager manager;
    MoveGenerator generator;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        generator = new MoveGenerator();
        manager = new TileManager(LightAI.Active ? TileColor.Dark : TileColor.Light, this.fixed1, generator);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnClick(object sender, EventArgs e)
    {

            Console.WriteLine("Click");

           // Application.Quit();

    }    

    protected void ChangeAI(object obj, EventArgs e) {
        Console.WriteLine("Switch AI");
        Gtk.RadioButton rb = (Gtk.RadioButton)obj;
        if(rb.Active)
        {
            if(rb.Name == "DarkAI")
                manager.ChangePlayer(TileColor.Light);
            else if(rb.Name == "LightAI")
                manager.ChangePlayer(TileColor.Dark);
        }
    }


    /*protected void OnClick(object o, ButtonReleaseEventArgs args)
    {
        Console.WriteLine("Click");
    }*/
}
