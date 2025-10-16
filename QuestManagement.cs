using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace QuestTracker
{
    public class QuestManagement
    {
        public List<Quest> quests = new List<Quest>();
        public void AddQuest()
        {
            //Console.WriteLine("Enter quest title:"); eftersom chat kostatde för göra min egna chattGPT
            //var title = Console.ReadLine();


            // var description = CreateAIDescription(title).Result;

            string[] titles = 
            {
            "1.Slay the Coffee Dragon ",
            "2.Find the Missing Wi-Fi Signal ",
            "3.Save the Kingdom of Deadlines ",
            "4.Rescue the Lost USB of Power ",
            "5.Defeat the Monday Boss "
            };

            for (int i = 0; i < titles.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {titles[i]}");
            }

            Console.Write("Enter your choice (1-5): ");
            int choice = Convert.ToInt32(Console.ReadLine());
            string title = titles[Math.Clamp(choice - 1, 0, titles.Length - 1)];

            // Skapa en rolig beskrivning beroende på titeln
            string description = title switch
            {
                "1.Slay the Coffee Dragon " => "A mighty beast made of caffeine threatens your productivity. Only one brave soul can brew victory!",
                "2.Find the Missing Wi-Fi Signal " => "Somewhere in the dark corners of your home, the sacred connection has vanished. Search, hero!",
                "3.Save the Kingdom of Deadlines " => "The kingdom trembles under procrastination. You must restore order before the time runs out!",
                "4.Rescue the Lost USB of Power " => "Legends speak of a tiny relic holding ancient files. Find it before your boss notices it's gone!",
                "5.Defeat the Monday Boss " => "It rises every week without fail. Gather your strength, face the beast, and survive until Friday!",
                _ => "An unknown quest awaits you in the shadows..."
            };

            //Console.WriteLine("Enter quest description:");
            //var discription = Console.ReadLine();

            Console.WriteLine("Enter amount of days to complete the quest");
            int dueDateInput = Convert.ToInt32(Console.ReadLine());
            DateTime dueDate = DateTime.Now.AddDays(dueDateInput);

            Console.WriteLine("Enter quest priority (1=High, 2=Medium,3=Low):");
            var priorityInput = Convert.ToInt32(Console.ReadLine());
            Priority priority;

            Console.WriteLine("Has the task been completed? (1=Yes, 0=No):");
            var isCompletedInput = Convert.ToInt32(Console.ReadLine());
            bool isCompleted = isCompletedInput == 1;

            Quest app = new Quest
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = (Priority)(priorityInput - 1),
                IsCompleted = isCompleted,
            };

            quests.Add(app);
            Console.WriteLine("Task added!");
        }

        private async Task<string> CreateAIDescription(string? title)
        {

            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            var apiKey = config["OpenAI:API_KEY"];

            var client = new ChatClient("gpt-4o-mini", apiKey);
            var prompt = $"Du är en storyteller för mitt fantasispel, ge mig en beskrivning för följande quest med titel: {title}. ge mig endast beskrivningen i form av en sträng inget annat!";

            // 🚀 Skicka prompten och hämta svar
            ChatCompletion response = await client.CompleteChatAsync(new[]
            {
                new UserChatMessage(prompt)
            });
            Console.WriteLine(response.Content[0].Text);
            return response.Content[0].Text.ToString();

        }

        private string GetQuestStatus(Quest quest)
        {
            if (quest.IsCompleted)
                return "Completed";
            else if (DateTime.Now > quest.DueDate)
                return "Overdue";
            else
                return "Active";
        }

        // Replace SetStatusColor(a.Status) with SetStatusColor(GetQuestStatus(a))
        public void ShowAllQuests()
        {
            if (quests.Count == 0)
            {
                Console.WriteLine("No quest yet.");
                return;
            }
            var sorted = quests.OrderBy(a => a.DueDate);

            foreach (var a in sorted)
            {
                SetStatusColor(GetQuestStatus(a)); // fixed: use derived status
                Console.WriteLine(a.GetSummary());
            }

            foreach (var a in sorted)
            {
                SetStatusColor(GetQuestStatus(a)); // fixed: use derived status
                Console.WriteLine(a.GetSummary());
                Console.WriteLine($"{a.GetDaysSinceApplied()} days have passed since application.");
            }

            Console.ResetColor(); //återställ färg till default 
        }

       public void UpdateQuest() 
        {
            Console.WriteLine("Enter part of the quest title to update (or number):");
            var raw = Console.ReadLine();
            var query = (raw ?? "").Trim();

            // Om användaren skrev en siffra, låt den vara indexval direkt
            if (int.TryParse(query, out int idx) && idx >= 1 && idx <= quests.Count)
            {
                var questByIndex = quests[idx - 1];
                EditQuest(questByIndex); // din fortsatta uppdateringslogik
                return;
            }

            // Normalisera: ta bort allt som inte är bokstav/siffra och gör till lowercase
            string Normalize(string s) =>
                Regex.Replace(s ?? "", @"[^0-9A-Za-zåäöÅÄÖ]", "").ToLowerInvariant();

            var normQuery = Normalize(query);

            var matches = quests
                .Where(q => Normalize(q.Title).Contains(normQuery)) // bindestreck, mellanslag osv ignoreras
                .ToList();

            if (matches.Count == 0)
            {
                Console.WriteLine("No matching quests found.");
                return;
            }

            Console.WriteLine("\nFound quests:");
            for (int i = 0; i < matches.Count; i++)
                Console.WriteLine($"{i + 1}. {matches[i].Title} (Due: {matches[i].DueDate:d})");

            Console.Write("\nSelect a quest number to update: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > matches.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var quest = matches[choice - 1];
            EditQuest(quest); // fortsätt med din befintliga uppdateringskod
        }

        // Exempel: bryt ut din befintliga uppdateringsinmatning hit
        private void EditQuest(Quest quest)
        {
            //Console.WriteLine("Enter new description (leave blank to keep current):");
            //var newDescription = Console.ReadLine();
            //if (!string.IsNullOrWhiteSpace(newDescription))
            //    quest.Description = newDescription;

            Console.WriteLine("Enter new description (leave blank to auto-generate a fun one):");
            var newDescription = Console.ReadLine();

            // Om användaren lämnar tomt, generera en rolig description baserat på titeln
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                // Skapa en rolig beskrivning beroende på titel
                newDescription = quest.Title switch
                {
                    var t when t.Contains("Coffee", StringComparison.OrdinalIgnoreCase)
                        => "The caffeine beast stirs again... will you brew victory or chaos?",
                    var t when t.Contains("Wi-Fi", StringComparison.OrdinalIgnoreCase)
                        => "Once more, the sacred signal flickers in the void. Adventure awaits in the router’s shadow!",
                    var t when t.Contains("Deadline", StringComparison.OrdinalIgnoreCase)
                        => "The hourglass of fate is running dry. You must race against time itself!",
                    var t when t.Contains("USB", StringComparison.OrdinalIgnoreCase)
                        => "The legendary USB of Power was spotted near the desk of doom. Retrieve it before the night shift begins!",
                    var t when t.Contains("Monday", StringComparison.OrdinalIgnoreCase)
                        => "The Monday Boss rises again... armed with spreadsheets and despair!",
                    _ => "A mysterious new challenge emerges from the fog of destiny..."
                };

                Console.WriteLine($"✨ Auto-generated description: {newDescription}");
            }
            else
            {
                Console.WriteLine("Description updated.");
            }

            // Uppdatera quest
            quest.Description = newDescription;
            //cc

            Console.WriteLine("Enter new due date in days from now (leave blank to keep current):");
            var dueInput = Console.ReadLine();
            if (int.TryParse(dueInput, out int days))
                quest.DueDate = DateTime.Now.AddDays(days);

            Console.WriteLine("Enter new priority (1=High, 2=Medium, 3=Low) (leave blank to keep current):");
            var prioInput = Console.ReadLine();
            if (int.TryParse(prioInput, out int prio) && prio >= 1 && prio <= 3)
                quest.Priority = (Priority)(prio - 1);

            Console.WriteLine("\n✅ Quest updated!");

            //Console.WriteLine("Enter the title of the quest to update:");
            //var title = Console.ReadLine();
            //var quest = quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            //if (quest != null)
            //{
            //    Console.WriteLine("Enter new description (leave blank to keep current):");
            //    var newDescription = Console.ReadLine();
            //    if (!string.IsNullOrEmpty(newDescription))
            //    {
            //        quest.Description = newDescription;
            //    }
            //    Console.WriteLine("Enter new due date in days from now (leave blank to keep current):");
            //    var dueDateInput = Console.ReadLine();
            //    if (int.TryParse(dueDateInput, out int days))
            //    {
            //        quest.DueDate = DateTime.Now.AddDays(days);
            //    }
            //    Console.WriteLine("Enter new priority (1=High, 2=Medium, 3=Low) (leave blank to keep current):");
            //    var priorityInput = Console.ReadLine();
            //    if (int.TryParse(priorityInput, out int priority) && priority >= 1 && priority <= 3)
            //    {
            //        quest.Priority = (Priority)(priority - 1);
            //    }
            //    Console.WriteLine("Quest updated.");
            //}
            //else
            //{
            //    Console.WriteLine($"Quest '{title}' not found.");
            //}
        }

        public void CompleteQuest()
        {
            Console.WriteLine("Enter the title of the quest to mark as completed:");
            var title = Console.ReadLine();
            var quest = quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (quest != null)
            {
                quest.IsCompleted = true;
                Console.WriteLine($"Quest '{title}' marked as completed.");
            }
            else
            {
                Console.WriteLine($"Quest '{title}' not found.");
            }
        }

        private void SetStatusColor(string status)
        {
            switch (status)
            {
                case "Completed":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "Active":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "Overdue":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
        }

        public void NotifySoonOverdueQuests()
        {
            var overdueQuests = quests.Where(q => !q.IsCompleted && DateTime.Now.AddHours(24) > q.DueDate).ToList();
            if (overdueQuests.Count == 0)
            {
                Console.WriteLine("No overdue quests.");
                return;
            }
            Console.WriteLine("Overdue Quests:");
            NotifyHero(overdueQuests);
        }

        public void NotifyHero(List<Quest> quests) 
        {
                var config = new ConfigurationBuilder()
                                .AddUserSecrets<Program>()  // reads from your user secrets
                                .Build();

                var accountSid = config["Twilio:ACCOUNT_SID"];
                var authToken = config["Twilio:AUTH_TOKEN"];
                var fromNumber = config["Twilio:FROM_NUMBER"];
                var toNumber = config["Twilio:TO_NUMBER"];

                TwilioClient.Init(accountSid, authToken);

                var from = new PhoneNumber(fromNumber);
                var to = new PhoneNumber(toNumber);

                var body = "The following quests are overdue or due within 24 hours:\n" +
                           string.Join("\n", quests.Select(q => $"- {q.Title} (Due: {q.DueDate})"));
            var message = MessageResource.Create(
                    to: to,
                    from: from,
                    body: body

                );                
            }
    }
}
