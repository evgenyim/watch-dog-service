using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingserviceTests
{
    class TestServiceGood : Service
    {
        public override bool IsAlive()
        {
            return true;
        }
    }

    class TestServiceBad : Service
    {
        public override bool IsAlive()
        {
            return false;
        }
    }

}
