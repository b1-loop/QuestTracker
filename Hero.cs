using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;
using Twilio.Types;

namespace QuestTracker
{
        //Skapa ny hjälteprofil med:
        //Username(hjältenamn)
        //Password(lösenord) – styrkekontroll(minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken).
        //Email eller Phone för 2FA.

        //Vid inloggning:
        //Ange namn/lösenord.
        //Systemet skickar kod via SMS/Email(2FA) → måste anges korrekt för att komma in i guilden.
    public class Hero
    {
        public string Username { get; set; }    
        public string Password { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool CheckPassword(string password)
        {
           
            if (password.Length < 6) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) return false;
            return true;
        }

        public bool CheckUsername(string username)
        {
            
            if (string.IsNullOrEmpty(username)) return false;
            return true;
        }

        public bool Login(string username, string password)
        {
            if (username == Username && password == Password)
            {
                Console.WriteLine("Login successful. 2FA code sent to your phone/email.");
                return TwoFactorAuth();
                
            }
            Console.WriteLine("Invalid username or password.");
            return false;
            
        }

        public bool Register(string username, string password, string phone = null, string email = null)
        {
            if (!CheckUsername(username))
            {
                Console.WriteLine("Invalid username.");
                return false;
            }
            if (!CheckPassword(password))
            {
                Console.WriteLine("Password does not meet the requirements.");
                return false;
            }
            if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Either phone or email must be provided for 2FA.");
                return false;
            }
            Username = username;
            Password = password;
            Phone = phone;
            Email = email;
            Console.WriteLine("Registration successful.");
            return true;
        }

        public bool TwoFactorAuth()
        {
            var config = new ConfigurationBuilder()
                            .AddUserSecrets<Program>()  // reads from your user secrets
                            .Build();

            var accountSid = config["Twilio:ACCOUNT_SID"];
            var authToken = config["Twilio:AUTH_TOKEN"];
            var fromNumber = config["Twilio:FROM_NUMBER"];
            var toNumber = config["Twilio:TO_NUMBER"];


            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("Twilio credentials are missing.");
                return false;
            }

            if (string.IsNullOrEmpty(fromNumber) || string.IsNullOrEmpty(toNumber))
            {
                Console.WriteLine("Twilio phone numbers are missing.");
                return false;
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
            bool waitForCode = true;
            while (waitForCode)
            {
                Console.Write("Enter the 2FA code sent to your phone/email: ");
                var inputCode = Console.ReadLine();
                if (inputCode == code)
                {
                    Console.WriteLine("2FA successful. Welcome to the guild!");
                    waitForCode = false;
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid code. Please try again.");
                }
            }
            return false;

        }
    }
}
