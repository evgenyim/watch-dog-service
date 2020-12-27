using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ServiceStorage
{
    public class IndexedDenial
    {
        public int Id;
        public Denial Denial;
        public string Url;

        public IndexedDenial(int id, Denial d, string url)
        {
            Id = id;
            Denial = d;
            Url = url;
        }
    }
}
