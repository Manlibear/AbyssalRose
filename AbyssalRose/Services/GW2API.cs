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


        public static List<GuildHallUpgrade> GetGuildUpgrades()
        {
            List<Guild.Upgrade> queryUpgrades = new List<Guild.Upgrade>();
            List<GuildHallUpgrade> upgrades = new List<GuildHallUpgrade>();

            var client = new RestClient(ConfigHelper.Instance.GW2APIEndpoint);
            var request = new RestRequest("v2/guild/upgrades", Method.GET);

            RestResponse<List<int>> response = (RestResponse<List<int>>)client.Execute<List<int>>(request);

            int size = 200;
            if (response.Data.Count > 0)
            {
                var batches = response.Data.Batch(size).ToList();

                foreach (var b in batches)
                {
                    string ids = string.Join(",", b.ToList());
                    var upgradeRequest = new RestRequest("v2/guild/upgrades?ids=" + ids, Method.GET);
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
                    ghup.RequiredMaterials.AddRange(gUp.Costs.Where(x => x.Type == "Item").Select(x => new GuildHallUpgrade.RequiredMaterial(x.ID, x.Name, x.Count, x.Type)));
                    upgrades.Add(ghup);
                }
            }

            return upgrades;
        }

        public static void AddIconsToRequiredMaterials(ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            //API
            GetIconsByType("Item", "Items", ref mats);
            GetIconsByType("Coins", "Currencies", ref mats);

            //Manual
            SetLocalIcons("Aetherium", "/images/aetherium.png", ref mats);
            SetLocalIcons("Guild Favor", "/images/favor.png", ref mats);
        }

        private static void SetLocalIcons(string name, string icon, ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            if (mats.Where(x => x.Name == name).Count() > 0)
            {
                mats.Where(x => x.Name == name).First().Icon = icon;
            }
        }

        private static void GetIconsByType(string type, string apiEndpoint, ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            string ids = String.Join(",", mats.Where(x => x.Type == type).Select(x => x.MaterialID).ToList());

            if (ids.Length > 0)
            {
                var iconRequest = new RestRequest("v2/" + apiEndpoint + "?ids=" + ids, Method.GET);

                var client = new RestClient(ConfigHelper.Instance.GW2APIEndpoint);
                List<object> iconResponse = (List<object>)client.Execute<List<object>>(iconRequest).Data;

                foreach (dynamic iconResult in iconResponse)
                {
                    mats.Where(x => x.MaterialID == iconResult["id"]).First().Icon = iconResult["icon"];
                }
            }
        }

        public static void AddGuildStashAmountToMaterials(ref List<GuildHallUpgrade.RequiredMaterial> mats)
        {
            string accessToken = "?access_token=" + ConfigHelper.Instance.GW2APIKey;
            var client = new RestClient(ConfigHelper.Instance.GW2APIEndpoint);
            var stashRequest = new RestRequest("v2/guild/" + ConfigHelper.Instance.GW2GuildID + "/treasury" + accessToken, Method.GET);
            dynamic stashResponse = client.Execute<dynamic>(stashRequest).Data;

            if (stashResponse.Count > 0) //Guild has not unlocked stash if 0
            {
                foreach (dynamic stashItem in stashResponse)
                {
                    var matInStash = mats.Where(x => x.MaterialID == Convert.ToInt32(stashItem["item_id"]));

                    if (matInStash.Count() > 0)
                    {
                        matInStash.First().AmountInStash = Convert.ToInt32(stashItem["count"]);
                    }
                }
            }
        }
    }
}
