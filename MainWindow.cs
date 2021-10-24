using System;
using Cannon_GUI;
using Gtk;
using Pango;

/*
 * GUI class
 */
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
        // Board background
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

    /*
     * Switch players event
     */
    protected void ChangeAI(object obj, EventArgs e) //TODO handle 2 agents
    {
        Gtk.RadioButton rb = (Gtk.RadioButton)obj;
        if (rb.Active)
        {
            Console.WriteLine(rb.Name);
            if (rb.Name == "darkAI")
            {
                MainClass.AI1.ChangePlayer(TileColor.Dark);
                MainClass.Manager.ChangePlayer(TileColor.Light);

            }
            else if (rb.Name == "lightAI")
            {
                MainClass.AI1.ChangePlayer(TileColor.Light);
                MainClass.Manager.ChangePlayer(TileColor.Dark);
            }
            MainClass.Manager.playing = MainClass.Manager.Player == TileColor.Dark;
            MainClass.Reset();
        }
    }

    /*
     * Start a new game event
     */
    protected void Reset(object obj, EventArgs e)
    {
        //MainClass.Manager.InitialPosition();
        //MainClass.Clock.Reset();
        MainClass.Reset();
    }

    /*
     * Undo button event
     */
    protected void Undo(object obj, EventArgs e)
    {
        MainClass.Undo();
    }

}
