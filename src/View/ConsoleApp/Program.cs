using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("check <port> : checks that on adress http://localhost:<port>/ exists working service");
            Console.WriteLine("quit : close app");
            ConsoleApp app = new ConsoleApp();
            while (true)
            {
                string command = Console.ReadLine();
                if (!app.HandleCommand(command))
                    break;
            }

        }
    }
}
