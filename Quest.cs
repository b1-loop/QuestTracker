using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace QuestTracker
{
    //Title(uppdragsnamn)
    //Description(beskrivning av uppdraget)
    //DueDate(när uppdraget måste slutföras)
    //Priority(Hög, Medium, Låg)
    //IsCompleted(om uppdraget är klart)

    public enum Priority
    {
        High,
        Medium,
        Low
    }
    public class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string GetSummary()
        {
            // Implementation of summary (existing or placeholder)
            return $"{Title} - {Description} (Due: {DueDate.ToShortDateString()}, Priority: {Priority}, Completed: {IsCompleted})";
        }

        public int GetDaysSinceApplied()
        {
            return (DateTime.Now - CreatedDate).Days;
        }
    }

}
