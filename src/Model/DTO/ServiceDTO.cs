using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public abstract class ServiceDTO
    {
        public int Id { get; set; }

        public string Type { get; set; }
        
        public string Url { get; set; }
        
        public int TimeCheck { get; set; }
    }
}
