using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PictureMemoryTraining.Business;
using Timer = System.Timers.Timer;

namespace PictureMemoryTraining.Views.Models
{
    public class UserInfoMode : NotifyPropertyChanged
    {
        public string TestCode { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
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

        private DateTime _recordTime = DateTime.MinValue;
        public void StartRecord()
        {
            _recordTime = DateTime.Now;
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
