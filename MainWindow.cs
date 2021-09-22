using System;
using Cannon_GUI;
using Gtk;
using Pango;

public partial class MainWindow : Gtk.Window
{
    protected TileManager manager;

    public TextView LogBox { get => logBox; }
    public Fixed BoardPanel { get => boardPanel; }
    public RadioButton LightAI { get => lightAI; }
    public RadioButton DarkAI { get => darkAI; }

    public TileManager Manager { get => manager; }

    public Label TimeLabel { get => timeLabel; }

    public bool Quit = false;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        boardImage.Pixbuf = new Gdk.Pixbuf($"images{Constants.Slash}board.{Constants.ImgExt}");
    }



    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        //Application.Quit();
        a.RetVal = true;
        Quit = true;
    }

    protected void OnClick(object sender, EventArgs e)
    {
        Console.WriteLine("Click");
    }

    protected void ChangeAI(object obj, EventArgs e) //TODO handle 2 agents
    {
        Gtk.RadioButton rb = (Gtk.RadioButton)obj;
        if (rb.Active)
        {
            if (rb.Name == "DarkAI")
            {
                MainClass.Manager.ChangePlayer(TileColor.Light);
                MainClass.AI1.ChangePlayer(TileColor.Dark);
            }
            else if (rb.Name == "LightAI")
            {
                MainClass.Manager.ChangePlayer(TileColor.Dark);
                MainClass.AI1.ChangePlayer(TileColor.Light);
            }
            MainClass.Manager.playing = MainClass.Manager.Player == TileColor.Dark;
            MainClass.Manager.InitialPosition();
        }
    }

    protected void Reset(object obj, EventArgs e)
    {
        //MainClass.Manager.InitialPosition();
        //MainClass.Clock.Reset();
        MainClass.Reset();

    }

}
