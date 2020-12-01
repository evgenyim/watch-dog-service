using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    abstract public class Service
    {
        public string url { get; set; }
        public int timeCheck { get; set; }
        abstract public Status IsAlive();
    }
}
