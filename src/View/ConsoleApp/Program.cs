using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleApp app = new ConsoleApp();
            app.HandleCommand("help");
            while (true)
            {
                string command = Console.ReadLine();
                if (!app.HandleCommand(command))
                    break;
            }

        }
    }
}
