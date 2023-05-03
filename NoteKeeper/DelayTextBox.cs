using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NoteKeeper
{
    public class DelayTextBox : TextBox
    {
        private System.Windows.Forms.Timer m_delayedTextChangedTimer;

        public DelayTextBox() : base()
        {
            DelayedTextChangedTimeout = 5 * 1000; // 10 seconds
        }

        ~DelayTextBox()
        {
            if (m_delayedTextChangedTimer != null)
            {
                m_delayedTextChangedTimer.Stop();
                m_delayedTextChangedTimer.Dispose();
            }
        }

        public int DelayedTextChangedTimeout { get; set; }

        protected virtual void OnDelayedTextChanged(EventArgs e)
        {
            MainWindow.instance.SaveData();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            InitializeDelayedTextChangedEvent();
            base.OnTextChanged(e);
        }

        private void InitializeDelayedTextChangedEvent()
        {
            if (m_delayedTextChangedTimer != null)
                m_delayedTextChangedTimer.Stop();

            if (m_delayedTextChangedTimer == null || m_delayedTextChangedTimer.Interval != this.DelayedTextChangedTimeout)
            {
                m_delayedTextChangedTimer = new System.Windows.Forms.Timer();
                m_delayedTextChangedTimer.Tick += new EventHandler(HandleDelayedTextChangedTimerTick);
                m_delayedTextChangedTimer.Interval = this.DelayedTextChangedTimeout;
            }

            m_delayedTextChangedTimer.Start();
        }

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = sender as System.Windows.Forms.Timer;
            timer.Stop();

            OnDelayedTextChanged(EventArgs.Empty);
        }
    }
}
