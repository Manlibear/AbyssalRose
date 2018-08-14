using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbyssalRose.Models
{
    public class Events
    {
        public int ID;
        public string Title;
        public DateTime Date;
        public string Description;
        public EventType Type;

        public enum EventType
        {
            PvP,
            WvW,
            Missions,
            Fractals,
            PvE,
            Social
        }
    }
}