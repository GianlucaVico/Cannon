using System;
using System.Timers;
using Gtk;
namespace Cannon_GUI
{
    /*
     * Manage the time of the game.
     */
    public class GameClock : Loggable
    {
        protected Timer timer; // Timer object, fire and event every tick
        protected TimeSpan elapsed, max;  // Maximum time allowed and elepsed time
        protected Label timeLabel;  // To display the time in the GUI

        public TimeSpan Elapsed { get => elapsed; }

        public GameClock(TimeSpan maxTime) : this(maxTime, null) { }

        public GameClock(TimeSpan maxTime, Label timeLabel)
        {
            this.timeLabel = timeLabel;
            Reset();
            max = maxTime;
            Stop();
        }

        protected void Tick(object obj, ElapsedEventArgs e)
        {
            elapsed += TimeSpan.FromMilliseconds(timer.Interval);
            if (TimeOut())
            {
                //logger.Log(">> TIMEOUT: AI LOOSE <<");
                timer.Stop();
            }
            if (timeLabel != null)
                timeLabel.Text = elapsed.ToString(@"mm\:ss");
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Reset()
        {
            if (!(timer == null))
            {
                timer.Dispose();
            }
            timer = new Timer(Constants.ClockResolution);
            timer.Elapsed += Tick;
            timer.AutoReset = true;
            timer.Enabled = true;
            elapsed = TimeSpan.Zero;
            timeLabel.Text = elapsed.ToString(@"mm\:ss");
        }

        public TimeSpan TimeRemaining()
        {
            return max - elapsed;
        }

        public bool TimeOut()
        {
            return elapsed > max;
        }

        public void SetTime(TimeSpan time)
        {
            elapsed = time;
        }
    }
}
