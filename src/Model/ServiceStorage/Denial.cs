using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Denial
    {
        public int serviceId { get; set; }
        public bool startWorking { get; set; }
        public DateTime time { get; set; }

        public Denial() { }

        public Denial(int serviceId, bool startWorking)
        {
            this.serviceId = serviceId;
            this.startWorking = startWorking;
            time = DateTime.Now;
        }

        public Denial(int serviceId, bool startWorking, DateTime time)
        {
            this.serviceId = serviceId;
            this.startWorking = startWorking;
            this.time = time;
        }
    }
}
