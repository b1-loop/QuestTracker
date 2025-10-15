using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace QuestTracker;

internal class Program
{
    static void Main(string[] args)
    {
        QuestManagement questManagement = new QuestManagement();
        Hero newHero = new Hero();
        bool isLoggedIn = false;

        
        while (true)
        {
            if (isLoggedIn)
            {
                MenuHelper.DisplayMenu();
                var mainInput = Console.ReadLine();
                switch (mainInput)
                {
                    case "1":
                        questManagement.AddQuest();
                        break;
                    case "2":
                        questManagement.ShowAllQuests();
                        break;
                    case "3":
                        questManagement.UpdateQuest();
                        break;

                    case "4":
                        questManagement.CompleteQuest();
                        break;
                    case "5":
                        questManagement.NotifySoonOverdueQuests();
                        break;
                    case "6":
                        isLoggedIn = false;
                        Console.WriteLine("Logged out.");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                MenuHelper.LogInMenu();
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Enter username:");
                        var username = Console.ReadLine();

                        Console.WriteLine("Enter password:");
                        var password = Console.ReadLine();

                        Console.WriteLine("Enter phone number (optional):");
                        var phone = Console.ReadLine();
                        Console.WriteLine("Enter email (optional):");
                        var email = Console.ReadLine();
                        Console.WriteLine("Registration successful. Please log in.");
                        newHero.Register(username, password, phone, email);
                        break;
                    case "2":
                        Console.WriteLine("Enter username:");
                        var loginUsername = Console.ReadLine();

                        Console.WriteLine("Enter password:");
                        var loginPassword = Console.ReadLine();
                        var ok = newHero.Login(loginUsername, loginPassword);
                        if (ok) isLoggedIn = true;
                        break;
                    case "3":
                        return;

                }
            }
        }
    }
}


