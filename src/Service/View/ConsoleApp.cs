using Service.Controller;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    class ConsoleApp
    {
        TrackingService t;

        public ConsoleApp(TrackingService t)
        {
            this.t = t;
        }
        public void ShowServices(int x)
        {
            Console.WriteLine(x);
        }

        public async void HandleCommand()
        {

        }

    }
}
