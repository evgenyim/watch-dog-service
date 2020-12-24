using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ServiceStorage.Interfaces
{
    public interface IServiceStorage
    {
        int AddService(Service s);
        int AddService(string url);
        int AddService(string url, string checkUrl, int timeCheck);
        List<Status> CheckServices();

        void DeleteService(int id);
    }
}
