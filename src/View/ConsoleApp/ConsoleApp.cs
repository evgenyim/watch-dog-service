using Controller.TrackingService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Model;
using Model.ServiceStorage;

namespace ConsoleApp
{
    class ConsoleApp
    {
        private TrackingService t = new TrackingService();

        public void ShowServices(int x)
        {
            Console.WriteLine(x);
        }

        public bool HandleCommand(string command)
        {
            string[] commands = command.Split();
            if (commands[0] == "check")
            {
                List<Status> ret = t.CheckServices();
                foreach(var res in ret)
                {
                    Console.WriteLine(res.toString());
                }
                return true;
            } 
            else if (commands[0] == "add")
            {
                t.AddService("WebService", commands[1], commands[2]);
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
