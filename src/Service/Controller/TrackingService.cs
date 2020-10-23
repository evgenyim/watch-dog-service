using Service.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Service.Controller
{
	public class TrackingService
	{
		public int x = 10;
		ServiceStorage storage = new ServiceStorage();
		ConsoleApp app;
		public TrackingService()
		{
			app = new ConsoleApp(this);
		}

		public void PR()
        {
			Console.WriteLine(646);
        }

		public void CheckServices()
        {
			Thread.Sleep(4100);
			app.HandleCommand();
			app.ShowServices(x);
			Console.WriteLine("Start");
			Thread.Sleep(5000);
			Console.WriteLine("Finish");
			app.ShowServices(x);

		}

		public void Run()
        {
			app.ShowServices(x);
			Console.ReadLine();
		}
	}
}
