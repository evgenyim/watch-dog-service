using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ServiceStorage
{
    public class WebStatus: Status
    {
        public string Url;

        public WebStatus(int id, bool status, string url)
        {
            ServiceId = id;
            IsAlive = status;
            Url = url;
        }

        public override string toString()
        {
            if (IsAlive)
                return Url + " is working";
            return Url + " is not working";
        }
    }
}
