using System;
namespace Cannon_GUI
{
    /*
     * Abstract class for classes that need to use a clock
     */
    public abstract class Timed
    {
        protected GameClock clock;
        protected TimeSpan iterationEnd;
        protected TimeSpan halfIteration; // Half of the iteration, used, e.g., to decide if there is enough time to continue
        protected TimeSpan iteration;  // Time allowed for an iteration

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
