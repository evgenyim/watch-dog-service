using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ServiceStorage
{
    abstract public class Status
    {
        public int ServiceId;
        public bool IsAlive;
        abstract public string toString();

    }
}
