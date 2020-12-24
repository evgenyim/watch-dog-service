using System;
using System.Collections.Generic;
using System.Text;

namespace Model.ServiceStorage
{
    abstract public class Service
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int TimeCheck { get; set; }
        abstract public Status IsAlive();
    }
}
