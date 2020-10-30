using Controller;
using System;
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
                if (t.CheckServices(commands[1]))
                {
                    Console.WriteLine("Service is alive");
                }
                else
                {
                    Console.WriteLine("Service is dead");
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
                Console.WriteLine("check <port> : checks that on adress http://localhost:<port>/ exists working service");
                Console.WriteLine("quit : close app");
            }
            Console.WriteLine("Wrong input, type help");
            return true;
        }
    }

}
