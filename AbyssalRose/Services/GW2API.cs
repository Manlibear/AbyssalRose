using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbyssalRose.Models;
using AbyssalRose.Extensions;
using RestSharp.Deserializers;
using AbyssalRose.Models.GuildHallUpgradeModels;

namespace AbyssalRose.Services
{


    public class GW2API
    {
        #region API Models
        private class Guild
        {
            [DeserializeAs(Name = "id")]
            public string ID { get; set; }

            [DeserializeAs(Name = "name")]
            public string Name { get; set; }

            [DeserializeAs(Name = "description")]
            public string Description { get; set; }

            [DeserializeAs(Name = "type")]
            public string Type { get; set; }


            public class Upgrade
            {
                [DeserializeAs(Name = "id")]
                public int ID { get; set; }

                [DeserializeAs(Name = "name")]
                public string Name { get; set; }

                [DeserializeAs(Name = "type")]
                public string Type { get; set; }

                [DeserializeAs(Name = "build_time")]
                public int BuildTime { get; set; }

                [DeserializeAs(Name = "icon")]
                public string Icon { get; set; }

                [DeserializeAs(Name = "costs")]
                public List<Cost> Costs { get; set; }

                public class Cost
                {
                    [DeserializeAs(Name = "item_id")]
                    public int ID { get; set; }

                    [DeserializeAs(Name = "name")]
                    public string Name { get; set; }

                    [DeserializeAs(Name = "type")]
                    public string Type { get; set; }

                    [DeserializeAs(Name = "count")]
                    public int Count { get; set; }
                }
            }
        }
        #endregion




        //TODO: Refactor on Config table
        const string endpoint = "https://api.guildwars2.com/v2";
        const string guildID = "9D9D532B-0743-E611-80D4-E4115BEBA648";
        
        public static List<GuildHallUpgrade> GetGuildUpgrades()
        {
            List<Guild.Upgrade> queryUpgrades = new List<Guild.Upgrade>();
            List<GuildHallUpgrade> upgrades = new List<GuildHallUpgrade>();

            var client = new RestClient(endpoint);
            var request = new RestRequest("guild/upgrades", Method.GET);

            RestResponse<List<int>> response = (RestResponse<List<int>>)client.Execute<List<int>>(request);

            int size = 200;
            var batches = response.Data.Batch(size).ToList();

            foreach (var b in batches)
            {

                string ids = string.Join(",", b.ToList());
                var upgradeRequest = new RestRequest("guild/upgrades?ids=" + ids, Method.GET);
                RestResponse<List<Guild.Upgrade>> upgradeResponse = (RestResponse<List<Guild.Upgrade>>)client.Execute<List<Guild.Upgrade>>(upgradeRequest);

                queryUpgrades.AddRange(upgradeResponse.Data.Where(x => x.Type == "Unlock" && x.BuildTime == 0));

            }

            foreach (Guild.Upgrade gUp in queryUpgrades)
            {
                GuildHallUpgrade ghup = new GuildHallUpgrade();
                ghup.Name = gUp.Name;
                ghup.UpgradeID = gUp.ID;
                ghup.Icon = gUp.Icon;
                ghup.RequiredMaterials = new List<GuildHallUpgrade.RequiredMaterial>();
                ghup.RequiredMaterials.AddRange(gUp.Costs.Select(x => new GuildHallUpgrade.RequiredMaterial(x.ID, x.Name, x.Count, x.Type)));
                upgrades.Add(ghup);
            }

            return upgrades;
        }

        public static void AddIconsToRequiredMaterials(ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            //Items
            GetIconsByType("Item", "Items", ref mats);
            GetIconsByType("Coins", "Currencies", ref mats);
            //GetIconsByType("Currency", "Currencies", ref mats);
            //GetIconsByType("Collectible", "Currencies", ref mats);

            mats.Where(x => x.Name == "Aetherium").First().Icon = "/images/aetherium.png";
            mats.Where(x => x.Name == "Guild Favor").First().Icon = "/images/favor.png";
        }

        private static void GetIconsByType(string type, string apiEndpoint, ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            string ids = String.Join(",", mats.Where(x => x.Type == type && x.Name != "Aetherium").Select(x => x.MaterialID).ToList());
            if (ids.Length > 0)
            {
                var iconRequest = new RestRequest(apiEndpoint + "?ids=" + ids, Method.GET);

                var client = new RestClient(endpoint);
                RestResponse<List<object>> iconResponse = (RestResponse<List<object>>)client.Execute<List<object>>(iconRequest);

                foreach (dynamic iconResult in iconResponse.Data)
                {
                    mats.Where(x => x.MaterialID == iconResult["id"]).First().Icon = iconResult["icon"];
                }
            }
        }

        public static void AddGuildStashAmountToMaterials(ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            var client = new RestClient(endpoint);
            var stashRequest = new RestRequest("guild/" + guildID + "/stash", Method.GET);
            RestResponse<dynamic> stashResponse = (RestResponse<dynamic>)client.Execute<dynamic>(stashRequest);

            //foreach(dynamic stashItem in stashResponse.Data.inventory)
            //{
            //    mats.Where(x => x.MaterialID == stashItem.id).First().AmountInStash = stashItem.count;
            //}
        }
    }
}
