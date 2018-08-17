using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AbyssalRose.Models
{
    public class Event
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }

        public enum EventType
        {
            PvP = 1,
            WvW = 2,
            Missions = 3,
            Fractals = 4,
            PvE = 5,
            Social = 6
        }
    }
}