using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace QuestTracker;

internal class Program
{
    static void Main(string[] args)
    {
        var superMario = new Hero
        {
            Username = "SuperMario",
            Password = "Abcd1234!?",
            Phone = "+46701234567",
        };

        superMario.Login(Console.ReadLine(), Console.ReadLine());
    }
}


