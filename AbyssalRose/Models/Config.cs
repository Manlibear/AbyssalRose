namespace AbyssalRose.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Config
    {
        public int ID { get; set; }
        public string ConfigName { get; set; }
        public int FocusedUpgradeID { get; set; }
        public string GuildID { get; set; }
        public string BotToken { get; set; }
        public string DiscordClientID { get; set; }
        public string DiscordClientSecret { get; set; }
    }
}
