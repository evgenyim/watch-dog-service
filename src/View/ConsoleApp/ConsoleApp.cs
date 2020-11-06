using Controller;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class ConsoleApp
    {
        TrackingService t = new TrackingService();

        public ConsoleApp()
        {
        }
        public void ShowServices(int x)
        {
            Console.WriteLine(x);
        }

        public bool HandleCommand(string command)
        {
            string[] commands = command.Split();
            if (commands[0] == "check")
            {
                List<bool> ret = t.CheckServices();
                foreach(bool res in ret)
                {
                    Console.WriteLine(res);
                }
                return true;
            } 
            else if (commands[0] == "add")
            {
                t.AddWebservice(commands[1]);
                return true;
            }
            else if (commands[0] == "quit")
            {
                return false;
            }
            else if (commands[0] == "help")
            {
                Console.WriteLine("Commands:");
                Console.WriteLine("add <port> : adds new service with adress http://localhost:<port>/");
                Console.WriteLine("check : checks all services");
                Console.WriteLine("quit : close app");
                return true;
            }
            Console.WriteLine("Wrong input, type help");
            return true;
        }
    }

}
