using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestTracker
{
    public static class MenuHelper
    {
        public static void DisplayMenu()
        {
            Console.WriteLine("1. Add Quest");
            Console.WriteLine("2. View Quests");
            Console.WriteLine("3. Update Quest");
            Console.WriteLine("4. Complete Quest");
            Console.WriteLine("5. notify soon overdue Quest");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");
        }

        public static void LogInMenu()
        {
         
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");

        }
    }
}
