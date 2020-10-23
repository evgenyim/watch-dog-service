using Service.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			TrackingService t = new TrackingService();
			t.Run();
		}
	}
}
