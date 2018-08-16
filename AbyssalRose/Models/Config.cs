namespace AbyssalRose.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Config
    {
        public int ID { get; set; }
        public string ConfigName { get; set; }
        public int FocusedUpgradeID { get; set; }
        public string DiscordGuildID { get; set; }
        public string DiscordBotToken { get; set; }
        public string DiscordClientID { get; set; }
        public string DiscordClientSecret { get; set; }
        public string GW2APIEndpoint { get; set; }
        public string GW2APIKey { get; set; }
        public string GW2GuildID { get; set; }
    }
}
