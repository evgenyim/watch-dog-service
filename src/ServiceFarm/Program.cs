using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace ConsoleHosting
{
    class Program
    {
        public static List<(IDisposable, int)> servers = new List<(IDisposable, int)>();
        static int cnt;
        static void Main(string[] args)
        { 
            Console.WriteLine("Enter ports amount:");
            int ports = Int32.Parse(Console.ReadLine());

            CancellationTokenSource cts = new CancellationTokenSource();

            for (int i = 0; i < ports; i++)
            {
                string baseAddress = "http://localhost:123" + i.ToString() + "/";
                IDisposable server = WebApp.Start<Startup>(url: baseAddress);
                Console.WriteLine($"WebAPI SelfHost started at {baseAddress}");

                servers.Add((server, i));
            }
            cnt = ports;
            while (HandleCommand(Console.ReadLine()))
            { }
            Console.WriteLine("Closing web services");
            foreach((IDisposable s, int i) in servers)
            {
                Console.WriteLine("Closing " + i.ToString() + " service");
                s.Dispose();
            }
            Console.WriteLine("All services closed");
            Console.ReadLine();
        }

        public static bool HandleCommand(string command)
        {
            string[] commands = command.Split();
            if (commands[0] == "add")
            {
                string baseAddress = "http://localhost:123" + cnt.ToString() + "/";
                IDisposable server = WebApp.Start<Startup>(url: baseAddress);
                Console.WriteLine($"WebAPI SelfHost started at {baseAddress}");

                servers.Add((server, cnt));
                cnt += 1;
                return true;
            }
            else if (commands[0] == "stop")
            {
                int i = Int32.Parse(commands[1]);

                (IDisposable s, int j) = servers[i];
                s.Dispose();
                return true;
            }
            else if (commands[0] == "show")
            {
                foreach ((IDisposable s, int i) in servers)
                {
                    Console.WriteLine(i.ToString() + " http://localhost:123" + i.ToString() + "/");
                }
                return true;
            }
            else if (commands[0] == "quit")
            {
                return false;
            }
            else if (commands[0] == "help")
            {
                Console.WriteLine("Commands:");
                Console.WriteLine("add : starts new app");
                Console.WriteLine("stop <i> : stops app with number i");
                Console.WriteLine("show : shows all aps with their IDs");
                Console.WriteLine("quit : close app");
                return true;
            }
            Console.WriteLine("Wrong input, type help");
            return true;
        }

    }
}
