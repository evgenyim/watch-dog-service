using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ServiceStorage
{
    public class Denial
    {
        public int ServiceId { get; set; }
        public bool StartWorking { get; set; }
        public DateTime Time { get; set; }

        public Denial() { }

        public Denial(int serviceId, bool startWorking)
        {
            ServiceId = serviceId;
            StartWorking = startWorking;
            Time = DateTime.Now;
        }

        public Denial(int serviceId, bool startWorking, DateTime time)
        {
            ServiceId = serviceId;
            StartWorking = startWorking;
            Time = time;
        }
    }
}
