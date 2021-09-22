using System;
using Gtk;
//Tile size: 70px
using Gdk;
using System.Threading.Tasks;

namespace Cannon_GUI
{
    class MainClass
    {
        protected static Log guiLog;
        protected static MoveGenerator generator;
        protected static GameClock clock;
        protected static Agent ai1, ai2;   //TODO set the other agent
        protected static bool found1, found2;   //did we found the move for the other agent?
        protected static TileManager manager;

        protected static bool resultNotified;

        public static Agent AI1 { get => ai1; }
        public static Agent AI2 { get => ai2; }
        public static TileManager Manager { get => manager; }
        public static GameClock Clock { get => clock; }

        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();

            guiLog = new GUILog(win.LogBox);
            //guiLog = new ConsoleLog();
            generator = new MoveGenerator();
            clock = new GameClock(Constants.MaxTime, win.TimeLabel);

            ai1 = new SimpleAgent(generator, clock, win.LightAI.Active ? TileColor.Light : TileColor.Dark);
            ai1.Iteration = Constants.MaxIteration;

            Console.WriteLine(win.LightAI.Active);
            manager = new TileManager(win.LightAI.Active ? TileColor.Dark : TileColor.Light, win.BoardPanel, generator, ai1, guiLog);

            //clock.Start();
            while (!win.Quit || Application.EventsPending())
            {
                Application.RunIteration();

                //Alternate players playing


                if (manager.LastState.End() && !resultNotified)
                {
                    guiLog.Log($">> WINNER: {manager.LastState.Winner()} <<");
                    resultNotified = true;
                }
                else
                {
                    //Do actions and switch
                    if (manager.playing)
                    {
                        //human playing
                        //Console.WriteLine("Human playing");
                    }
                    else
                    {
                        found1 = ai1.DoMove(manager.GetGameState(), out Move move);
                        if (found1)
                        {
                            Console.WriteLine("AI playing");
                            manager.ApplyMove(move);
                            found1 = false;
                            manager.playing = true;
                        }
                        if (clock.TimeOut() && !resultNotified)
                        {
                            guiLog.Log(">> TIMEOUT: AI LOOSE <<");
                            resultNotified = true;
                        }
                    }
                }
            }
        }

        public static void Reset()
        {
            found1 = false;
            found2 = false;
            clock.Reset();
            manager.InitialPosition();
        }

        protected void AIvsHumanIteration() { }
    }
}
