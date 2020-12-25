using Model;
using Model.ServiceStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingserviceTests
{
    class TestServiceGood : Service
    {
        public TestServiceGood()
        {
            Id = 1;
            TimeCheck = 2;
        }
        public override Status IsAlive()
        {
            return new TestStatus(Id, true);
        }
    }

    class TestServiceBad : Service
    {
        public TestServiceBad()
        {
            Id = 2;
            TimeCheck = 2;
        }
        public override Status IsAlive()
        {
            return new TestStatus(Id, false);
        }
    }

    class TestServiceChanging : Service
    {
        private bool lastChecked = true;
        
        public TestServiceChanging()
        {
            Id = 3;
            TimeCheck = 2;
        }

        public override Status IsAlive()
        {
            lastChecked = !lastChecked;
            return new TestStatus(Id, lastChecked);
        }
    }

    class TestStatus : Status
    {
        public TestStatus(int id, bool status)
        {
            ServiceId = id;
            IsAlive = status;
        }
        public override string toString()
        {
            return IsAlive.ToString();
        }
    }

}
