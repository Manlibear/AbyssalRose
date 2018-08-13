using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbyssalRose.Models
{
    namespace GuildHallUpgradeModels
    {
        public class GuildHallUpgrade
        {
            public int UpgradeID;
            public string Name;
            public string Icon;

            public List<RequiredMaterial> RequiredMaterials;

            public class RequiredMaterial
            {
                public RequiredMaterial(int materialId, string name, int count, string type)
                {
                    this.MaterialID = materialId;
                    this.Name = name;
                    this.AmountNeeded = count;
                    this.Type = type;
                }

                public int MaterialID;
                public string Name;
                public int AmountNeeded;
                public int AmountInStash;
                public string Icon;
                public string Type;
            }
        }
    }
}