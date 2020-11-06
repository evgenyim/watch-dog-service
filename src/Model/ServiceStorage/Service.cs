using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public abstract class Service
    {
        public string url;
        abstract public bool IsAlive();
    }
}
