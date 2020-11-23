using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IServiceStorage
    {
        int AddService(Service s);
        int AddWebService(string url);
    }
}
