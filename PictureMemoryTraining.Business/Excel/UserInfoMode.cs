using System;
using Timer = System.Timers.Timer;

namespace PictureMemoryTraining.Business.Excel
{
    public class UserInfoMode : NotifyPropertyChanged
    {
        public string TestCode { get; set; }
        public string UserName { get; set; }
        public double Age { get; set; }
        public DateTime TestDate { get; set; }

        private int _consumingTime = 0;
        public int ConsumingSeconds
        {
            get => _consumingTime;
            set
            {
                _consumingTime = value;
                OnPropertyChanged();
            }
        }

        public void StartRecord()
        {
            var timer = new Timer();
            timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConsumingSeconds += 1;
        }
    }
}
