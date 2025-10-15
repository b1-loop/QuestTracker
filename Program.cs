using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace QuestTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");
            var toNumber = Environment.GetEnvironmentVariable("TWILIO_TO_NUMBER");

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("Twilio credentials are missing.");
                return;
            }

            if (string.IsNullOrEmpty(fromNumber) || string.IsNullOrEmpty(toNumber))
            {
                Console.WriteLine("Twilio phone numbers are missing.");
                return;
            }

            TwilioClient.Init(accountSid, authToken);

            var from = new PhoneNumber(fromNumber);
            var to = new PhoneNumber(toNumber);
            var code = new Random().Next(100000, 999999).ToString();
            var message = MessageResource.Create(
                to: to,
                from: from,
                body: $"Hello from my C# console app 👋 here is your code sir: {code}"
            );
        }
    }
}
