using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    abstract public class Service
    {
        public string url;
        public int timeCheck;
        abstract public Status IsAlive();
    }
}
