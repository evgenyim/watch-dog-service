using System;
using Microsoft.Owin.Hosting;

namespace ConsoleHosting
{
    class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine("Enter port number:");
            string port = Console.ReadLine();

            string baseAddress = "http://localhost:" + port + "/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"WebAPI SelfHost started at {baseAddress}");
                Console.WriteLine("Press enter to finish");
                Console.ReadLine();
            }
        }
    }
}
