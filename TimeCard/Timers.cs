using System;
using System.Timers;

namespace TimeCard
{
    public class Timers
    {
        private readonly Timer m_Timer = new Timer(60.0 * 1000);
        private DateTime m_Yesterday = DateTime.Today;

        public event EventHandler NewDay;

        public Timers()
        {
            m_Timer.Elapsed += Timer_Elapsed;
            m_Timer.Start();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Yesterday != DateTime.Today)
            {
                if (NewDay != null)
                {
                    NewDay(this, EventArgs.Empty);
                }
                m_Yesterday = DateTime.Today;
            }
        }
    }
}
