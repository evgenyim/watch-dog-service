using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    abstract public class Status
    {
        abstract public string toString();
        abstract public string getUrl();
        abstract public bool getStatus();
    }
}
