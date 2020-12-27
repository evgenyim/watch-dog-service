using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ServiceStorage;

namespace Controller.Interfaces
{
    public interface ITrackingService
    {
        List<Status> CheckServices();
        List<Service> LoadServices(bool fromDB);
        List<IndexedDenial> LoadDenials(bool fromDB);
        void Save(bool toDB);
    }
}
