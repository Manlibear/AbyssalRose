using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AbyssalRose.Models
{
    public class EventsViewModel
    {
        public List<Event> AllEvents;

        public string GetEvents()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<Dictionary<string, object>> lde = new List<Dictionary<string, object>>();

            //TODO: Consider the range here, newer than one month old
            DateTime startDate = DateTime.Today.AddMonths(-1);
            List<Event> events = db.Events.Where(x => x.StartDate > startDate).ToList();

            foreach (Event e in events)
            {
                Dictionary<string, object> de = new Dictionary<string, object>
                {
                    { "title", e.Title },
                    { "allDay", false },
                    { "start", FormatDateTime(e.StartDate) },
                    { "end", FormatDateTime(e.EndDate) },
                    { "className", "event-" + e.Type.ToString() }
                };

                lde.Add(de);
            }
            
            return SimpleJson.SimpleJson.SerializeObject(lde);
        }

        public string FormatDateTime(DateTime d)
        {
            return d.ToString("yyyy-MM-dd") + "T" + d.ToString("HH:mm:ss");
        }
    }
}