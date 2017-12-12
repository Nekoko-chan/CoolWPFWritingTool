using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ComplexWriter.global
{
    public class CountDown: INotifyPropertyChanged
    {
        public int Seconds
        {
            get { return _seconds; }
            set { _seconds = value; OnPropertyChanged(); }
        }

        public event EventHandler Elapsed;


        private DispatcherTimer _timer;
        private int _seconds;

        public void StartCountdown(int seconds)
        {
            Seconds = seconds;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += _timer_Tick;
            _timer.Start();

        }

        void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Seconds=Seconds-1;
                if (Seconds != 0)
                    return;
                _timer.Stop();
                _timer.Tick -= _timer_Tick;
                _timer = null;
                if (Elapsed != null)
                    Elapsed(this,EventArgs.Empty);
            }
            catch 
            {
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
        }


        public void StopCountDown()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
            _timer = null;
        }
    }
}
