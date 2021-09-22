using System;
using System.Timers;
using Gtk;
namespace Cannon_GUI
{
    public class GameClock : Loggable
    {
        protected Timer timer;
        protected TimeSpan elapsed, max;
        protected Label timeLabel;

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
            timer = new Timer(1000);
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
        /*public void SetIterationTimeOut(TimeSpan maxItaration)
        {
            //Set the end time of the iterations
            iteration = elapsed + maxItaration;
        }*/

        /*public bool IterationTimeOut()
        {
            return elapsed > iteration;
        }*/
    }

    public abstract class Timed
    {
        protected GameClock clock;
        protected TimeSpan iterationEnd;
        protected TimeSpan halfIteration;
        protected TimeSpan iteration;

        public TimeSpan Iteration { set => iteration = value; }

        public void SetGameClock(GameClock clock)
        {
            this.clock = clock;
        }

        protected void SetIterationTimeOut(TimeSpan maxIteration)
        {
            iterationEnd = clock.Elapsed + maxIteration;
            halfIteration = clock.Elapsed + TimeSpan.FromTicks(maxIteration.Ticks / 2);
        }

        protected void SetIterationTimeOut()
        {
            SetIterationTimeOut(iteration);
        }

        protected bool IterationTimeOut()
        {
            return clock.Elapsed >= iterationEnd;
        }

        protected TimeSpan IterationRemaining()
        {
            TimeSpan tmp = iterationEnd - clock.Elapsed;
            return tmp > TimeSpan.Zero ? tmp : TimeSpan.Zero;
        }

        protected bool HalfIteration()
        {
            return clock.Elapsed >= halfIteration;
        }
    }
}
