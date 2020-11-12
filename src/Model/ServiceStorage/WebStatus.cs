using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class WebStatus: Status
    {
        private bool status;
        private string url;

        public WebStatus(bool status, string url)
        {
            this.status = status;
            this.url = url;
        }

        public override string toString()
        {
            if (status)
                return url + " is working";
            return url + " is not working";
        }
    }
}
