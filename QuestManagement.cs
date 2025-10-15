using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
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
            Console.WriteLine("Enter quest title:");
            var title = Console.ReadLine();


            var description = CreateAIDescription(title).Result;
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
         Console.WriteLine("Enter the title of the quest to update:");
            var title = Console.ReadLine();
            var quest = quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (quest != null)
            {
                Console.WriteLine("Enter new description (leave blank to keep current):");
                var newDescription = Console.ReadLine();
                if (!string.IsNullOrEmpty(newDescription))
                {
                    quest.Description = newDescription;
                }
                Console.WriteLine("Enter new due date in days from now (leave blank to keep current):");
                var dueDateInput = Console.ReadLine();
                if (int.TryParse(dueDateInput, out int days))
                {
                    quest.DueDate = DateTime.Now.AddDays(days);
                }
                Console.WriteLine("Enter new priority (1=High, 2=Medium, 3=Low) (leave blank to keep current):");
                var priorityInput = Console.ReadLine();
                if (int.TryParse(priorityInput, out int priority) && priority >= 1 && priority <= 3)
                {
                    quest.Priority = (Priority)(priority - 1);
                }
                Console.WriteLine("Quest updated.");
            }
            else
            {
                Console.WriteLine($"Quest '{title}' not found.");
            }
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
