using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Controller
{
    public interface ITrackingService
    {
        List<Status> CheckServices();
    }
}
